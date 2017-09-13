using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace DotNetApis.SimpleInjector
{
    /// <summary>
    /// Singleton creation helper methods.
    /// Each singleton created by this type will automatically retry creation if it fails.
    /// </summary>
    public static class Singleton
    {
        public static ISingleton<T> Create<T>(Func<T> factory) => new SynchronousSingleton<T>(factory);
        public static IAsyncSingleton<T> Create<T>(Func<Task<T>> factory) => new AsynchronousSingleton<T>(factory);
    }

    public interface ISingleton<out T>
    {
        T Value { get; }
    }

    public interface IAsyncSingleton<T> : ISingleton<Task<T>>
    {
    }

    public sealed class SynchronousSingleton<T> : ISingleton<T>
    {
        private readonly Lazy<T> _lazy;

        public SynchronousSingleton(Func<T> factory)
        {
            _lazy = new Lazy<T>(factory, LazyThreadSafetyMode.PublicationOnly);
        }

        public T Value => _lazy.Value;
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
        public static TaskAwaiter<T> GetAwaiter<T>(this ISingleton<Task<T>> @this) => @this.Value.GetAwaiter();
    }
}
