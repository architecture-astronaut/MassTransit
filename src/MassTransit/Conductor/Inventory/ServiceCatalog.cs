namespace MassTransit.Conductor.Inventory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes.Internals.Extensions;
    using Internals.GraphValidation;


    public class ServiceCatalog :
        IServiceCatalog
    {
        readonly Dictionary<Type, IServiceRegistration> _registrations;

        public ServiceCatalog(Dictionary<Type, IServiceRegistration> registrations)
        {
            _registrations = registrations;
        }

        public IExecutionPlanner<TResult> GetExecutionPlanner<TResult>(params Type[] inputTypes)
            where TResult : class
        {
            if (!_registrations.TryGetValue(typeof(TResult), out var registration))
                throw new ConfigurationException($"Service type not registered: {TypeCache<TResult>.ShortName}");

            IServiceRegistration[] serviceRegistrations = GetServiceRegistrations(registration);

            List<Type> missingInputTypes = inputTypes.Except(serviceRegistrations.SelectMany(r => r.Providers).Select(p => p.InputType)).ToList();
            if (missingInputTypes.Any())
            {
                var missingInputTypeNames = string.Join(", ", missingInputTypes.Select(x => TypeCache.GetShortName(x)));

                throw new ConfigurationException($"One or more input types were not found: {missingInputTypeNames}");
            }

            return new ExecutionPlanner<TResult>(serviceRegistrations);
        }

        IServiceRegistration[] GetServiceRegistrations(IServiceRegistration registration)
        {
            DependencyGraph<IServiceRegistration> graph = BuildRegistrationGraph(_registrations);

            return graph.GetItemsInOrder(registration).ToArray();
        }

        static DependencyGraph<IServiceRegistration> BuildRegistrationGraph(Dictionary<Type, IServiceRegistration> registrations)
        {
            var graph = new DependencyGraph<IServiceRegistration>(registrations.Count);

            foreach (var registration in registrations.Values)
            {
                graph.Add(registration);

                foreach (var provider in registration.Providers)
                {
                    if (registrations.TryGetValue(provider.InputType, out var inputRegistration))
                        graph.Add(registration, inputRegistration);
                }
            }

            graph.EnsureGraphIsAcyclic();

            return graph;
        }
    }
}
