namespace TraineeManagement.Contracts.Events;

public record SubmissionProcessingRequested
(
    string MessageId,
    string CorrelationId,
    long TaskAssignmentId,
    DateTime RequestedAt,
    string ContractVersion
);