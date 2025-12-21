using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using EnvironmentsService.Application.Interfaces;

namespace EnvironmentsService.Infrastructure.Messaging.Producers
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
        {
            _logger = logger;

            // Configuration du producer Kafka
            var config = new ProducerConfig
            {
                // Adresse du serveur Kafka
                BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",

                // En cas d'erreur, essayer de renvoyer
                MessageSendMaxRetries = 3,

                // Attendre que le message soit bien reçu
                Acks = Acks.All
            };

            // Créer le producer
            // <string, string> = clé et valeur sont des strings
            _producer = new ProducerBuilder<string, string>(config).Build();

            _logger.LogInformation("Kafka Producer créé avec succès");
        }

        public async Task ProduceAsync<T>(string topic, T message) where T : class
        {
            try
            {
                // 1. Sérialiser l'objet en JSON
                var jsonMessage = JsonSerializer.Serialize(message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });

                // 2. Créer le message Kafka
                var kafkaMessage = new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),  // Clé unique pour le partitionnement
                    Value = jsonMessage,              // Le JSON de l'événement
                    Timestamp = Timestamp.Default     // Timestamp actuel
                };

                // 3. Envoyer le message sur Kafka
                var result = await _producer.ProduceAsync(topic, kafkaMessage);

                // 4. Logger le succès
                _logger.LogInformation(
                    "Message publié sur Kafka - Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                    topic,
                    result.Partition.Value,
                    result.Offset.Value
                );
            }
            catch (ProduceException<string, string> ex)
            {
                // Erreur lors de l'envoi
                _logger.LogError(ex, "Erreur lors de la publication sur Kafka - Topic: {Topic}", topic);
                throw;
            }
            catch (Exception ex)
            {
                // Autre erreur
                _logger.LogError(ex, "Erreur inattendue lors de la publication sur Kafka");
                throw;
            }
        }

        // Libérer les ressources quand le producer n'est plus utilisé
        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
            _logger.LogInformation("Kafka Producer disposé");
        }
    }
}