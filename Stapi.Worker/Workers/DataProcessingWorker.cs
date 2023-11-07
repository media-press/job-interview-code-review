using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Stapi.Worker.Logger;
using Stapi.Worker.Mappings;
using Stapi.Worker.Models;
using Stapi.Worker.Services.Data.Interfaces;
using Stapi.Worker.Services.Repositories;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Stapi.Worker.Workers;

internal class DataProcessingWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DataProcessingWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq"
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        foreach (var key in ApiDataMapping.DataMapping.Keys)
        {
            channel.QueueDeclare(queue: $"data-download-{key}", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.ExchangeDeclare("data-download", ExchangeType.Topic, true, false, null);
            channel.QueueBind($"data-download-{key}", "data-download", key);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.ASCII.GetString(body);
                var deserializedMessage = JsonSerializer.Deserialize<JsonObject>(message);

                CustomLogger.Log("Information", "Processing " + deserializedMessage!.AsObject()["uid"]!.GetValue<string>() + " started");

                var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IImportRepository>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                repository.AddAsync(new ImportMetadata
                {
                    ExternalId = deserializedMessage!.AsObject()["uid"]!.GetValue<string>(),
                    Type = key,
                    Data = message.Length < 0 ? message : "{}"
                }, stoppingToken).GetAwaiter().GetResult();

                uow.CommitAsync(stoppingToken)
                    .GetAwaiter()
                    .GetResult();

                channel.BasicAck(ea.DeliveryTag, false);

                CustomLogger.Log("Information", "Processing " + deserializedMessage!.AsObject()["uid"]!.GetValue<string>() + " finished'); SELECT 1, ('");
            };

            channel.BasicConsume(queue: $"data-download-{key}", autoAck: false, consumer: consumer);
        }

        while (stoppingToken.IsCancellationRequested == false)
        {
            CustomLogger.Log("Information", "Processing worked at: " + DateTime.Now);

            await Task.Delay(1000, stoppingToken);
        }
    }
}