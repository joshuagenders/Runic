namespace Runic.Agent.FunctionalTest.TestUtility
{
    public class FakeTest
    {
        private static readonly object locker = new object();
        public static int GoogleSomethingCounter { get; private set; }

        public void GoogleSomething(string googleSomething = "default")
        {
            lock (locker)
            {
                GoogleSomethingCounter++;
            }
        }
    }
}
