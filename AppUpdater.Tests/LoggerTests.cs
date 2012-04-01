using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AppUpdater.Log;

namespace AppUpdater.Tests
{
    [TestFixture]
    public class LoggerTests
    {
        [SetUp]
        public void Setup()
        {
            Logger.LoggerProvider = (type) => new TestLog(type);
        }

        [Test]
        public void For_ReturnsTheLoggerForTheType()
        {
            ILog log = Logger.For(typeof(LoggerTests));

            Assert.That(log, Is.InstanceOf<TestLog>());
            Assert.That((log as TestLog).Type, Is.EqualTo(typeof(LoggerTests)));
        }

        [Test]
        public void ForT_ReturnsTheLoggerForTheType()
        {
            ILog log = Logger.For<LoggerTests>();

            Assert.That(log, Is.InstanceOf<TestLog>());
            Assert.That((log as TestLog).Type, Is.EqualTo(typeof(LoggerTests)));
        }

        [Test]
        public void For_WithAProviderThatReturnsAnError_ReturnsTheEmptyLog()
        {
            Logger.LoggerProvider = (type) => { throw new Exception("Error"); };

            ILog log = Logger.For(typeof(LoggerTests));

            Assert.That(log, Is.InstanceOf<EmptyLog>());
        }

        [Test]
        public void ForT_WithAProviderThatReturnsAnError_ReturnsTheEmptyLog()
        {
            Logger.LoggerProvider = (type) => { throw new Exception("Error"); };

            ILog log = Logger.For<LoggerTests>();

            Assert.That(log, Is.InstanceOf<EmptyLog>());
        }

        public class TestLog : EmptyLog
        {
            public Type Type { get; set; }

            public TestLog(Type type)
            {
                this.Type = type;
            }
        }
    }
}
