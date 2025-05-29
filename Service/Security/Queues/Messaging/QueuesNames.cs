namespace Security.Queues.Messaging;

public partial class MessagingQueues
{
    public static class QueuesNames
    {
        public const string EmailVerficationQueue = "email_verification_queue";
        public const string WelcomeQueue = "welcome_queue";
        public const string NewEmailVerificationQueue = "new_email_verification";
        public const string PasswordRecuperationQeue = "password_recuperation";
    }

}