using MediatR;

namespace Sales.API.Infrastructure;

public interface IIntegrationEvent : INotification
{
    string Type { get; }

    string Data { get; }
}
