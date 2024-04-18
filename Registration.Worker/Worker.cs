using Amazon.SQS;
using Amazon.SQS.Model;
using Dapper;
using Newtonsoft.Json;
using Registration.Data.Entities;
using Registration.Data.Helpers;

namespace Registration.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public AmazonSQSClient clientSqs = new("AKIAQBIF3N6OSP63DWEK", "RuTynoLIPansPutnRLzOgPsZQdZ0EmCcFhJHQd1G");

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DapperHelper objDapperHelper = new DapperHelper($"Server=registration.cray2kmcoto4.us-east-1.rds.amazonaws.com;Database=Registration;User Id=admin;Password=UQEoR8iJ;");


            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var message = await clientSqs.ReceiveMessageAsync("https://sqs.us-east-1.amazonaws.com/002696638365/EventLogQueue");
                if (message.Messages.Any())
                {

                    foreach (var item in message.Messages)
                    {
                        var even = JsonConvert.DeserializeObject<RegistrationGetResponse>(item.Body);
                        DynamicParameters b = new DynamicParameters();
                        b.Add("pEventLogId", even.EventLogId);
                        b.Add("pEventType", even.EventType);
                        b.Add("pEventDate", even.EventDate);
                        b.Add("pEventDescription", even.EventDescription);

                        int result = objDapperHelper.ExecuteSingleRow<int>("spPostEventLogs", b);
                    }
                    await DeleteMessage(message.Messages);
                } 
                await Task.Delay(1000, stoppingToken);
            }
        }
        public async Task DeleteMessage(List<Message> messages)
        {
            foreach (var message in messages)
            {
                await clientSqs.DeleteMessageAsync("https://sqs.us-east-1.amazonaws.com/002696638365/EventLogQueue", message.ReceiptHandle);
            }
        }
    }
}