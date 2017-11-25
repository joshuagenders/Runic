using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.Patterns;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.WorkGenerator;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.AssemblyManagement;

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
            var work = new Work(
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
            var mockConsumer = new Mock<IConsumer<Work>>();
            var mockDatetime = new Mock<IDatetimeService>();
            var mockConfig = new Mock<ICoreConfiguration>();

            mockConfig.Setup(m => m.TaskCreationPollingIntervalSeconds).Returns(2);
            var producer = new WorkProducer(mockConsumer.Object, mockDatetime.Object, mockConfig.Object);

            mockDatetime.Setup(d => d.Now).Returns(time);
            var cts = new CancellationTokenSource();
            
            producer.AddUpdateWorkItem("test", work);
            producer.PopulateWorkQueue(cts.Token);

            mockDatetime.Setup(d => d.Now).Returns(time.AddMinutes(1).AddSeconds(1));
            producer.PopulateWorkQueue(cts.Token);

            mockConsumer.Verify(c => c.EnqueueTask(work), Times.Exactly((int)work.JourneysPerMinute));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenCancelled_ThenProductionStops()
        {
            var work = new Work(
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
            var mockConsumer = new Mock<IConsumer<Work>>();
            var mockDatetime = new Mock<IDatetimeService>();
            var mockConfig = new Mock<ICoreConfiguration>();

            mockConfig.Setup(m => m.TaskCreationPollingIntervalSeconds).Returns(0);
            var producer = new WorkProducer(mockConsumer.Object, mockDatetime.Object, mockConfig.Object);
            //this return is required otherwise it runs sync and blocks the test thread
            mockDatetime.Setup(d => d.WaitMilliseconds(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(async () => await Task.Delay(50));
            var cts = new CancellationTokenSource();
            producer.AddUpdateWorkItem("test", work);
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
            var workQueue = new ConcurrentQueue<Work>();
            var mockDts = new Mock<IDatetimeService>();
            var consumer = new WorkConsumer(workQueue, mockDts.Object, mockPersonFactory.Object);
            var mockPerson = new Mock<IPerson>();
            var cts = new CancellationTokenSource();

            mockPersonFactory.Setup(p => p.GetPerson(It.IsAny<Journey>())).Returns(mockPerson.Object);

            var work = new Work(
                new Journey()
                {
                    Name = "Test Journey"
                })
            {
                JourneysPerMinute = 1
            };

            cts.CancelAfter(1000);
            consumer.EnqueueTask(work);

            await consumer.ProcessCallback(new WorkContext()
            {
                Ctx = cts.Token,
                Work = work
            });
            mockPerson.Verify(p => p.PerformJourneyAsync(work.Journey, cts.Token));
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public void WhenMultipleItemsAreQueued_ThenConsumerConsumesMultipleItems()
        {
            var mockPersonFactory = new Mock<IPersonFactory>();
            var workQueue = new ConcurrentQueue<Work>();
            var mockDts = new Mock<IDatetimeService>();
            var consumer = new WorkConsumer(workQueue, mockDts.Object, mockPersonFactory.Object);
            var mockPerson = new Mock<IPerson>();
            var cts = new CancellationTokenSource();

            mockPersonFactory.Setup(p => p.GetPerson(It.IsAny<Journey>())).Returns(mockPerson.Object);

            var work = new Work(
                new Journey()
                {
                    Name = "Test Journey"
                })
            {
                JourneysPerMinute = 1
            };

            cts.CancelAfter(1500);
            consumer.EnqueueTask(work);
            consumer.EnqueueTask(work);
            consumer.ProcessQueue(cts.Token);
            Thread.Sleep(1250);
            mockPerson.Verify(p => p.PerformJourneyAsync(work.Journey, cts.Token), Times.Exactly(2));
        }

        public void ProducerProducesItemsForMultipleTestPlans()
        {
            // TODO
        }
    }
}
