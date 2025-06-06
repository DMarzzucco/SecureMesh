# Usa la imagen oficial de Rust como base
FROM rust:latest as builder

RUN rustup target add x86_64-unknown-linux-musl
RUN apt-get update && apt-get install -y musl-tools

# Establece el directorio de trabajo
WORKDIR /app

# Copia solo Cargo.toml primero (sin el lock)
COPY Cargo.toml .
RUN mkdir src && echo "fn main() {}" > src/main.rs
RUN cargo build --release --target x86_64-unknown-linux-musl || true

# Ahora copiá el resto del código (incluye el verdadero src)
COPY . .

# Compila tu aplicación en modo release
RUN cargo build --release --target x86_64-unknown-linux-musl

# Segunda etapa: crea una imagen más ligera para correr la app
FROM alpine:latest

# Instala las dependencias necesarias para ejecutar tu binario
RUN apk add --no-cache ca-certificates

# Copia el binario compilado desde la etapa anterior
COPY --from=builder /app/target/x86_64-unknown-linux-musl/release/welcome_message /usr/local/bin/welcome_message

# Define el comando por defecto
CMD ["welcome_message"]
