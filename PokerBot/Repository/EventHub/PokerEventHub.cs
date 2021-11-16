using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;

namespace PokerBot.Repository.EventHub {
    public class PokerEventHub : IPokerEventHub {
        private ISecrets _secrets;
        private readonly ILogger<PokerEventHub> _logger;
        static EventHubProducerClient producerClient;
        public PokerEventHub(ISecrets secrets, ILogger<PokerEventHub> logger) {
            _secrets = secrets;
            _logger = logger;
        }
        public async void SendEvent(string message) {
            producerClient = new EventHubProducerClient(_secrets.EventHubConnectionString(), _secrets.EventHubName());
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
            if(!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(message)))) {
                throw new Exception("Event is too large for the batch and cannot be sent.");
            }
            try {
                await producerClient.SendAsync(eventBatch);
                _logger.LogInformation("A batch of events has been published");
            }
            finally {
                await producerClient.DisposeAsync();
            }
        }
    }
}