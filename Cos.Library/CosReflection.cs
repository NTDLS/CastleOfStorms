using NTDLS.Helpers;
using NTDLS.Semaphore;
using System.Reflection;

namespace Cos.Library
{
    public static class CosReflection
    {
        private static readonly PessimisticCriticalResource<Dictionary<string, Type>> _typeCache = new();
        private static readonly PessimisticCriticalResource<Dictionary<string, PropertyInfo>> _staticPropertyCache = new();
        private static readonly PessimisticCriticalResource<Dictionary<Type, List<Type>>> _subClassesOfCache = new();

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            return givenType.GetInterfaces()
                .Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                || givenType.BaseType != null && (givenType.BaseType.IsGenericType &&
                                                   givenType.BaseType.GetGenericTypeDefinition() == genericType ||
                                                   IsAssignableToGenericType(givenType.BaseType, genericType));
        }

        public static IEnumerable<Type> GetSubClassesOf<T>()
        {
            var cached = _subClassesOfCache.Use(o =>
            {
                o.TryGetValue(typeof(T), out var cached);
                return cached;
            });
            if (cached != null)
            {
                return cached;
            }

            List<Type> allTypes = [];

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                allTypes.AddRange(assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(T))));
            }

            _subClassesOfCache.Use(o => o.TryAdd(typeof(T), allTypes));

            return allTypes;
        }

        public static string GetStaticPropertyValue(string typeName, string propertyName)
        {
            string key = $"[{typeName}].[{propertyName}]";

            var cached = _staticPropertyCache.Use(o =>
            {
                if (o.TryGetValue(key, out var cachedPropertyInfo))
                {
                    return cachedPropertyInfo.GetValue(null) as string;
                }
                return null;
            });

            if (cached != null)
            {
                return cached;
            }

            var type = GetTypeByName(typeName) ?? throw new Exception("Type not found.");

            var propertyInfo = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            if (propertyInfo != null)
            {
                _staticPropertyCache.Use(o => o.TryAdd(key, propertyInfo));
                return propertyInfo.GetValue(null) as string ?? string.Empty;
            }

            throw new Exception("Static property not found: {typeName}->{propertyName}.");
        }

        public static T CreateInstanceFromType<T>(Type type, object[] constructorArgs)
        {
            return (T)Activator.CreateInstance(type, constructorArgs).EnsureNotNull();
        }

        public static T CreateInstanceFromType<T>(Type type)
        {
            return (T)Activator.CreateInstance(type).EnsureNotNull();
        }

        public static T CreateInstanceFromTypeName<T>(string typeName, object[] constructorArgs)
        {
            var type = GetTypeByName(typeName);
            return (T)Activator.CreateInstance(type, constructorArgs).EnsureNotNull();
        }

        public static T CreateInstanceFromTypeName<T>(string typeName)
        {
            var type = GetTypeByName(typeName);
            return (T)Activator.CreateInstance(type).EnsureNotNull();
        }

        public static bool DoesTypeExist(string typeName)
        {
            return GetTypeByName(typeName) != null;
        }

        public static Type GetTypeByName(string typeName)
        {
            var cached = _typeCache.Use(o =>
            {
                o.TryGetValue(typeName, out var cachedType);
                return cachedType;
            });

            if (cached != null)
            {
                return cached;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                if (type != null)
                {
                    _typeCache.Use(o => o.TryAdd(typeName, type));
                    return type;
                }
            }

            throw new Exception($"Type not found: {typeName}");
        }

        /// <summary>
        // Caches all types that inherit from T;
        /// </summary>
        public static void BuildReflectionCacheOfType<T>()
        {
            foreach (var item in GetSubClassesOf<T>())
            {
                _ = CosReflection.GetTypeByName(item.Name);
            }
        }

        public static T? CreateInstanceOf<T>(object[] constructorArgs)
        {
            return (T?)Activator.CreateInstance(typeof(T), constructorArgs);
        }
    }
}
