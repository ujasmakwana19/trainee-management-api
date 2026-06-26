using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Data.IDateTimeAutoService;
using TraineeManagement.Data.SubmissionModel;
using TraineeManagement.Data.UserModel;

namespace TraineeManagement.Data.SubmissionFileModel;

public class SubmissionFile : IDateTimeAuto
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [RequiredField]
    public long SubmissionId {get; set;}
    public Submission Submission {get; set; }= null!;
    
    [RequiredField]
    // Used to serve the file name with the same name to there client
    public string OriginalFileName { get; set; } = string.Empty;
    
    [RequiredField]
    // storage path name
    public string StorageName { get; set; } = string.Empty!;
    
    [RequiredField]
    // Content Type of the file stored ()
    public string ContentType { get; set; } = string.Empty!;

    [RequiredField]
    // Size of the file in bytes
    public long SizeBytes { get; set; }

    [RequiredField]
    // To check integrity of file and the handle duplicate file upload
    public string Checksum {get; set;} = string.Empty;

    [RequiredField]
    // Used to see who uploaded
    public long UploadedByUserId {get; set;}
    public User User {get; set; }= null!;

}