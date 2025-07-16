package main

import (
	"encoding/json"
	"fmt"
	"log"

	"github.com/streadway/amqp"
)

type UserMessage struct {
	Email     string `json:"Email"`
	TwoAFCode string `json:"TwoAFCode"`
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
		"2af_queue",
		true,
		false,
		false,
		false,
		nil,
	)
	failOnError(err, "no could declare the queue")

	msgs, err := ch.Consume(
		q.Name,
		"2af_worker",
		false,
		false,
		false,
		false,
		nil,
	)
	failOnError(err, "no could consume the queue")

	fmt.Println("🔶 wait for the message...")

	// channel to wait forever
	forever := make(chan bool)

	go func() {
		for d := range msgs {
			var msg UserMessage
			if err := json.Unmarshal(d.Body, &msg); err != nil {
				log.Printf("❌ Error to deserialize message: %s", err)
			} else {
				fmt.Printf("Hi %s, your code of verification is: %s\n", msg.Email, msg.TwoAFCode)
			}

			d.Ack(false)
		}
	}()

	<-forever
}
