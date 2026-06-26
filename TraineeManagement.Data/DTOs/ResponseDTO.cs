using TraineeManagement.Data.TraineeDTO;

namespace TraineeManagement.Data.ResponseDTO;

public record InterCommunicationResponse<T>(
    bool? Success,
    string message,
    int ErrorCode,
    T? Data
);
