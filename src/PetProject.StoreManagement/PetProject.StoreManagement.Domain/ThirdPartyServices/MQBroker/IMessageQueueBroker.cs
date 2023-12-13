namespace PetProject.StoreManagement.Domain.ThirdPartyServices.MQBroker
{
    public interface IMessageQueueBroker
    {
        void SendMessage<T>(T data);

        void SendMessage<T>(T data, string queueName);

        void ReceiveMessage(string queueName, Action<object?, string> action);
    }
}
