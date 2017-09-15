using System;
using System.Linq;
using System.Linq.Expressions;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace DotNetApis.SimpleInjector
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
            var instanceOfType = InstanceOfType(consumer);
            var producer = TryGetRegistration(instanceOfType);
            if (producer == null)
                return _decorated.GetInstanceProducer(consumer, throwOnFailure);
            var registration = producer.Lifestyle.CreateRegistration(consumer.Target.TargetType, () =>
            {
                var instanceOf = _container.GetInstance(instanceOfType);
                return instanceOfType.GetProperty("Value").GetValue(instanceOf);
            }, _container);
            return new InstanceProducer(consumer.Target.TargetType, registration);
        }

        public void Verify(InjectionConsumerInfo consumer)
        {
            if (TryGetRegistration(InstanceOfType(consumer)) == null)
                _decorated.Verify(consumer);
        }

        private static Type InstanceOfType(InjectionConsumerInfo consumer) => typeof(InstanceOf<>.For<>).MakeGenericType(consumer.Target.TargetType, consumer.Target.Member.DeclaringType);

        private InstanceProducer TryGetRegistration(Type instanceOfType) => _container.GetCurrentRegistrations().FirstOrDefault(r => r.ServiceType == instanceOfType);
    }

    public static class AutomaticInstanceOfExtensions
    {
        public static void UseAutomaticInstanceOf(this Container container)
        {
            container.Options.DependencyInjectionBehavior = new AutomaticInstanceOf(container, container.Options.DependencyInjectionBehavior);
        }
    }
}
