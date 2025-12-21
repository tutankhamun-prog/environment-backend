namespace EnvironmentsService.Application.Interfaces
{
    public interface IKafkaProducer
    {
        // Publier un événement sur un topic Kafka
        // topic = le "canal" Kafka (comme un sujet d'email)
        // message = l'objet à envoyer (sera sérialisé en JSON)
        Task ProduceAsync<T>(string topic, T message) where T : class;
    }
}