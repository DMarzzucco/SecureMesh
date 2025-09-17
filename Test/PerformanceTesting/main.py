import time
from faker import Faker
from locust import HttpUser, between, task
from utils.generators import GeneratorsStrings

fake = Faker()


class SystemSecureMeshFlow(HttpUser):

    wait_time = between(1, 5)

    def on_start(self):
        self.generator = GeneratorsStrings()

        unique_id = self.generator.random_string()

        self.full_name = f"{fake.first_name} {fake.last_name}"
        self.username = f"{fake.first_name}{unique_id}"
        self.email = f"{fake.last_name}@{self.generator.random_domain_email()}"

    @task
    def register_and_login_session(self):
        password_gen = self.generator.random_password_generation()
        headers = {"Content-Type": "application/json"}

        payload_reg = {
            "FullName": f"{self.full_name}",
            "Username": f"{self.username}",
            "Email": f"{self.email}",
            "Password": f"{password_gen}",
            "Roles": 0,
        }

        response = self.client.post(
            "/api/Idp/registered", json=payload_reg, headers=headers, verify=False
        )

        print (f"RESPONSE MESSAGE {response.text}")

        time.sleep(5)

        payload_log = {"Username": self.username, "Password": "Pr@motheus98"}

        self.client.put(
            "/api/Idp/login", json=payload_log, headers=headers, verify=False
        )
