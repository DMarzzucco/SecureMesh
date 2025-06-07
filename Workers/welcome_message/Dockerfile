# Etapa 1: builder
FROM rust:latest as builder

# Añadimos target musl para compilación estática
RUN rustup target add x86_64-unknown-linux-musl

# Instalamos musl-tools para compilación
RUN apt-get update && apt-get install -y musl-tools && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copiamos solo Cargo.toml y creamos dummy main.rs para cachear deps
COPY Cargo.toml .
RUN mkdir src && echo "fn main() {}" > src/main.rs

# Construimos dummy para cache de deps
RUN cargo build --release --target x86_64-unknown-linux-musl || true

# Copiamos todo el código real
COPY . .

# Build real del binario estático
RUN cargo build --release --target x86_64-unknown-linux-musl

# Etapa 2: imagen final, pequeña y limpia
FROM alpine:latest

RUN apk add --no-cache ca-certificates

# Copiamos el binario compilado desde builder
COPY --from=builder /app/target/x86_64-unknown-linux-musl/release/welcome_message /usr/local/bin/welcome_message

CMD ["welcome_message"]
