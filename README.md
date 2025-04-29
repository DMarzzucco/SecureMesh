# 🔐 SecureMesh - Distributed Security System

Secure Mesh is a security, authentication, and authorization testing template designed to easily scale to a larger distributed system. The system is composed of three independent APIs and a relational database, all orchestrated using Docker.

---

## 📌 Index

1. [General Architecture](#general-architecture)
2. [Services](#services)
   - [User API](#user-api)
   - [Security API](#security-api)
3. [API Gateway](#api-gateway)
4. [Communication between Microservices (gRPC)](#communication-between-microservices-grpc)
5. [Security and Authentication](#security-and-authentication)
6. [Orchestration with Docker ](#orchestration-with-docker)
7. [Deployment](#deployment)
9. [Ports](#ports)
10. [Author](#author)

---

## 🧱 General Architecture

The system is composed of the following services:

- `user-api`: User lifecycle management.
- `auth-api`: Responsible for validating credentials and generating tokens.
- `gateway-api`: Centralizes routes and manages authentication, roles and access limits.
- `postgres`: Relation database for persisten storage.

![Models](/img/arch23.png)

All services are independent, self-contained, and communicate using `gRPC` when necessary.

---

## ⚙️ Services

### 👥 User API

- Microservices-oriented architecture with independent business logic.
- Functionalities:
    - Register, delete, retrieve, and update users.
    - Update sensitive properties such as password and role (with separate paths).
    - Validations:
        - Valid and unique emails.
        - Unique usernames.
        - Strong passwords (minimum 8 characters, one capital letter, one symbol).
        - Emails restricted to a whitelist of domains.
- ORM: `Entity Framework`.
- DTO mapping: `AutoMapper`.
- Exposes `gRPC` services for:
    - `GetUserByIdForAuth`
    - `FindByValueForAuth (username/password)`
    - `UpdateRefreshToken`
---

### 🔒 Security API

- gRPC client that connects with `user-api`.
- Features:
    - Credential validation.
    - Generation of `accessToken` and `refreshToken`.
    - Hashes the refresh token once it has been generated.
    - Stores tokens in secure cookies.
    - Automatic token renewal middleware if the token is about to expire.
    - Invalidates old tokens after each renewal.

---

## 🚪 API Gateway

- Centralizes requests using `YARP` (Yet Another Reverse Proxy).
- Features:
    - API routing (`user-api` and `auth-api`).
    - CORS policy management.
    - Authentication with `OAuth2`, `JWT Bearer`, and `Cookies`.
    - Integrated role hierarchy.
    - DoS attack protection using `RateLimit` policies.

---

## 📡 Communication between Microservices (gRPC)

- `user-api` acts as a `gRPC` server.
- `auth-api` acts as a client.
- Services exposed by `user-api`:
    - `GetUserByIdForAuth`
    - `FindByValueForAuth (username + password)`
    - `UpdateRefreshToken`

---

## 🔒 Security and Authentication

- **JWT**: Use of `accessToken` and `refreshToken` for secure sessions.
- **Secure cookies**: HTTP-only and SSL-protected.
- **Token renewal**: Automatic via middleware before expiration.
- **Roles**: Integrated hierarchy validated from `gateway-api`.
- **Authorization policies**: Based on role and resource accessed.
- **Rate limiting**: Protects routes against malicious requests.

> [!WARNING]
> The SSL certificates used as tests are self-signed, so in certain parts of the source code, you can find code that allows them to be used exclusively for development.
---

## 🐳 Orchestration with Docker 

- Each service runs in its own individual container.
- Controlled start sequence:
  1. PostgreSQL
  2. `user-api` (with internal retry up to 10 attempts every 5 secons)
     - When the API is connected, migrations are executed automatically.
  3. `auth-api`
     - Its deployment depends on `user-api`, coordinated with `bash` scripts.
  4. `gateway-api`
     - Its deployment depends on `user-api` and `auth-api`, coordinated with `bash` scripts.

---

## 🚀 Deployment

1. Clone repository
2. Execute in terminal:

   ```bash

   docker-compose up --build

   ```
---

## 🔌 Ports
1. User API ["https://*:4080"]("https://localhost:4080/swagger/index.html") 
2. Security API ["https://*:5090"]("https://localhost:5090/swagger/index.html")
3. API Gateway ["https://*:8888"]("https://localhost:8888/")

> [!NOTE]
> Since they are using self-signed SSL certificates, I have not yet been able to invalidate their use, so Swagger cannot be accessed through the API gateway, only through Postman.

---

## ✍ Author
### **Dario Marzzucco**
