using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace Mirror.SimpleWeb.Tests
{
    [Category("SimpleWebTransport")]
    public class CheckNodeTest
    {
        /// <summary>
        /// This test will fail if SimpleWebTransport isnt in root of project
        /// </summary>
        [Test]
        public void FindFullPath()
        {
            string actual = RunNode.ResolvePath("HelloWorld.js");
            string expected = "./Assets/SimpleWebTransport/tests/node~/HelloWorld.js";

            Assert.That(Path.GetFullPath(actual), Is.EqualTo(Path.GetFullPath(expected)));
        }

        [Test]
        public void ShouldReturnHelloWorld()
        {
            RunNode.Result result = RunNode.Run("HelloWorld.js", false);

            Assert.That(result.timedOut, Is.False);

            Assert.That(result.output, Has.Length.EqualTo(1));
            Assert.That(result.output[0], Is.EqualTo("Hello World!"));

            Assert.That(result.error, Has.Length.EqualTo(0));
        }

        [Test]
        public void ShouldReturnHelloWorld2()
        {
            RunNode.Result result = RunNode.Run("HelloWorld2.js", false);

            Assert.That(result.timedOut, Is.False);

            Assert.That(result.output, Has.Length.EqualTo(2));
            Assert.That(result.output[0], Is.EqualTo("Hello World!"));
            Assert.That(result.output[1], Is.EqualTo("Hello again World!"));

            Assert.That(result.error, Has.Length.EqualTo(0));
        }

        [Test]
        public void ShouldReturnHelloError()
        {
            RunNode.Result result = RunNode.Run("HelloError.js", false);

            Assert.That(result.timedOut, Is.False);

            Assert.That(result.output, Has.Length.EqualTo(0));

            Assert.That(result.error, Has.Length.EqualTo(1));
            Assert.That(result.error[0], Is.EqualTo("Hello Error!"));
        }

        [Test]
        public void ShouldReturnHelloError2()
        {
            RunNode.Result result = RunNode.Run("HelloError2.js", false);

            Assert.That(result.timedOut, Is.False);

            Assert.That(result.output, Has.Length.EqualTo(0));

            Assert.That(result.error, Has.Length.EqualTo(2));
            Assert.That(result.error[0], Is.EqualTo("Hello Error!"));
            Assert.That(result.error[1], Is.EqualTo("Hello again Error!"));
        }

        [Test]
        public void ShouldFinishBeforeTimeout()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            RunNode.Result result = RunNode.Run("HelloWorld.js", false);

            stopwatch.Stop();
            double seconds = stopwatch.Elapsed.TotalSeconds;
            // hello script is fast and should finish faster than 1 second
            Assert.That(seconds, Is.LessThan(2.0));
        }

        [Test]
        public void ShouldStopAfterTimeout()
        {
            RunNode.Result result = RunNode.Run("Timeout.js", false);

            Assert.That(result.timedOut, Is.True);

            Assert.That(result.output, Has.Length.EqualTo(1));
            Assert.That(result.output[0], Is.EqualTo("Should be running"));

            Assert.That(result.error, Has.Length.EqualTo(0));
        }
    }
}