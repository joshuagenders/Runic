using Moq;

namespace Runic.Agent.TestUtility
{
    public class TestObject<T> where T : class
    {
        private Mock<T> _mockObject { get; set; }
        public Mock<T> MockObject
        {
            get
            {
                if (_mockObject == null)
                    _mockObject = new Mock<T>();
                return _mockObject;
            }
            private set { _mockObject = value; }
        }
        public bool HasInstance { get; set; } = false;
        private T _instance;
        public T Instance
        {
            get
            {
                return _instance ?? MockObject.Object;
            }
            set
            {
                _instance = value;
                HasInstance = true;
            }
        }
    }
}
