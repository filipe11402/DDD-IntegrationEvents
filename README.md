## Integration Events demonstration project

This project will showcase how to implement integration events, on a DDD architecture based system

---

## Technologies used
- Docker
- RabbitMQ
- dotnet 6

## Patterns used
- Outbox pattern
- Repository/Unit Of Work
- CQRS

---

## Initial Notes
This project does not adhere to the best principles, as it is a **Demo** project to showcase integration events. 

But nonetheless, some of them they were followed, as it was needed to show how those principles would help to achieve the goal of this project.

To sum up, this project uses RabbitMQ, the implementation details of it, are out of this project scope, as advised, you should have some initial knowledge so that you can follow through

---

## Context
In our system, we will have 2 loosely coupled monoliths, on which it's base, it's an hospital.

With that said, the system will have the following requirements/constraints
- When creating a new patient(someone that has entered the hospital), it will equaly be a client, because of invoicing and billing at the end of his stay.
- A client is always a patient, and the same goes the other way around.

Our system will be divided into 2::
- [Hospital](src/Hospital.API/)
- [Sales](src/Sales.API/)

---

## What is an Integration Event
First of all, and event, means it's something that has already happened(past)
An Integration Event, is used, when we need to notify an outter boundary(or system) that something has happened, ence the event keyword.

With that said, this is really usefull do decouple our systems, because with this, we can make communication between services or boundaries, asyncronous!

Want to see more information regarding events?

**[Integration Events vs Domain Events](https://github.com/filipe11402/DDD-DomainEvents.git)**

---

## Implementing Integration Events in our system

So, as we see by the requirements mentioned above.
We Can conclude that the system that will trigger the **PatientCreated...** will be the the Hospital.

We can also see, that we will have 2 different Entities, `Patient` and `Client`.

---

## Publish the events with the Hospital system

To achieve this, we will use in our favor 2 patterns

**UnitOfWork**, which will help us use the **Commiting before Dispatching** way, that i talked about in the project that was mentioned above.

**OutboxPattern**, this will help us, ensure that the events are sent, and if any errors occur while sending, they will be kept until sent.

We create our Domain event in the Domain layer, inheriting from our `IDomainEvent` interface

```cs
public record PatientCreatedDomainEvent(Guid Id, string Name, string Email, string Address) : IDomainEvent;
```

Now, how would we say that this has happened?

For that, since this is related to when a `Patient` is created, we will do it inside the constructor

Inside the constructor we would do
```cs
...
DomainEvents.Add(
    new PatientCreatedDomainEvent(Id, Name, Email, Address)
);
```

Now, here is where the **Outbox pattern will help us**
This pattern, will help achieve consistency in the events that are triggered, because we can keep track of what events were sent, and resend them, if they were not successfull.

### The Outbox Event entity
```cs
public class Event{
    public string EventName { get; private set; }

    public DateTime DateOccurred { get; private set; }

    public DateTime? ProcessedDate { get; private set; }

    //The data that the event has sent
    public string Data { get; private set; }

    private Event() { }

    public Event(Guid id, string eventName, DateTime dateOccurred, string data) : base(id)
    {
        EventName = eventName;
        DateOccurred = dateOccurred;
        Data = data;
    }

    public void Processed() 
    {
        ProcessedDate = DateTime.UtcNow;
    }
}
```

Now, let's save the event, everytime something changes...

For that, we will need to now use the **UnitOfWork** pattern, so that we can get all the events that were triggered by the entities + save changes in an atomic way.

Here's how the [Commit()](src/Hospital.API/Infrastructure/Repositories/UnitOfWork.cs) method would look like


Let's walk trough this code by steps

1 - We get all the domain events, that were dispatched, from our aggregates

2 - While iterating over each single one, we map it to the correct integration event

3 - In the end, we create the `Event` entity, with the serialized integration event, and save everything atomically.

If you see this, we can see that, if something either goes wrong with saving it on the database, we will do nothing... It's one of those cases, we either get all of it, or none. And that's how it should be, to avoid inconsistency.

Now, to inform the other service that something has happened, we need to send this event, as a message(through a message bus like RabbitMQ or Kafka).

To implement this, we will use RabbitMQ as our message broker

And to achieve this, we will use something that dotnet has, out of the box, which is a `BackgroundService`, that when you class inherits from it, and you register it, it will run as a singleton, during you project lifetime in a constant loop.

Our [BackgroundService](src/Hospital.API/Infrastructure/Workers/IntegrationEventPublisherService.cs) will look like this

Let's run trough the code!

While the system is running, we get all our `Event`, that we previously saved in the database and that weren't processed still.

Next, for each single one of them, we try to publish it, to our queue, and if successfull, we will just set it, as it was processed, so that we don't send duplicated messages, and saving those changes.

We wrapped this code in a `try-catch`, because if something fails, we need to be informed in some sort of way, so that we can fix it.

To finalize, we also need to register this service like this
`services.AddHostedService<MyBackgroundService>`, otherwise it won't be running in the background.

### Consuming the events inside Sales system

Now, we are going to act as consumers of the events, and do the needed logic, that we need everytime we are informed about it.

To handle those situations, we will use the Mediatr package, so that we can use the `INotification` interface and publish those same messages internally.

Let's setup the Integration events and handlers...

Create [IIntegrationEvent](src/Sales.API/Domain/Events/IIntegrationEvent.cs) interface

Create the [PatientCreatedIntegrationEvent](src/Sales.API/Infrastructure/Events/PatientCreatedIntegrationEvent.cs)

Create the [PatientCreatedIntegrationEventHandler](src/Sales.API/Infrastructure/Events/Handlers/PatientCreatedIntegrationEventHandler.cs) which will handle the logic for when this event happens

Now, onto consuming those messages

The path that i followed was:
- We subscribe to an event type on the project startup.

- We will use [IEventSubscriptions](src/Sales.API/Infrastructure/Subscriptions/EventBusSubscriptions.cs) to keep track of what events we subscribed and what are the handlers for that same event

- Then, we create a listener to that event type, so that we can receive the messages in real-time

- After that, we get the message information, and call the needed handler based on the event type.

Let's get to it!

First let's [subscribe](src/Sales.API/Infrastructure/EventBus/RabbitMQBus.cs) to events, and at the same time say to RabbitMQ to create the needed consumer/listener to that queue, so that we receive the messages

Then, inform on startup the events to subscribe, using the [RegisterEventSubscriptions](src/Sales.API/Infrastructure/InjectDependencies.cs)

Next up, we need to register our `IEventBus` so that we can subscribe to events and handle those as well
```cs
//This will keep track of our subscribed events
services.AddSingleton<IEventSubscriptions, EventSubscriptions>();
services.AddSingleton<IEventBus, RabbitMQBus>();
```

To finalize, to handle those events, we will need to call our `Consumer_Received()` method, which is inside of it, will do the following steps

1 - Consume the message

2 - Get the event type based on the message

3 - Tell the `IEventSubscriptions` to handle the event, based on the event type

4 - Remove the message from the queue(acknowledge that it was consumed), so that we don't process it more than once

---

## Examples to test

We can test a scenario, that will most certainly happen during any software lifetime, that is
1 - Run only the `Hospital.API` project
2 - Try and create a new Patient

What will happen?

Since we used a message broker, our event as published, but the consumers didn't consume it yet!(Kinda simulated when one of our services is down)

Now, if we run the `Sales.API`, we can see that the event was consumed, and it is not in the queue!

This will help us keep the data consistent and assure that the system will continue to work and what happened while it was down, doesn't just vanish.

## Final thoughts
As this demo shows, we can achieve with this more consistency with our system and how it propagates information to other external systems.

We can also, if we want, make `Domain Events` be published to a Queue.

It also shows what in reality, a `Integration Event` is, and one of my favorite ways of implementing it.

And, it was also show something interesting, that in a system, a `Domain Event` can also be an `Integration Event`, one of the main differences, it's **where** they are being sent or **Who** they are informing.

---

## Run the project

```
docker compose up
```
The project will start all three services in the following endpoints

- Hospital API
    - localhost:4000
- Sales API
    - localhost:3000
- RabbitMQ Service
    - localhost:15672(management UI)
    - localhost:5672(channel)

## References

TODO: ???

[NelsonBN RabbitMQ](https://github.com/NelsonBN/demo-rabbitmq)

[Microsoft](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation)

[Event Bus](https://github.com/evgomes/net-core-event-bus)

[Renato Groffe RabbitMQ](https://renatogroffe.medium.com/net-5-rabbitmq-exemplos-de-implementa%C3%A7%C3%A3o-1366663b8519)

[Domain Events](https://github.com/filipe11402/DDD-DomainEvents)