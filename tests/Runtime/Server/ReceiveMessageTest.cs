using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Mirror.SimpleWeb.Tests.Server
{
    [Category("SimpleWebTransport")]
    public class ReceiveMessageTest : SimpleWebTestBase
    {
        protected override bool StartServer => true;

        [UnityTest]
        public IEnumerator ReceiveArray()
        {
            // dont worry about result, run will timeout by itself
            _ = RunNode.RunAsync("SendMessages.js");

            yield return WaitForConnect;

            // wait for message
            yield return new WaitForSeconds(0.5f);
            const int messageSize = 10;

            Assert.That(onData, Has.Count.EqualTo(1), "Should have 1 message");

            (int connId, ArraySegment<byte> data) = onData[0];

            Assert.That(connId, Is.EqualTo(1), "Connd id should be 1");

            Assert.That(data.Count, Is.EqualTo(messageSize), "Shoudl have 10 bytes");
            for (int i = 0; i < messageSize; i++)
            {
                // js sends i+10 for each byte
                Assert.That(data.Array[data.Offset + i], Is.EqualTo(i + 10), "Data should match");
            }
        }

        [UnityTest]
        public IEnumerator ReceiveManyArrays()
        {
            // dont worry about result, run will timeout by itself
            _ = RunNode.RunAsync("SendManyMessages.js");

            yield return WaitForConnect;

            // wait for message
            yield return new WaitForSeconds(0.5f);
            const int expectedCount = 100;
            const int messageSize = 10;

            Assert.That(onData, Has.Count.EqualTo(expectedCount), $"Should have {expectedCount} message");

            for (int i = 0; i < expectedCount; i++)
            {
                (int connId, ArraySegment<byte> data) = onData[i];

                Assert.That(connId, Is.EqualTo(1), "Connd id should be 1");

                Assert.That(data.Count, Is.EqualTo(messageSize), "Should have 10 bytes");

                Assert.That(data.Array[data.Offset + 0], Is.EqualTo(i), "Data should match: first bytes should be send index");

                for (int j = 1; j < messageSize; j++)
                {
                    // js sends i+10 for each byte
                    Assert.That(data.Array[data.Offset + j], Is.EqualTo(j + 10), "Data should match");
                }
            }
        }


        [UnityTest]
        public IEnumerator ReceiveAlmostLargeArrays()
        {
            // dont worry about result, run will timeout by itself
            _ = RunNode.RunAsync("SendAlmostLargeMessages.js");

            yield return WaitForConnect;

            // wait for message
            yield return new WaitForSeconds(0.5f);
            const int messageSize = 10000;

            Assert.That(onData, Has.Count.EqualTo(1), $"Should have 1 message");

            (int connId, ArraySegment<byte> data) = onData[0];

            Assert.That(connId, Is.EqualTo(1), "Connd id should be 1");

            Assert.That(data.Count, Is.EqualTo(messageSize), "Should have 16384 bytes");
            for (int i = 0; i < messageSize; i++)
            {
                // js sends i%255 for each byte
                Assert.That(data.Array[data.Offset + i], Is.EqualTo(i % 255), "Data should match");
            }
        }

        [UnityTest]
        public IEnumerator ReceiveLargeArrays()
        {
            // dont worry about result, run will timeout by itself
            _ = RunNode.RunAsync("SendLargeMessages.js");

            yield return WaitForConnect;

            // wait for message
            yield return new WaitForSeconds(0.5f);
            const int messageSize = 16384;

            Assert.That(onData, Has.Count.EqualTo(1), $"Should have 1 message");

            (int connId, ArraySegment<byte> data) = onData[0];

            Assert.That(connId, Is.EqualTo(1), "Connd id should be 1");

            Assert.That(data.Count, Is.EqualTo(messageSize), "Should have 16384 bytes");
            for (int i = 0; i < messageSize; i++)
            {
                // js sends i%255 for each byte
                Assert.That(data.Array[data.Offset + i], Is.EqualTo(i % 255), "Data should match");
            }
        }

        [UnityTest]
        public IEnumerator ReceiveManyLargeArrays()
        {
            // dont worry about result, run will timeout by itself
            _ = RunNode.RunAsync("SendManyLargeMessages.js");

            yield return WaitForConnect;

            // wait for message
            yield return new WaitForSeconds(0.5f);
            const int expectedCount = 100;
            const int messageSize = 16384;

            Assert.That(onData, Has.Count.EqualTo(expectedCount), $"Should have {expectedCount} message");

            for (int i = 0; i < expectedCount; i++)
            {
                (int connId, ArraySegment<byte> data) = onData[i];

                Assert.That(connId, Is.EqualTo(1), "Connd id should be 1");

                Assert.That(data.Count, Is.EqualTo(messageSize), "Should have 10 bytes");

                Assert.That(data.Array[data.Offset + 0], Is.EqualTo(i), "Data should match: first bytes should be send index");

                for (int j = 1; j < messageSize; j++)
                {
                    // js sends i%255 for each byte
                    Assert.That(data.Array[data.Offset + j], Is.EqualTo(j % 255), "Data should match");
                }
            }
        }

        [UnityTest]
        public IEnumerator ReceiveTooLargeArrays()
        {
            // dont worry about result, run will timeout by itself
            _ = RunNode.RunAsync("SendTooLargeMessages.js");

            yield return WaitForConnect;

            // wait for message
            yield return new WaitForSeconds(0.5f);

            Assert.That(onData, Has.Count.EqualTo(0), $"Should have 0 message");

            Assert.That(onDisconnect, Has.Count.EqualTo(1), $"Should have 1 disconnect");
            Assert.That(onDisconnect[0], Is.EqualTo(1), $"connId should be 1");

            Assert.That(onError, Has.Count.EqualTo(1), $"Should have 1 error");
            Assert.That(onError[0].connId, Is.EqualTo(1), $"connId should be 1");
            Assert.That(onError[0].exception, Is.TypeOf<InvalidDataException>(), $"Should be InvalidDataException");
        }
    }
}