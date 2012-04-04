using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using AppUpdater.Utils;

namespace AppUpdater.Tests.Utils
{
    [TestFixture]
    public class ChecksumTests
    {
        [Test]
        public void Calculate_CreatesTheChecksum()
        {
            byte[] data = Encoding.UTF8.GetBytes("some text");
            MemoryStream stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;

            string checksum = Checksum.Calculate(stream);

            Assert.That(checksum, Is.EqualTo("B94F6F125C79E3A5FFAA826F584C10D52ADA669E6762051B826B55776D05AED2"));
        }
    }
}
