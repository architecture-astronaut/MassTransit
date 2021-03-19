namespace MassTransit.RedisIntegration.Serializers
{
    using System.IO;
    using Courier;


    public class MassTransitJsonSerializer<T> :
        ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            SerializerCache.Serializer.Serialize(writer, data);

            writer.Flush();
            return stream.ToArray();
        }
    }
}
