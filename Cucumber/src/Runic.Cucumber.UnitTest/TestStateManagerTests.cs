using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runic.Cucumber.UnitTest
{
    class DummyObj
    {
        public string State { get; set; }
    }

    [TestClass]
    public class TestStateManagerTests
    {
        [TestMethod]
        public void TestStoreAndRetrieve()
        {
            TestStateManager manager = new TestStateManager();
            var obj = new DummyObj() { State = "test" };
            manager.AddObject(obj);
            var returnedObj = manager.GetObject<DummyObj>();
            returnedObj.Should().Be(obj);
            returnedObj.State.Should().Be("test");
        }

        [TestMethod]
        public void TestCreateAndRetrieve()
        {
            TestStateManager manager = new TestStateManager();
            var obj = manager.GetObject<DummyObj>();
            obj.Should().NotBeNull();
            obj.State.Should().BeNull();
        }

        [TestMethod]
        public void TestDuplicateStore()
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
    }
}
