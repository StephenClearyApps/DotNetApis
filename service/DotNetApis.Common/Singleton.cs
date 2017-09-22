using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace DotNetApis.Common
{
    /// <summary>
    /// Singleton creation helper methods.
    /// Each singleton created by this type will automatically retry creation if it fails.
    /// </summary>
    public static class Singleton
    {
        /// <summary>
        /// Creates a synchronous singleton. The <paramref name="factory"/> delegate will be invoked the first time <see cref="ISingleton{T}.Value"/> is retrieved.
        /// If <paramref name="factory"/> throws an exception, then that exception is propogated but not cached; <paramref name="factory"/> will be invoked again the next time <see cref="ISingleton{T}.Value"/> is retrieved.
        /// If <see cref="ISingleton{T}.Value"/> is retrieved simultaneously from multiple threads, then <paramref name="factory"/> will be invoked once by the first thread; all other threads will block until <paramref name="factory"/> completes, and all will receive the resulting value or exception.
        /// </summary>
        /// <typeparam name="T">The type being created.</typeparam>
        /// <param name="factory">The factory method.</param>
        public static ISingleton<T> Create<T>(Func<T> factory) => new SynchronousSingleton<T>(factory);

        /// <summary>
        /// Creates an asynchronous singleton. The <paramref name="factory"/> delegate will be invoked the first time <see cref="ISingleton{T}.Value"/> is retrieved.
        /// If <paramref name="factory"/> throws an exception, then that exception is propogated but not cached; <paramref name="factory"/> will be invoked again the next time <see cref="ISingleton{T}.Value"/> is retrieved.
        /// If <see cref="ISingleton{T}.Value"/> is awaited simultaneously from multiple code paths, then <paramref name="factory"/> will be invoked once by the first thread; all other code paths will asynchronously wait until <paramref name="factory"/> completes, and all code paths will receive the resulting value or exception.
        /// </summary>
        /// <typeparam name="T">The type being created.</typeparam>
        /// <param name="factory">The factory method.</param>
        public static IAsyncSingleton<T> Create<T>(Func<Task<T>> factory) => new AsynchronousSingleton<T>(factory);

        /// <summary>
        /// Creates a synchronous initialization. The <paramref name="factory"/> delegate will be invoked the first time <see cref="ISingleton{T}.Value"/> is retrieved.
        /// If <paramref name="factory"/> throws an exception, then that exception is propogated but not cached; <paramref name="factory"/> will be invoked again the next time <see cref="ISingleton{T}.Value"/> is retrieved.
        /// If <see cref="ISingleton{T}.Value"/> is retrieved simultaneously from multiple threads, then <paramref name="factory"/> will be invoked once by the first thread; all other threads will block until <paramref name="factory"/> completes, and all will receive the resulting success or exception.
        /// </summary>
        /// <param name="factory">The factory method.</param>
        public static ISingleton<object> Create(Action factory) => Create(() =>
        {
            factory();
            return (object)null;
        });

        /// <summary>
        /// Creates an asynchronous singleton. The <paramref name="factory"/> delegate will be invoked the first time <see cref="ISingleton{T}.Value"/> is retrieved.
        /// If <paramref name="factory"/> throws an exception, then that exception is propogated but not cached; <paramref name="factory"/> will be invoked again the next time <see cref="ISingleton{T}.Value"/> is retrieved.
        /// If <see cref="ISingleton{T}.Value"/> is awaited simultaneously from multiple code paths, then <paramref name="factory"/> will be invoked once by the first thread; all other code paths will asynchronously wait until <paramref name="factory"/> completes, and all code paths will receive the resulting success or exception.
        /// </summary>
        /// <param name="factory">The factory method.</param>
        public static IAsyncSingleton<object> Create(Func<Task> factory) => Create(async () =>
        {
            await factory().ConfigureAwait(false);
            return (object)null;
        });
    }

    /// <summary>
    /// A synchronous singleton.
    /// </summary>
    /// <typeparam name="T">The type being created.</typeparam>
    public interface ISingleton<out T>
    {
        T Value { get; }
    }

    /// <summary>
    /// An asynchronous singleton.
    /// </summary>
    /// <typeparam name="T">The type being created.</typeparam>
    public interface IAsyncSingleton<T>
    {
        Task<T> Value { get; }
    }

    public sealed class SynchronousSingleton<T> : ISingleton<T>
    {
        // Use AsyncLazy insted of Lazy so that the factory method is only called once when initialized from multiple threads simultaneously.
        private readonly AsyncLazy<T> _lazy;

        public SynchronousSingleton(Func<T> factory)
        {
#pragma warning disable 1998
            _lazy = new AsyncLazy<T>(async () => factory(), AsyncLazyFlags.ExecuteOnCallingThread | AsyncLazyFlags.RetryOnFailure);
#pragma warning restore 1998
        }

        public T Value => _lazy.Task.GetAwaiter().GetResult();
    }

    public sealed class AsynchronousSingleton<T> : IAsyncSingleton<T>
    {
        private readonly AsyncLazy<T> _lazy;

        public AsynchronousSingleton(Func<Task<T>> factory)
        {
            _lazy = new AsyncLazy<T>(factory, AsyncLazyFlags.ExecuteOnCallingThread | AsyncLazyFlags.RetryOnFailure);
        }

        public Task<T> Value => _lazy.Task;
    }

    public static class SingletonExtensions
    {
        /// <summary>
        /// Enables asynchronous singletons to be awaited directly.
        /// </summary>
        /// <typeparam name="T">The type being created.</typeparam>
        /// <param name="this">The singleton.</param>
        public static TaskAwaiter<T> GetAwaiter<T>(this IAsyncSingleton<T> @this) => @this.Value.GetAwaiter();

        public static Task EnsureInitializedAsync<T>(this IAsyncSingleton<T> @this) => @this.Value;
        public static void EnsureInitialized<T>(this ISingleton<T> @this)
        {
            var _ = @this.Value;
        }
    }
}
