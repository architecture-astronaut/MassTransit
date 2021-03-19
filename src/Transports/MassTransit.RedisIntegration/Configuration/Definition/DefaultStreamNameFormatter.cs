namespace MassTransit.RedisIntegration.Definition
{
    using System;
    using Topology;


    public class DefaultStreamNameFormatter :
        IEntityNameFormatter
    {
        protected DefaultStreamNameFormatter()
        {
        }

        public static IEntityNameFormatter Instance { get; } = new DefaultStreamNameFormatter();

        public string FormatEntityName<T>()
        {
            return GetMessageName(typeof(T));
        }

        protected virtual string SanitizeName(string name)
        {
            return name.ToLowerInvariant();
        }

        string GetMessageName(Type type)
        {
            if (type.IsGenericType)
                return SanitizeName(type.GetGenericArguments()[0].Name);

            var messageName = type.Name;

            return SanitizeName(messageName);
        }
    }
}
