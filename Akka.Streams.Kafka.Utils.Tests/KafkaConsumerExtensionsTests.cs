using System;
using Xunit;
using Moq;
using Confluent.Kafka;

namespace Akka.Streams.Kafka.Utils.Tests
{
    public class KafkaConsumerExtensionsTests
    {
        [Fact]
        public void LastMessagePartitionOffsetsSingleOrEmptyTest()
        {
            // arrange
            const string topic = "Topic1";
            const int topicPartitionCount = 1;
            var timeoutMs = 3000;

            var consumer = Mock.Of<IConsumer<NotUsed, NotUsed>>(
                c => c.QueryWatermarkOffsets(new TopicPartition(topic, 0), TimeSpan.FromMilliseconds(timeoutMs)) 
                                            == new WatermarkOffsets(new Offset(0), new Offset(0)));

            // act
            var topicPartitionOffsets = KafkaConsumerExtensions.LastMessagePartitionOffsets(consumer, topic, topicPartitionCount, timeoutMs);

            // assert
            Assert.Single(topicPartitionOffsets);
            Assert.Equal(topic, topicPartitionOffsets[0].Topic);
            Assert.Equal(0, topicPartitionOffsets[0].Partition.Value);
            Assert.Equal(Offset.Beginning, topicPartitionOffsets[0].Offset);
        }

        [Fact]
        public void LastMessagePartitionOffsetsMultipleTest()
        {
            // arrange
            const string topic = "Topic1";
            const int topicPartitionCount = 2;
            var timeoutMs = 300;

            var consumer = Mock.Of<IConsumer<NotUsed, NotUsed>>(
                c => c.QueryWatermarkOffsets(new TopicPartition(topic, 0), TimeSpan.FromMilliseconds(timeoutMs))
                                            == new WatermarkOffsets(new Offset(0), new Offset(1))
                    && c.QueryWatermarkOffsets(new TopicPartition(topic, 1), TimeSpan.FromMilliseconds(timeoutMs))
                                            == new WatermarkOffsets(new Offset(100), new Offset(1000))
                                            );

            // act
            var topicPartitionOffsets = KafkaConsumerExtensions.LastMessagePartitionOffsets(consumer, topic, topicPartitionCount, timeoutMs);

            // assert
            Assert.Equal(2, topicPartitionOffsets.Length);

            Assert.Equal(topic, topicPartitionOffsets[0].Topic);
            Assert.Equal(0, topicPartitionOffsets[0].Partition.Value);
            Assert.Equal(0, topicPartitionOffsets[0].Offset.Value);

            Assert.Equal(topic, topicPartitionOffsets[1].Topic);
            Assert.Equal(1, topicPartitionOffsets[1].Partition.Value);
            Assert.Equal(999, topicPartitionOffsets[1].Offset.Value);
        }
    }
}
