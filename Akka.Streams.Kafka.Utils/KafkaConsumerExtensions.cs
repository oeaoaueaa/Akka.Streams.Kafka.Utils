using Akka.Event;
using Confluent.Kafka;
using System;
using System.Linq;

namespace Akka.Streams.Kafka.Utils
{
    public static class KafkaConsumerExtensions
    {
        public static TopicPartitionOffset[] LastMessagePartitionOffsets<TKey, TValue>(
            this IConsumer<TKey, TValue> consumer,
            string topic,
            int topicPartitionCount,
            int timeoutMs = 30000,
            ILoggingAdapter log = null)
        {
            var waterMarkOffsetsAndRestart = Enumerable.Range(0, topicPartitionCount).Select(p =>
            {
                var topicPartition = new TopicPartition(topic, p);
                var watermarkOffsets = consumer.QueryWatermarkOffsets(topicPartition, TimeSpan.FromMilliseconds(timeoutMs));

                var topicPartitionOffset = watermarkOffsets.High > 0 ?
                        new TopicPartitionOffset(topicPartition, watermarkOffsets.High - 1)
                        : new TopicPartitionOffset(topicPartition, Offset.Beginning);

                return (watermarkOffsets, topicPartitionOffset);
            }).ToArray();

            log?.Debug($"LastMessagePartitionOffsets: {string.Join("| ", waterMarkOffsetsAndRestart.Select(wmotpo => $"{wmotpo.watermarkOffsets.ToString()} {wmotpo.topicPartitionOffset.ToString()}"))}");

            return waterMarkOffsetsAndRestart.Select(wmotpo => wmotpo.topicPartitionOffset).ToArray();
        }
    }

}
