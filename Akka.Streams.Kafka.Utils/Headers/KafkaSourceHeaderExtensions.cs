using Confluent.Kafka;
using System;
using System.Linq;

namespace Akka.Streams.Kafka.Utils.Headers
{
    public static class KafkaSourceHeaderExtensions
    {
        public const string HeaderSourcePartition = "_srcPrt";
        public const string HeaderSourceOffset = "_srcOff";

        public static void AddSourceHeader<TKey, TValue>(this Message<TKey, TValue> message,
            int partition, long offset)
        {
            if (message.Headers == null) message.Headers = new Confluent.Kafka.Headers();

            message.Headers.Add(HeaderSourcePartition, KafkaHeadersExtensions.GetHeaderBytes(partition));
            message.Headers.Add(HeaderSourceOffset, KafkaHeadersExtensions.GetHeaderBytes(offset));
        }

        public static (int partition, long offset)? GetSourceHeader<TKey, TValue>(this Message<TKey, TValue> message)
        {
            var partitionHeader = message.Headers?.FirstOrDefault(h => h.Key == HeaderSourcePartition);
            var partition = partitionHeader?.GetInt();

            var offsetHeader = message.Headers?.FirstOrDefault(h => h.Key == HeaderSourceOffset);
            var offset = offsetHeader?.GetLong();

            if (partition == null || offset == null)
                return null;

            return (partition.Value, offset.Value);
        }
    }
}
