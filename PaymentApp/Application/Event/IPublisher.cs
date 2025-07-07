// File: Application/Common/Events/IPublisher.cs
namespace Application.Common.Events
{
    public interface IPublisher
    {
        Task PublishAsync<T>(T @event) where T : class;
    }
}
