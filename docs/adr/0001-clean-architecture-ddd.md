# Clean Architecture and Domain-Driven Design with FastEndpoints

We structured the backend into Clean Architecture layers (Domain, Application, Infrastructure, Presentation) and modeled business logic through Domain-Driven Design (DDD) aggregate roots, value objects, and domain events. FastEndpoints is chosen for the HTTP presentation layer to implement the Request-Endpoint-Response (REPR) pattern in .NET 10. This ensures domain logic remains decoupled from frameworks, persistence, and external APIs, while providing high-performance, maintainable endpoints.
