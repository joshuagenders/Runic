using Runic.Orchestration;
using Xunit;

namespace Runic.UnitTest
{
    public class TestTimedAction
    {
        [Fact]
        public async void TestTimedActionResponse()
        {
            var result = await new TimedAction("someaction", () =>
            {
                return "result";
            }).Execute();
            Assert.Equal(result.ExecutionResult, "result");
            Assert.True(result.ElapsedMilliseconds > 0);
        }
    }
}
