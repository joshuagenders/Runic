using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runic.Cucumber.UnitTest.Tests
{
    class DummyObj
    {
        public string State { get; set; }
    }

    [TestClass]
    public class TestStateManagerTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenStoringAndRetrieving_ReturnsSameObject()
        {
            TestStateManager manager = new TestStateManager();
            var obj = new DummyObj() { State = "test" };
            manager.AddObject(obj);
            var returnedObj = manager.GetObject<DummyObj>();
            returnedObj.Should().Be(obj);
            returnedObj.State.Should().Be("test");
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenCreateAndRetrieve_ReturnsNewObject()
        {
            TestStateManager manager = new TestStateManager();
            var obj = manager.GetObject<DummyObj>();
            obj.Should().NotBeNull();
            obj.State.Should().BeNull();
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenObjectIsStoredTwice_OverwritesObject()
        {
            TestStateManager manager = new TestStateManager();
            var obj = new DummyObj() { State = "test" };
            var obj2 = new DummyObj() { State = "test2" };
            manager.AddObject(obj);
            manager.AddObject(obj2);
            var returnedObj = manager.GetObject<DummyObj>();
            returnedObj.Should().Be(obj2);
            returnedObj.State.Should().Be("test2");
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenObjectIsRetrievedTwice_ReturnsSameObject()
        {
            TestStateManager manager = new TestStateManager();
            var obj = new DummyObj() { State = "test" };
            manager.AddObject(obj);
            var returnedObj1 = manager.GetObject<DummyObj>();
            var returnedObj2 = manager.GetObject<DummyObj>();

            returnedObj1.Should().Be(obj);
            returnedObj2.Should().Be(obj);
            returnedObj1.State.Should().Be("test");
            returnedObj2.State.Should().Be("test");
        }
    }
}
