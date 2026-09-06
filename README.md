# OmniPay API

OmniPay is a simple payment platform API project that I developed using .NET 8.

The project supports operations such as creating payments, changing payment statuses, and simulating payment processing. While developing this project, I learned and practiced technologies such as Clean Architecture, CQRS, Entity Framework Core, and PostgreSQL.

## What Does the Project Include?

- Creating payments
- Retrieving payment details by ID
- Processing payments
- Marking payments as successful
- Marking payments as failed
- Cancelling payments
- Simulated payment gateway
- PostgreSQL database
- Entity Framework Core
- CQRS command and query structure
- xUnit tests
- API testing with Swagger
- Docker support

## Project Structure

The project mainly consists of the following layers:

- `OmniPay.Domain`: The `Payment` entity and payment-related business rules
- `OmniPay.Application`: Commands, queries, handlers, DTOs, and interfaces
- `OmniPay.Infrastructure`: PostgreSQL, EF Core repository, and simulated payment gateway
- `OmniPay.API`: API controllers and application startup configuration
- `OmniPay.Tests`: Unit tests

## Technologies Used

- .NET 8
- C#
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Npgsql
- xUnit
- Swagger / OpenAPI
- Docker
- GitHub Actions

## Payment Flow

Payment operations are handled through CQRS handlers.

Some of the handlers used in the project are:

- `CreatePaymentCommandHandler`
- `GetPaymentByIdQueryHandler`
- `ProcessPaymentCommandHandler`
- `MarkPaymentAsSuccessfulCommandHandler`
- `MarkPaymentAsFailedCommandHandler`
- `CancelPaymentCommandHandler`

The payment provider currently uses a simulated gateway instead of a real bank or payment provider integration.

## Running the Project

### Requirements

- .NET SDK 8.0 or later
- PostgreSQL or Docker

