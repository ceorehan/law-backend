namespace ZALaw.Api.Models;

public enum UserRole
{
    Client = 0,
    Consultant = 1,
    Admin = 2
}

public enum ApplicationStatus
{
    New = 0,
    InProgress = 1,
    Submitted = 2,
    DocumentsUnderReview = 3,
    ConsultantReviewing = 4,
    CorrectionRequired = 5,
    ReadyForFiling = 6,
    Filed = 7,
    Completed = 8
}

public enum DocumentStatus
{
    Uploaded = 0,
    UnderReview = 1,
    Approved = 2,
    Rejected = 3,
    ReplacementRequired = 4
}

public enum AppointmentType
{
    VideoCall = 0,
    PhoneCall = 1,
    InPerson = 2
}

public enum AppointmentStatus
{
    Scheduled = 0,
    Completed = 1,
    Cancelled = 2,
    Rescheduled = 3
}

public enum NotificationType
{
    Message = 0,
    DocumentReview = 1,
    StatusChange = 2,
    Appointment = 3,
    CorrectionRequired = 4
}
