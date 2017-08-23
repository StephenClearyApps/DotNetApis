using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.Disposables;

namespace Common
{
    public sealed class LoggerScope
    {
        private readonly AsyncLocalStack<ImmutableDictionary<string, object>> Scopes = new AsyncLocalStack<ImmutableDictionary<string, object>>();

        public IDisposable Push(object state)
        {
            if (!(state is IEnumerable<KeyValuePair<string, object>> stateValues))
                return new AnonymousDisposable(() => { }); // todo: noopdisposable

            var result = ImmutableDictionary.CreateBuilder<string, object>();
            stateValues.Do(x => result[x.Key] = x.Value);
            Scopes.Push(result.ToImmutable());

            return new AnonymousDisposable(() => Scopes.Pop());
        }

        public IDictionary<string, object> GetMergedStateDictionary()
        {
            var result = new Dictionary<string, object>();

            foreach (var scope in Scopes.CurrentStack)
            {
                foreach (var value in scope)
                {
                    result[value.Key] = value.Value;
                }
            }

            return result;
        }
    }
}
