using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Runic.Core.Query;
using Runic.Data;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace Runic.Clients
{
    public class RuneMessageClient : IRuneClient
    {
        public Task<List<Rune>> RetrieveRunes(params RuneQuery[] queries)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "query_runes",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(queries);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "query_runes",
                                     basicProperties: null,
                                     body: body);

                //poll

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var rBody = ea.Body;
                    var rMessage = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", rMessage);
                };
                
                return Task.Run(() =>
                {
                    bool noAck = false;
                    BasicGetResult result = channel.BasicGet("query_results", noAck);

                    channel.BasicConsume(queue: "query_results",
                                         noAck: true,
                                         consumer: consumer);
                    return JsonConvert.DeserializeObject<List<Rune>>(Encoding.UTF8.GetString(result.Body));
                });
            }

        }

        public void SendRunes(params Rune[] runes)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "mined_runes",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(runes);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "mined_runes",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
