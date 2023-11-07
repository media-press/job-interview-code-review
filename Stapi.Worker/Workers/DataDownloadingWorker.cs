using RabbitMQ.Client;
using Stapi.Worker.Logger;
using Stapi.Worker.Mappings;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Stapi.Worker.Workers;

internal class DataDownloadingWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested == false)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            await Parallel.ForEachAsync(ApiDataMapping.DataMapping, new ParallelOptions
            {
                MaxDegreeOfParallelism = 4
            }, async (data, cancellationToken) =>
            {
                var (key, value) = data;

                CustomLogger.Log("Information", "Processing " + key + " started");

                channel.QueueDeclare(queue: $"data-download-{key}", durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.ExchangeDeclare("data-download", ExchangeType.Topic, true, false, null);
                channel.QueueBind($"data-download-{key}", "data-download", key);

                var pageSize = 10;
                var page = 0;
                var isLastPage = false;

                do
                {
                    var httpClient = new HttpClient(new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                        AllowAutoRedirect = false
                    });

                    var result = await httpClient.GetAsync($"https://stapi.co/api/v1/rest/{key}/search?pageSize={pageSize}&page={page++}", cancellationToken);
                    var body = await result.Content.ReadAsStringAsync(cancellationToken);

                    var jsonObject = JsonSerializer.Deserialize<JsonObject>(body);
                    if (jsonObject is null)
                    {
                        return;
                    }

                    if (jsonObject.ContainsKey("page"))
                    {
                        isLastPage = jsonObject["page"]!["lastPage"]!.GetValue<bool>();
                    }

                    if (jsonObject.ContainsKey(value))
                    {
                        foreach (var jsonNode in jsonObject[value]!.AsArray())
                        {
                            var jsonString = jsonNode!.ToJsonString();

                            channel.BasicPublish(exchange: "data-download",
                                routingKey: key,
                                basicProperties: null,
                            body: Encoding.UTF8.GetBytes(jsonString));

                            CustomLogger.Log("Information", "Message " + key + " published " + jsonString);
                        }
                    }

                    CustomLogger.Log("Information", "Processing " + key + " finished");
                } while (isLastPage == false);
            });

            await Task.Delay(1000, stoppingToken);
        }
    }
}