using FluentAssertions;
using Runic.Agent.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Runic.Agent.Core.UnitTest.Tests
{
    public class DatetimeServiceTests
    {        
        [Fact]
        public async Task WhenWaitingUntil_DatetimeServiceWaits()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var service = new DateTimeService();
            var expectedFinish = DateTime.Now.AddSeconds(1);
            await service.WaitMilliseconds(1000, cts.Token);
            DateTime.Now.Should().BeOnOrAfter(expectedFinish);
        }
    }
}
