// use std::{os::windows::process, thread::sleep};

use futures_lite::stream::StreamExt;
use lapin::{options::*, types::FieldTable, Connection,Consumer, ConnectionProperties};
use serde::Deserialize;
use serde_json;

#[derive(Debug, Deserialize)]
struct UserMessage {
    Email: String,
    Token: String,
    Id: Option<u32>,
}

#[tokio::main]
async fn main() {
    let addr = "amqp://user:password@localhost:5672/%2f";
    // let addr = "amqp://user:password@rabbitmq:5672/%2f";
    let conn = Connection::connect(addr, ConnectionProperties::default())
        .await
        .expect("❌ not could connecto to RabbitMQ");

    let channel = conn
        .create_channel()
        .await
        .expect("❌ no could create a channel");

    channel
        .queue_declare(
            "woker4",
            QueueDeclareOptions::default(),
            FieldTable::default(),
        )
        .await
        .expect("❌ no could declarte the queue");

    let consumer = loop {
        match channel
            .basic_consume(
                "new_email_verification",
                "verification_new_email_worker",
                BasicConsumeOptions::default(),
                FieldTable::default(),
            )
            .await
        {
            Ok(consumer) => {
                println!("✅ Connect to queue");
                break consumer;
            }
            Err(_) => {
                println!("Wait for a queue..");
                tokio::time::sleep(std::time::Duration::from_secs(10)).await;
            }
        }
    };

    process_message(consumer).await;
}

///process_message fn
async fn process_message(mut consumer: Consumer) {
    println!("🐇 wait for the message...");

    while let Some(result) = consumer.next().await {
        match result {
            Ok(delivery) => {
                let body = &delivery.data;

                match serde_json::from_slice::<UserMessage>(body) {
                    Ok(msg) => {
                        println!(
                        "📤 Send verification new email: {} with adress : https://localhost:5090/api/Security/5413444_dsdn123fS_231_ddf?klt1276={}",
                        msg.Email, msg.Token
                        );
                    }
                    Err(e) => {
                        eprintln!("❌ Error to deserialize message: {:?}", e);
                    }
                }
                delivery
                    .ack(BasicAckOptions::default())
                    .await
                    .expect("❌ Error not could send ACK");
            }
            Err(e) => {
                eprintln!("❌ error to recibe the message: {:?}", e);
            }
        }
    }
}
