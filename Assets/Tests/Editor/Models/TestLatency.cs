using System;
using Models;
using NUnit.Framework;
using Zenject;

namespace Tests.Editor.Models
{
    [TestFixture]
    public class TestLatency : ZenjectUnitTestFixture
    {
        [Test]
        public void TestLagGivenZero()
        {
            var l = new Latency(0, 0);
            Assert.AreEqual(l.Lag, 0);
        }

        [Test]
        public void TestRoundTripGivenZero()
        {
            var l = new Latency(0, 0);
            Assert.AreEqual(l.RoundTrip, 0);
        }
        
        [Test]
        public void TestThroughputGivenZero()
        {
            var l = new Latency(0, 0);
            Assert.AreEqual(l.Throughput, 0);
        }

        [Test]
        public void TestLagGiven100MsOffset()
        {
            const int ping = 1;
            const int pong = ping + 100;
            
            var l = new Latency(ping, pong);
            Assert.AreEqual(l.Lag, pong - ping);
        }
    }
}