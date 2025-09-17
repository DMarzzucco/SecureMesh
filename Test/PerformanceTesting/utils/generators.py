import string
import random


class GeneratorsStrings:

    # RANDOM STRING
    def random_string(self, length=5):
        return "".join(random.choices(string.ascii_letters + string.digits, k=length))

    # RANDOM DOMAIN EMAIL
    def random_domain_email(self):
        domain_list = [
            "gmail.com",
            "hotmail.com",
            "outlook.com",
            "icloud.com",
            "yahoo.com",
        ]
        return random.choice(domain_list)

    # RANDOM PASSWORD
    def random_password_generation(self, length=12):
        if length < 8:
            length = 8

        uppercase = string.ascii_uppercase
        digits = string.digits
        special = "!@#$%^&*()-_=+[]{}|;:,.<>?/"

        password = [
            random.choice(uppercase),
            random.choice(digits),
            random.choice(special),
        ]

        all_characters = uppercase + digits + special + string.ascii_letters

        while len(password) < length:
            password.append(random.choice(all_characters))

        random.shuffle(password)

        password_str = "".join(password)
        return password_str
