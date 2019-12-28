using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Akka.Streams.Kafka.Utils.Headers;

namespace Akka.Streams.Kafka.Utils.Tests.Headers
{
    public class KafkaSourceHeaderExtensionsTests
    {
        [Fact]
        public void AddGetSourceHeaderNoHeaders()
        {
            // arrange
            const int partition = 1;
            const int offset = 777;

            Message<NotUsed, NotUsed> message = new Message<NotUsed, NotUsed>();
            message.Headers = null;

            // act
            message.AddSourceHeader(partition, offset);
            var setted = message.GetSourceHeader<NotUsed, NotUsed>();

            // assert
            Assert.NotNull(setted);
            Assert.Equal(partition, setted.Value.partition);
            Assert.Equal(offset, setted.Value.offset);
        }

        [Fact]
        public void AddGetSourceHeaderEmptyHeaders()
        {
            // arrange
            const int partition = 0;
            const int offset = 0;

            Message<NotUsed, NotUsed> message = new Message<NotUsed, NotUsed>();
            message.Headers = new Confluent.Kafka.Headers();

            // act
            message.AddSourceHeader(partition, offset);
            var setted = message.GetSourceHeader<NotUsed, NotUsed>();

            // assert
            Assert.NotNull(setted);
            Assert.Equal(partition, setted.Value.partition);
            Assert.Equal(offset, setted.Value.offset);
        }

        [Fact]
        public void AddGetSourceHeaderExistingHeaders()
        {
            // arrange
            const int partition = 0;
            const int offset = 0;
            var existingHeader = new Header("ExistingHeader", new byte[] { 0b00000000, 0b00000001 });

            Message<NotUsed, NotUsed> message = new Message<NotUsed, NotUsed>();
            message.Headers = new Confluent.Kafka.Headers();
            message.Headers.Add(existingHeader);

            // act
            message.AddSourceHeader(partition, offset);
            var setted = message.GetSourceHeader<NotUsed, NotUsed>();

            // assert
            Assert.NotNull(setted);
            Assert.Equal(partition, setted.Value.partition);
            Assert.Equal(offset, setted.Value.offset);
            Assert.Contains(message.Headers, h => h == existingHeader);
        }

        [Fact]
        public void AddGetSourceHeaderKeepsFirstHeaders()
        {
            // arrange
            const int partition = 7;
            const int offset = 100000;
            var existingHeader = new Header("ExistingHeader", new byte[] { 0b00000000, 0b00000001 });

            Message<NotUsed, NotUsed> message = new Message<NotUsed, NotUsed>();
            message.Headers = new Confluent.Kafka.Headers();
            message.Headers.Add(existingHeader);

            // act
            message.AddSourceHeader(partition, offset);
            message.AddSourceHeader(1, 100);

            var setted = message.GetSourceHeader<NotUsed, NotUsed>();

            // assert
            Assert.NotNull(setted);
            Assert.Equal(partition, setted.Value.partition);
            Assert.Equal(offset, setted.Value.offset);
            Assert.Contains(message.Headers, h => h == existingHeader);
        }
    }
}
