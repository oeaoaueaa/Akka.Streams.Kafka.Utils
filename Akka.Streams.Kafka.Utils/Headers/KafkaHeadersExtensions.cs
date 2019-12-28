using Confluent.Kafka;
using System;
using System.Text;

namespace Akka.Streams.Kafka.Utils.Headers
{
    public static class KafkaHeadersExtensions
    { 
        public static byte[] GetHeaderBytes(int headerValue)
        {
            byte[] bytes = BitConverter.GetBytes(headerValue);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        public static byte[] GetHeaderBytes(long headerValue)
        {
            byte[] bytes = BitConverter.GetBytes(headerValue);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        public static byte[] GetHeaderBytes(string headerValue)
        {
            return Encoding.UTF8.GetBytes(headerValue);
        }

        public static int GetInt(this IHeader header)
        {
            var bytes = header.GetValueBytes();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

        public static long GetLong(this IHeader header)
        {
            var bytes = header.GetValueBytes();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt64(bytes, 0);
        }

        public static string GetString(this IHeader header)
        {
            var bytes = header.GetValueBytes();
            return Encoding.UTF8.GetString(bytes);
        }
    }

}
