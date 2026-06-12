using TraineeManagement.Api.Data;
using TraineeManagement.Api.MentorDTO;

namespace TraineeManagement.Api.MentorServices;

public interface IMentorService
{
    Task DeleteMentor(long id);
    Task<IEnumerable<MentorResponse>> GetAll();
    Task<MentorResponse> GetById(long id);
    Task<MentorResponse> CreateMentor(MentorRequestBody mentor);
    Task<MentorResponse> UpdateMentor(long Id, MentorRequestBody mentor);
}



    
    
