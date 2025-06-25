# 🔐 SecureMesh - Distributed Security System

Secure Mesh is a security, authentication, and authorization testing template designed to easily scale to a larger distributed system. The system is composed of three independent APIs and a relational database, all orchestrated using Docker and docker-compose.

> [!NOTE]
> The project is currently orchestrated with docker-compose, but it will soon be migrated to Kubernetes.


## 🧱 General Architecture

The system is composed of the following services:

- `User API`: Manages the full lifecycle of a user.
- `Security API`: Responsible for validating credentials, generating tokens, and verifying user accounts.
- `Hangfire Server`: Executes background jobs.
- `API Gateway`: Centralizes routes and manages authentication, roles and access limits.
- `Redis`: - `Redis`: Base de datos que se usa para almacenar tokens de un solo uso, los cuales son eliminados con TTL una vez hayan expirados.
- `RabbitMQ`: Asynchronous messaging system used for email verification, welcome messages, and account recovery.
- `PostgreSQL`: Relation database for persisten storage.

![Models](/img/arch23.png)

All services are independent, self-contained, and communicate using `gRPC` when necessary.

---

## 🔒 Security and Authentication

- **JWT**: Uses both accessToken and refreshToken for secure session handling.
- **Secure cookies**: HTTP-only and SSL-protected.
- **CSRF Tokens**: Temporarily implemented as a per-session protection mechanism.
- **Account Verification**: Users must verify their email before logging in.
- **Token renewal**: Performed automatically via middleware before expiration.
- **Roles**: Hierarchy integrated and validated at the `gateway-api` level.
- **Authorization policies**: Based on role and resource accessed.
- **Rate limiting**: Protects sensitive routes from brute-force and malicious attempts.

> [!WARNING]
> The SSL certificates used as tests are self-signed, so in certain parts of the source code, you can find code that allows them to be used exclusively for development.
---
## 🟦 Database - User Model

```SQL
CREATE TABLE "User"(
    "Id" integer GENERATED ALWAYS AS IDENTITY NOT NULL,
    "FullName" varchar(50) NOT NULL,
    "Username" varchar(50) NOT NULL,
    "Email" varchar(50) NOT NULL,
    "EmailVerified" boolean NOT NULL,
    "Password" text NOT NULL,
    "Roles" varchar(20) NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "ScheduledDeletionJobId" text,
    "CsrfToken" text,
    "CsrfTokenExpiration" timestamp with time zone,
    "RefreshToken" text,
    PRIMARY KEY(Id)
);
```

## 🚀 Deployment
1. You must have Docker Desktop and Makefile installed.
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
2. Security API ["https://*:5090"]("https://localhost:5090/swagger/index.html")
3. Hangfire ["https://*:3434"]("https://localhost:3434/hangfire")
4. API Gateway ["https://*:8888"]("https://localhost:8888/")
5. RabbitMQ ["https://*:15672"]("http://localhost:15672/#/")

> [!NOTE]
> Since they are using self-signed SSL certificates, I have not yet been able to invalidate their use, so Swagger cannot be accessed through the API gateway, only through Postman.

---

## ✍ Author
### **Dario Marzzucco**
