package main

import (
	"encoding/json"
	"fmt"
	"log"

	"github.com/streadway/amqp"
)

type UserMessage struct {
	Email string `json:"Email"`
	Token string `json:"Token"`
	Id    *int   `json:"Id,omitempty"`
}

func failOnError(err error, msg string) {
	if err != nil {
		log.Fatalf("❌ %s: %s", msg, err)
	}
}

func main() {
	// url := "amqp://user:password@localhost:5672/"
	url := "amqp://user:password@rabbitmq:5672/"

	conn, err := amqp.Dial(url)
	failOnError(err, "not could connect to RabbitMQ")
	defer conn.Close()

	ch, err := conn.Channel()
	failOnError(err, "no could create a channel")
	defer ch.Close()

	q, err := ch.QueueDeclare(
		"password_recuperation",
		true,
		false,
		false,
		false,
		nil,
	)
	failOnError(err, "no could declare the queue")

	msgs, err := ch.Consume(
		q.Name,
		"password_recuperation_worker",
		false,
		false,
		false,
		false,
		nil,
	)
	failOnError(err, "no could consume the queue")

	fmt.Println("🐇 wait for the message...")

	// channel to wait forever
	forever := make(chan bool)

	go func() {
		for d := range msgs {
			var msg UserMessage
			if err := json.Unmarshal(d.Body, &msg); err != nil {
				log.Printf("❌ Error to deserialize message: %s", err)
			} else {
				fmt.Printf("📤 Hello %s to Recuperation your account, you need inside in this adress: https://localhost:8888/api/Security/8382fd_1231sfw13312saeDAs12?hmk12=%s\n", msg.Email, msg.Token)
			}

			d.Ack(false)
		}
	}()

	<-forever
}
