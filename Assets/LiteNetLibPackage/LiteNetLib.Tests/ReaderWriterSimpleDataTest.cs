using LiteNetLib.Utils;

using NUnit.Framework;
using System.Net;

namespace LiteNetLib.Tests
{
    [TestFixture]
    [Category("DataReaderWriter")]
    public class ReaderWriterSimpleDataTest
    {
        [Test]
        public void WriteReadBool()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put(true);

            NetDataReader ndr = new NetDataReader(ndw);
            bool readBool = ndr.GetBool();

            Assert.That(readBool, Is.True);
        }

        [Test]
        public void WriteReadBoolArray()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.PutArray(new[] {true, false, true, false, false});

            NetDataReader ndr = new NetDataReader(ndw);
            bool[] readBoolArray = ndr.GetBoolArray();

            Assert.That(new[] {true, false, true, false, false}, Is.EqualTo(readBoolArray).AsCollection);
        }

        [Test]
        public void WriteReadByte()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put((byte) 8);

            NetDataReader ndr = new NetDataReader(ndw);
            byte readByte = ndr.GetByte();

            Assert.That(readByte, Is.EqualTo((byte) 8));
        }

        [Test]
        public void WriteReadByteArray()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put(new byte[] {1, 2, 4, 8, 16, byte.MaxValue, byte.MinValue});

            NetDataReader ndr = new NetDataReader(ndw);
            byte[] readByteArray = new byte[7];
            ndr.GetBytes(readByteArray, 7);

            Assert.That(
                new byte[] {1, 2, 4, 8, 16, byte.MaxValue, byte.MinValue},
                Is.EqualTo(readByteArray).AsCollection);
        }

#if NET5_0_OR_GREATER
        [Test]
        public void WriteReadByteSpan()
        {
            Span<byte> tempBytes = new byte[] { 1, 2, 4, 8 };
            var ndw = new NetDataWriter();
            ndw.Put(tempBytes);
            Span<byte> anotherTempBytes = new byte[] { 16, byte.MaxValue, byte.MinValue };
            ndw.Put(anotherTempBytes);

            var ndr = new NetDataReader(ndw);
            var readByteArray = new byte[7];
            ndr.GetBytes(readByteArray, 7);

            Assert.That(
                new byte[] { 1, 2, 4, 8, 16, byte.MaxValue, byte.MinValue },
                Is.EqualTo(readByteArray).AsCollection);
        }
#endif

        [Test]
        public void WriteReadDouble()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put(3.1415);

            NetDataReader ndr = new NetDataReader(ndw);
            double readDouble = ndr.GetDouble();

            Assert.That(readDouble, Is.EqualTo(3.1415));
        }

        [Test]
        public void WriteReadDoubleArray()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.PutArray(new[] {1.1, 2.2, 3.3, 4.4, double.MaxValue, double.MinValue});

            NetDataReader ndr = new NetDataReader(ndw);
            double[] readDoubleArray = ndr.GetDoubleArray();

            Assert.That(
                new[] {1.1, 2.2, 3.3, 4.4, double.MaxValue, double.MinValue},
                Is.EqualTo(readDoubleArray).AsCollection);
        }

        [Test]
        public void WriteReadFloat()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put(3.1415f);

            NetDataReader ndr = new NetDataReader(ndw);
            float readFloat = ndr.GetFloat();

            Assert.That(readFloat, Is.EqualTo(3.1415f));
        }

        [Test]
        public void WriteReadFloatArray()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.PutArray(new[] {1.1f, 2.2f, 3.3f, 4.4f, float.MaxValue, float.MinValue});

            NetDataReader ndr = new NetDataReader(ndw);
            float[] readFloatArray = ndr.GetFloatArray();

            Assert.That(
                new[] {1.1f, 2.2f, 3.3f, 4.4f, float.MaxValue, float.MinValue},
                Is.EqualTo(readFloatArray).AsCollection);
        }

        [Test]
        public void WriteReadInt()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put(32);

            NetDataReader ndr = new NetDataReader(ndw);
            int readInt = ndr.GetInt();

            Assert.That(readInt, Is.EqualTo(32));
        }

        [Test]
        public void WriteReadIntArray()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.PutArray(new[] {1, 2, 3, 4, 5, 6, 7, int.MaxValue, int.MinValue});

            NetDataReader ndr = new NetDataReader(ndw);
            int[] readIntArray = ndr.GetIntArray();

            Assert.That(new[] {1, 2, 3, 4, 5, 6, 7, int.MaxValue, int.MinValue}, Is.EqualTo(readIntArray).AsCollection);
        }

        [Test]
        public void WriteReadLong()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put(64L);

            NetDataReader ndr = new NetDataReader(ndw);
            long readLong = ndr.GetLong();

            Assert.That(readLong, Is.EqualTo(64L));
        }

        [Test]
        public void WriteReadLongArray()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.PutArray(new[] {1L, 2L, 3L, 4L, long.MaxValue, long.MinValue});

            NetDataReader ndr = new NetDataReader(ndw);
            long[] readLongArray = ndr.GetLongArray();

            Assert.That(new[] {1L, 2L, 3L, 4L, long.MaxValue, long.MinValue}, Is.EqualTo(readLongArray).AsCollection);
        }

        [Test]
        public void WriteReadNetEndPoint()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put(NetUtils.MakeEndPoint("127.0.0.1", 7777));

            NetDataReader ndr = new NetDataReader(ndw);
            IPEndPoint readNetEndPoint = ndr.GetIPEndPoint();

            Assert.That(readNetEndPoint, Is.EqualTo(NetUtils.MakeEndPoint("127.0.0.1", 7777)));
        }

        [Test]
        public void WriteReadSByte()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put((sbyte) 8);

            NetDataReader ndr = new NetDataReader(ndw);
            sbyte readSByte = ndr.GetSByte();

            Assert.That(readSByte, Is.EqualTo((sbyte) 8));
        }

        [Test]
        public void WriteReadShort()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put((short) 16);

            NetDataReader ndr = new NetDataReader(ndw);
            short readShort = ndr.GetShort();

            Assert.That(readShort, Is.EqualTo((short) 16));
        }

        [Test]
        public void WriteReadShortArray()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.PutArray(new short[] {1, 2, 3, 4, 5, 6, short.MaxValue, short.MinValue});

            NetDataReader ndr = new NetDataReader(ndw);
            short[] readShortArray = ndr.GetShortArray();

            Assert.That(
                new short[] {1, 2, 3, 4, 5, 6, short.MaxValue, short.MinValue},
                Is.EqualTo(readShortArray).AsCollection);
        }

        [Test]
        public void WriteReadString()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put("String", 10);

            NetDataReader ndr = new NetDataReader(ndw);
            string readString = ndr.GetString(10);

            Assert.That(readString, Is.EqualTo("String"));
        }

        [Test]
        public void WriteReadStringArray()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.PutArray(new[] {"First", "Second", "Third", "Fourth"});

            NetDataReader ndr = new NetDataReader(ndw);
            string[] readStringArray = ndr.GetStringArray(10);

            Assert.That(new[] {"First", "Second", "Third", "Fourth"}, Is.EqualTo(readStringArray).AsCollection);
        }

        [Test]
        public void WriteReadUInt()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put(34U);

            NetDataReader ndr = new NetDataReader(ndw);
            uint readUInt = ndr.GetUInt();

            Assert.That(readUInt, Is.EqualTo(34U));
        }

        [Test]
        public void WriteReadUIntArray()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.PutArray(new[] {1U, 2U, 3U, 4U, 5U, 6U, uint.MaxValue, uint.MinValue});

            NetDataReader ndr = new NetDataReader(ndw);
            uint[] readUIntArray = ndr.GetUIntArray();

            Assert.That(
                new[] {1U, 2U, 3U, 4U, 5U, 6U, uint.MaxValue, uint.MinValue},
                Is.EqualTo(readUIntArray).AsCollection);
        }

        [Test]
        public void WriteReadULong()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put(64UL);

            NetDataReader ndr = new NetDataReader(ndw);
            ulong readULong = ndr.GetULong();

            Assert.That(readULong, Is.EqualTo(64UL));
        }

        [Test]
        public void WriteReadULongArray()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.PutArray(new[] {1UL, 2UL, 3UL, 4UL, 5UL, ulong.MaxValue, ulong.MinValue});

            NetDataReader ndr = new NetDataReader(ndw);
            ulong[] readULongArray = ndr.GetULongArray();

            Assert.That(
                new[] {1UL, 2UL, 3UL, 4UL, 5UL, ulong.MaxValue, ulong.MinValue},
                Is.EqualTo(readULongArray).AsCollection);
        }

        [Test]
        public void WriteReadUShort()
        {
            NetDataWriter ndw = new NetDataWriter();
            ndw.Put((ushort) 16);

            NetDataReader ndr = new NetDataReader(ndw);
            ushort readUShort = ndr.GetUShort();

            Assert.That(readUShort, Is.EqualTo((ushort) 16));
        }

        [Test]
        public void WriteReadIPEndPoint()
        {
            NetDataWriter ndw = new NetDataWriter();
            IPEndPoint ipep = new IPEndPoint(IPAddress.Broadcast, 12345);
            IPEndPoint ipep6 = new IPEndPoint(IPAddress.IPv6Loopback, 12345);
            ndw.Put(ipep);
            ndw.Put(ipep6);

            NetDataReader ndr = new NetDataReader(ndw);
            IPEndPoint readIpep = ndr.GetIPEndPoint();
            IPEndPoint readIpep6 = ndr.GetIPEndPoint();

            Assert.That(readIpep, Is.EqualTo(ipep));
            Assert.That(readIpep6, Is.EqualTo(ipep6));
            Assert.That(ndr.AvailableBytes, Is.EqualTo(0));
        }
    }
}
