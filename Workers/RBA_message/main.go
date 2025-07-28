package main

import (
	"encoding/json"
	"fmt"
	"log"

	"github.com/streadway/amqp"
)

type SessionsMessage struct {
	Token     string `json:"Token"`
	Email     string `json:"Email"`
	UserAgent string `json:"UserAgent"`
	Location  string `json:"Location"`
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
		"RBA_queue",
		true,
		false,
		false,
		false,
		nil,
	)
	failOnError(err, "no could declare the queue")

	msgs, err := ch.Consume(
		q.Name,
		"RBA_worker",
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
			var msg SessionsMessage
			if err := json.Unmarshal(d.Body, &msg); err != nil {
				log.Printf("❌ Error to deserialize message: %s", err)
			} else {
				fmt.Printf("⚠⚠⚠ Hi %s, your account init session in %s and location %s : If your reali you put over theare : https://localhost:8888/api/Security/lskda_2312sd2000123sdaSD?k892=%s\n", msg.Email, msg.UserAgent, msg.Location, msg.Token)
			}

			d.Ack(false)
		}
	}()

	<-forever
}
