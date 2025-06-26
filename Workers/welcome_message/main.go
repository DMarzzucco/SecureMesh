package main

import (
	"encoding/json"
	"fmt"
	"log"

	"github.com/streadway/amqp"
)

type UserMessage struct {
	FullName string `json:FullName`
	Email    string `json:"Email"`
	Id       *int   `json:"Id,omitempty"`
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
		"welcome_queue",
		true,
		false,
		false,
		false,
		nil,
	)
	failOnError(err, "no could declare the queue")

	msgs, err := ch.Consume(
		q.Name,
		"welcome_worker",
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
				fmt.Printf("⛰ Welcome %s, your address %s\n was verificate, Now yout can log in", msg.FullName, msg.Email)
			}

			d.Ack(false)
		}
	}()

	<-forever
}
