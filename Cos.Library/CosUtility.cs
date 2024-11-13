using Cos.Library.Exceptions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Cos.Library
{
    public static class CosUtility
    {
        public delegate void TryAndIgnoreProc();
        public delegate T TryAndIgnoreProc<T>();

        public delegate void DebugPrintDurationProc();
        public delegate T DebugPrintDurationProc<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DebugPrintDuration(string displayName, DebugPrintDurationProc func)
        {
#if DEBUG
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            func();
            stopwatch.Stop();

            Debug.WriteLine($"{displayName}\t{stopwatch.ElapsedMilliseconds:n0}");
#else
            func();
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? DebugPrintDuration<T>(string displayName, DebugPrintDurationProc<T> func)
        {
#if DEBUG
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var result = func();
            stopwatch.Stop();

            Debug.WriteLine($"{displayName}\t{stopwatch.ElapsedMilliseconds:n0}");

            return result;
#else
            return func();
#endif
        }


        public static int GreaterOf(int one, int two) => (one > two) ? one : two;
        public static int LesserOf(int one, int two) => (one < two) ? one : two;
        public static uint GreaterOf(uint one, uint two) => (one > two) ? one : two;
        public static uint LesserOf(uint one, uint two) => (one < two) ? one : two;


        public delegate ReturnType InterlockProc<ReturnType, LockObjType>(LockObjType obj);

        /// <summary>
        /// Returns a value allowing for only single threaded access.
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <typeparam name="LockObjType"></typeparam>
        /// <param name="objectToLock"></param>
        /// <param name="proc"></param>
        /// <returns></returns>
        public static ReturnType? Interlock<ReturnType, LockObjType>(LockObjType objectToLock, InterlockProc<ReturnType, LockObjType> proc) where LockObjType : class
        {
            lock (objectToLock)
            {
                return proc(objectToLock);
            }
        }

        /// <summary>
        /// Returns if the number square (the product of a number multiplied my itself).
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsSquareNumber(int number)
        {
            if (number < 0)
            {
                return false; // Negative numbers cannot be square numbers.
            }

            // Calculate the square root of the number.
            int sqrt = (int)Math.Sqrt(number);

            // Check if the square of the square root is equal to the original number.
            return sqrt * sqrt == number;
        }

        /// <summary>
        /// Executes the given delegate and returns true if successful.
        /// </summary>
        /// <param name="func"></param>
        /// <returns>Returns TRUE successful, returns FALSE if an error occurs.</returns>
        public static bool TryAndIgnore(TryAndIgnoreProc func)
        {
            try { func(); return true; } catch { return false; }
        }

        /// <summary>
        /// We didn't need that exception! Did we?... DID WE?!
        /// </summary>
        public static T? TryAndIgnore<T>(TryAndIgnoreProc<T> func)
        {
            try { return func(); } catch { }
            return default;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
            => (new StackTrace())?.GetFrame(1)?.GetMethod()?.Name ?? "{unknown frame}";


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNull<T>([NotNull] T? value, string? message = null, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (value == null)
            {
                if (message == null)
                {
                    throw new CosNullException($"Value should not be null: '{strName}'.");
                }
                else
                {
                    throw new CosNullException(message);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNullOrEmpty([NotNull] Guid? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (value == null || value == Guid.Empty)
            {
                throw new CosNullException($"Value should not be null or empty: '{strName}'.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNullOrEmpty([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new CosNullException($"Value should not be null or empty: '{strName}'.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string strName = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new CosNullException($"Value should not be null or empty: '{strName}'.");
            }
        }

        [Conditional("DEBUG")]
        public static void AssertIfDebug(bool condition, string message)
        {
            if (condition)
            {
                throw new CosAssertException(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Assert(bool condition, string message)
        {
            if (condition)
            {
                throw new CosAssertException(message);
            }
        }
    }
}
