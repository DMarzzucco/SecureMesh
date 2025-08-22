namespace IdentifyService.Queues.Messaging;

public partial class MessagingQueues
{
    public static class QueuesNames
    {
        public const string EmailVerficationQueue = "email_verification_queue";
        public const string WelcomeQueue = "welcome_queue";
        public const string NewEmailVerificationQueue = "new_email_verification";
        public const string PasswordRecuperationQeue = "password_recuperation";
        public const string TwoAFCodeQeue = "2af_queue";
        public const string VerifySessionQueue = "RBA_queue";
    }
}