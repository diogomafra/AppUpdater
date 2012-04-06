using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AppUpdater.Utils;
using System.IO;

namespace AppUpdater.Tests.Utils
{
    [TestFixture]
    public class DataCompressorTests
    {
        [Test]
        public void Compress_ValidData_ReturnsSmallerData()
        {
            byte[] data = Encoding.UTF8.GetBytes(new string('a', 1000));

            byte[] compressedData = DataCompressor.Compress(data);

            Assert.That(compressedData.Length, Is.LessThan(data.Length));
        }

        [Test]
        public void Decompress_ValidData_ReturnsTheOriginalData()
        {
            byte[] originalData = Encoding.UTF8.GetBytes(new string('a', 1000));
            byte[] compressedData = DataCompressor.Compress(originalData);

            byte[] decompressedData = DataCompressor.Decompress(compressedData);

            Assert.That(decompressedData, Is.EqualTo(originalData));
        }

        [Test]
        public void Compress_NullData_ReturnsNullData()
        {
            byte[] data = null;

            byte[] compressedData = DataCompressor.Compress(data);

            Assert.That(compressedData, Is.Null);
        }

        [Test]
        public void Decompress_NullData_ReturnsNullData()
        {
            byte[] compressedData = null;

            byte[] decompressedData = DataCompressor.Decompress(compressedData);

            Assert.That(decompressedData, Is.Null);
        }
    }
}
