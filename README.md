# 🔐 SecureMesh - Distributed Security System

Secure Mesh is a security, authentication, and authorization testing template designed to easily scale to a larger distributed system. The system is composed of three independent APIs and a relational database, all orchestrated using Docker and docker-compose.

> [!NOTE]
> The project is currently orchestrated with docker-compose, but it will soon be migrated to Kubernetes.

## 🧱 General Architecture

### Overall Vision

This system is built following the principles of Clean Architecture, Hexagonal Architecture, and Domain-Driven Design (DDD). Each microservice is developed as an independent bounded context, speaking its own domain language, and adhering to patterns that ensure separation of concerns, scalability, and maintainability.

The system includes multiple microservices, each focused on a specific domain such as user management, security, sessions, etc.

---

### Key Components

#### 1. Microservices

- `API Gateway`: A unified entry point for clients (web, mobile, desktop). It exposes REST endpoints for consumption, encapsulates routing logic, and implements CORS policies, Rate Limiting, Role Hierarchies, and OAuth2 protocols.
- `Identity Service`: Manages user identity and data. Implements business logic related to profiles, authentication, and permissions.
- `User API`: Manages the full lifecycle of users.
- `Security API`: Manages the full lifecycle of user sessions.
- `Hangfire Server`: Executes background jobs.
- `Redis`: A key-value store used to hold one-time tokens, which are discarded after TTL expiration.
- `RabbitMQ`: An asynchronous messaging system used for email verification, welcome messages, and account recovery.
- `PostgreSQL`: Relational database for persistent storage.

#### 2. Microservice Architecture

Each microservice follows a structure based on:

- **Clean Architecture**:
  - **Domain Models**: Represent core concepts (e.g., UserModel, SessionModel).
  - **Use Cases**: Implement business logic.
  - **Repository and Service Interfaces**: Define abstract data access and external service integrations.

- **Hexagonal Architecture**:
  - **Ports (Interfaces)**: Define how use cases interact with the outside world.
  - **Adapters**: Implement ports using technologies like Entity Framework, gRPC, or REST.

#### 3. Communication Between Components
- **REST**: Primarily used for client-to-microservice and inter-microservice communication in specific scenarios.
- **gRPC**: Used for internal calls between microservices.
- **RabbitMQ**: For asynchronous tasks (e.g., sending emails, notifications).

#### 4. Databases

![Models](/img/db_usermodel.png)

---

## Architecture Diagram
![Models](/img/arch23.png)

---

## 🔒 Security and Authentication

- **JWT**: Utilizes both access tokens and refresh tokens for secure session handling.
- **Secure Cookies**: HTTP-only and SSL-protected.
- **2FA**: Implements two-factor authentication on routes such as password change or account deletion.
- **RBA (Risk-Based Authentication)**: A mechanism to validate and manage each user session.
- **Account Verification**: Users must verify their email before logging in.
- **Token Renewal**: Performed automatically via middleware before token expiration.
- **OTT (One-Time Tokens)**: Used for secure transactions like session verification.
- **Roles**: Hierarchical roles are integrated and validated at the API Gateway level.
- **Authorization Policies**: Based on role and resource access.
- **Rate Limiting**: Protects sensitive routes from brute-force attacks and malicious activities.

> [!WARNING]
> The SSL certificates used as tests are self-signed, so in certain parts of the source code, you can find code that allows them to be used exclusively for development.


---
## 🚀 Deployment
1. You must have Docker Desktop and Makefile (optional) installed.
2. Clone repository
3. Execute in terminal:

```bash
# Deploy RabbitMQ, PostgreSQL, and Redis
make infra

# Deploy all workers
make workers

# Deploy the entire system
make systems

# View all server logs
make logs

# Clean all volumes
make down

# Purge all builds
make purge
```
---

## 🔌 Ports
1. User API ["https://*:4080"]("https://localhost:4080/swagger/index.html") 
2. IPS ["https://*:5090"]("https://localhost:5090/swagger/index.html")
3. Hangfire ["https://*:3434"]("https://localhost:3434/hangfire")
4. API Gateway ["https://*:8888"]("https://localhost:8888/")
5. RabbitMQ ["https://*:15672"]("http://localhost:15672/#/")

> [!NOTE]
> Since they are using self-signed SSL certificates, I have not yet been able to invalidate their use, so Swagger cannot be accessed through the API gateway, only through Postman.

---

## ✍ Author
### **Dario Marzzucco**
