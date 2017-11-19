using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.Patterns;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.UnitTest.Tests
{
    [TestClass]
    public class ProducerConsumerTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenProducerProducesForAMinute_ThenCorrectAmountIsProduced()
        {
            var time = new DateTime(2017, 01, 23, 13, 00, 00);
            var testPlan = new TestPlan(
                new Journey()
                {
                    Name = "Test journey"
                }
            )
            {
                JourneysPerMinute = 20,
                Frequency = new ConstantFrequencyPattern()
                {
                    DurationSeconds = 120,
                    JourneysPerMinute = 20
                }
            };
            //
            var mockConsumer = new Mock<IConsumer<TestPlan>>();
            var mockDatetime = new Mock<IDatetimeService>();
            var mockConfig = new Mock<ICoreConfiguration>();

            mockConfig.Setup(m => m.TaskCreationPollingIntervalSeconds).Returns(2);
            var producer = new TestPlanProducer(mockConsumer.Object, mockDatetime.Object, mockConfig.Object);

            mockDatetime.Setup(d => d.Now).Returns(time);
            var cts = new CancellationTokenSource();
            
            producer.AddUpdateWorkItem("test", testPlan);
            producer.PopulateWorkQueue(cts.Token);

            mockDatetime.Setup(d => d.Now).Returns(time.AddMinutes(1).AddSeconds(1));
            producer.PopulateWorkQueue(cts.Token);

            mockConsumer.Verify(c => c.EnqueueTask(testPlan), Times.Exactly((int)testPlan.JourneysPerMinute));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenCancelled_ThenProductionStops()
        {
            var testPlan = new TestPlan(
                new Journey()
                {
                    Name = "Test journey"
                }
            )
            {
                JourneysPerMinute = 20,
                Frequency = new ConstantFrequencyPattern()
                {
                    DurationSeconds = 120,
                    JourneysPerMinute = 20
                }
            };
            //
            var mockConsumer = new Mock<IConsumer<TestPlan>>();
            var mockDatetime = new Mock<IDatetimeService>();
            var mockConfig = new Mock<ICoreConfiguration>();

            mockConfig.Setup(m => m.TaskCreationPollingIntervalSeconds).Returns(0);
            var producer = new TestPlanProducer(mockConsumer.Object, mockDatetime.Object, mockConfig.Object);
            //this return is required otherwise it runs sync and blocks the test thread
            mockDatetime.Setup(d => d.WaitMilliseconds(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(async () => await Task.Delay(50));
            var cts = new CancellationTokenSource();
            producer.AddUpdateWorkItem("test", testPlan);
            var producerTask = producer.ProduceWorkItems(cts.Token);
            try
            {
                cts.Cancel();
                await producerTask;
            }
            catch (TaskCanceledException)
            {
                //allg 
            }
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenProcessCallBackIsCalled_ThenJourneyIsPerformed()
        {
            var mockPersonFactory = new Mock<IPersonFactory>();
            var workQueue = new ConcurrentQueue<TestPlan>();
            var mockDts = new Mock<IDatetimeService>();
            var consumer = new TestPlanConsumer(workQueue, mockPersonFactory.Object, mockDts.Object);
            var mockPerson = new Mock<IPerson>();
            var cts = new CancellationTokenSource();

            mockPersonFactory.Setup(p => p.GetPerson(It.IsAny<Journey>())).Returns(mockPerson.Object);
            
            var testPlan = new TestPlan(
                new Journey()
                {
                    Name = "Test Journey"
                })
            {
                JourneysPerMinute = 1
            };

            cts.CancelAfter(1000);
            consumer.EnqueueTask(testPlan);

            await consumer.ProcessCallback(new TestPlanContext()
            {
                Ctx = cts.Token,
                TestPlan = testPlan
            });
            mockPerson.Verify(p => p.PerformJourneyAsync(testPlan.Journey, cts.Token));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenMultipleItemsAreQueued_ThenConsumerConsumesMultipleItems()
        {
            var mockPersonFactory = new Mock<IPersonFactory>();
            var workQueue = new ConcurrentQueue<TestPlan>();
            var mockDts = new Mock<IDatetimeService>();
            var consumer = new TestPlanConsumer(workQueue, mockPersonFactory.Object, mockDts.Object);
            var mockPerson = new Mock<IPerson>();
            var cts = new CancellationTokenSource();
            
            mockPersonFactory.Setup(p => p.GetPerson(It.IsAny<Journey>())).Returns(mockPerson.Object);

            var testPlan = new TestPlan(
                new Journey()
                {
                    Name = "Test Journey"
                })
            {
                JourneysPerMinute = 1
            };

            cts.CancelAfter(1500);
            consumer.EnqueueTask(testPlan);
            consumer.EnqueueTask(testPlan);
            consumer.ProcessQueue(cts.Token);
            Thread.Sleep(1250);
            mockPerson.Verify(p => p.PerformJourneyAsync(testPlan.Journey, cts.Token), Times.Exactly(2));
        }

        public void ProducerProducesItemsForMultipleTestPlans()
        {
            
        }
    }
}
