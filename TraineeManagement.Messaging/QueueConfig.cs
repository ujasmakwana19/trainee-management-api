namespace TraineeManagement.Messaging;

public class QueueConfig
{
    public const string SubmissionExchange = "submissions.exchange";
    public const string SubmissionQueue = "submission-processing";
    public const string SubmissionRouting = "submission.requested";

    public const string DeadLetterExchange = "submissions.exchange.dlx";
    public const string DeadLetterQueue = "submissions.queue.dlq";
    public const string DeadLetterRoutingKey = "submission.failed";

}