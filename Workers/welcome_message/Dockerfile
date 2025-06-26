FROM golang:1.24.4 AS builder

WORKDIR /app

COPY go.mod ./
RUN go mod download

COPY . .

RUN go build -o api

FROM debian:bookworm-slim

WORKDIR /app

RUN apt-get update && apt-get install -y netcat-openbsd && rm -rf /var/lib/apt/lists/*

COPY --from=builder /app/api .
COPY wait-for-it.sh ./wait-for-it.sh

RUN chmod +x wait-for-it.sh
# CMD ["./api"]
CMD ["./wait-for-it.sh", "rabbitmq:5672", "--", "./api"]