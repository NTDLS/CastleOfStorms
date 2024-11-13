#define EnableCrudeInstrumentation

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cos.Library
{
    public static class CrudeInstrumentation
    {
        public static InstrumentationMetrics Metrics { get; private set; } = new();

        public class InstrumentationMetrics
        {
            public Dictionary<string, InstrumentationMetric> Collection = new();

            //QuickWatch: Cos.Library.CrudeInstrumentation.Metrics.Ordered
            public List<KeyValuePair<string, InstrumentationMetric>> Ordered
                => Collection.OrderByDescending(static o => o.Value.Milliseconds).ToList();
        }

        public class InstrumentationMetric
        {
            public ulong Count { get; set; }
            public double Milliseconds { get; set; }
        }

        public delegate void CrudeInstrumentationProc();
        public delegate T CrudeInstrumentationProc<T>();
        public delegate T? CrudeInstrumentationNullableProc<T>();

        class MetricsTextItem
        {
            public string Milliseconds { get; set; } = string.Empty;
            public string Average { get; set; } = string.Empty;
            public string Count { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

#if !EnableCrudeInstrumentation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Witness(CrudeInstrumentationProc proc) => proc();
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Witness(string key, CrudeInstrumentationProc proc)
        {
            var sw = Stopwatch.StartNew();
            proc();
            sw.Stop();

            lock (Metrics)
            {
                if (Metrics.Collection.TryGetValue(key, out var metrics))
                {
                    metrics.Count++;
                    metrics.Milliseconds += sw.ElapsedMilliseconds;
                }
                else
                {
                    metrics = new InstrumentationMetric()
                    {
                        Count = 1,
                        Milliseconds = sw.ElapsedMilliseconds,
                    };
                    Metrics.Collection.Add(key, metrics);
                }
            }
        }
#endif

#if !EnableCrudeInstrumentation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Witness<T>(CrudeInstrumentationProc<T> proc) => proc();
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Witness<T>(string key, CrudeInstrumentationProc<T> proc)
        {
            var sw = Stopwatch.StartNew();
            T result = proc();
            sw.Stop();

            lock (Metrics)
            {
                if (Metrics.Collection.TryGetValue(key, out var metrics))
                {
                    metrics.Count++;
                    metrics.Milliseconds += sw.ElapsedMilliseconds;
                }
                else
                {
                    metrics = new InstrumentationMetric()
                    {
                        Count = 1,
                        Milliseconds = sw.ElapsedMilliseconds,
                    };
                    Metrics.Collection.Add(key, metrics);
                }
            }

            return result;
        }
#endif

#if !EnableCrudeInstrumentation
        public static T? Witness<T>(CrudeInstrumentationNullableProc<T?> proc) => proc();
#else
        public static T? Witness<T>(string key, CrudeInstrumentationNullableProc<T?> proc)
        {
            var sw = Stopwatch.StartNew();
            T? result = proc();
            sw.Stop();

            lock (Metrics)
            {
                if (Metrics.Collection.TryGetValue(key, out var metrics))
                {
                    metrics.Count++;
                    metrics.Milliseconds += sw.ElapsedMilliseconds;
                }
                else
                {
                    metrics = new InstrumentationMetric()
                    {
                        Count = 1,
                        Milliseconds = sw.ElapsedMilliseconds,
                    };
                    Metrics.Collection.Add(key, metrics);
                }
            }

            return result;
        }
#endif
    }
}
