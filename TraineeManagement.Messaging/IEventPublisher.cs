namespace TraineeManagement.Messaging;

public interface IEventPublisher
{
    // A Generic method for the creating publish service
    // message -> data
    // routingKey -> name of the queue
    // cancellation token -> to handle the mid way network work or timeout errors
    Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default)
        where T : class;
}