use futures_lite::stream::StreamExt;
use lapin::{Connection, ConnectionProperties, options::*, types::FieldTable};
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
    // let addr = "amqp://user:password@localhost:5672/%2f";
    let addr = "amqp://user:password@rabbitmq:5672/%2f";
    let conn = Connection::connect(addr, ConnectionProperties::default())
        .await
        .expect("❌ not could connecto to RabbitMQ");

    let channel = conn
        .create_channel()
        .await
        .expect("❌ no could create a channel");

    channel
        .queue_declare(
            "email_verification_queue",
            QueueDeclareOptions::default(),
            FieldTable::default(),
        )
        .await
        .expect("❌ no could declarte the queue");

    let mut consumer = channel
        .basic_consume(
            "email_verification_queue",
            "verficiation_email",
            BasicConsumeOptions::default(),
            FieldTable::default(),
        )
        .await
        .expect("❌ no could consummer the queue");

    println!("🐇 wait for the message...");

    while let Some(result) = consumer.next().await {
        if let Ok(delivery) = result {
            let body = &delivery.data;
            match serde_json::from_slice::<UserMessage>(body) {
                Ok(msg) => {
                    println!(
                        "📤 Send verification email: {} with adress : https://localhost:8888/api/Security/12349smska_wqj1n234msm949401?kl124={}",
                        msg.Email, msg.Token
                    );
                }
                Err(e) => {
                    eprintln!("❌ Error to deserialize message: {:?}", e);
                }
            }

            delivery.ack(BasicAckOptions::default()).await.unwrap();
        }
    }
}

