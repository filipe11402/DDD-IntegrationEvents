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

### Publish the events with the Hospital system

To achieve this, we will use in our favor 2 patterns

**UnitOfWork**, which will help us use the **Commiting before Dispatching** way, that i talked about in the project that was mentioned above.

**OutboxPattern**, this will help us, ensure that the events are sent, and if any errors occur while sending, they will be kept until sent.

TODO:???


## Run the project

```
docker compose up
```
The project will start all three services in the following endpoints

- Hospital
    - localhost:4000
- Sales
    - localhost:3000
- RabbitMQ
    - localhost:15672(management UI)
    - localhost:5672(channel)


## References

TODO: ???