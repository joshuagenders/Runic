using Runic.Agent.Core.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public class Runner<T>
    {
        private readonly IProducer<T> _producer;
        private readonly IConsumer<T> _consumer;
        private readonly TaskFactory _taskFactory;

        public Runner(IProducer<T> producer, IConsumer<T> consumer, ICoreConfiguration config)
        {
            _taskFactory = new TaskFactory();
            _producer = producer;
            _consumer = consumer;
        }

        public async Task Start(CancellationToken ctx)
        {
            var tasks = new [] {
                _producer.ProduceWorkItems(ctx)
                         .ContinueWith((_) => _consumer.Close()),
                _taskFactory.StartNew(() => Monitor(ctx))
            };
            await Task.WhenAll(tasks);
        }

        public async Task Monitor(CancellationToken ctx)
        {
            while (!ctx.IsCancellationRequested && !_consumer.Closed)
            {
                if (_consumer.Count > 0)
                    await _taskFactory.StartNew(() => _consumer.ProcessQueue(ctx));
            }
        }
    }
}
