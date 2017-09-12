using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace FunctionApp
{
    public sealed class AutomaticInstanceOf : IDependencyInjectionBehavior
    {
        private readonly IDependencyInjectionBehavior _decorated;
        private readonly Container _container;

        public AutomaticInstanceOf(Container container, IDependencyInjectionBehavior decorated)
        {
            _container = container;
            _decorated = decorated;
        }

        public InstanceProducer GetInstanceProducer(InjectionConsumerInfo consumer, bool throwOnFailure)
        {
            var producer = TryGetRegistration(consumer);
            if (producer == null)
                return _decorated.GetInstanceProducer(consumer, throwOnFailure);
            var instance = producer.ServiceType.GetProperty("Value").GetValue(producer.GetInstance());
            return InstanceProducer.FromExpression(consumer.Target.TargetType, Expression.Constant(instance), _container);
        }

        public void Verify(InjectionConsumerInfo consumer)
        {
            if (TryGetRegistration(consumer) == null)
                _decorated.Verify(consumer);
        }

        private InstanceProducer TryGetRegistration(InjectionConsumerInfo consumer)
        {
            if (consumer.ImplementationType.Name == "AzurePackageJsonTable")
                Debugger.Break();

            var instanceOfType = typeof(InstanceOf<>.For<>).MakeGenericType(consumer.Target.TargetType, consumer.Target.Member.DeclaringType);
            return _container.GetCurrentRegistrations().FirstOrDefault(r => r.ServiceType == instanceOfType);
        }
    }

    public static class AutomaticInstanceOfExtensions
    {
        public static void UseAutomaticInstanceOf(this Container container)
        {
            container.Options.DependencyInjectionBehavior = new AutomaticInstanceOf(container, container.Options.DependencyInjectionBehavior);
        }
    }
}
