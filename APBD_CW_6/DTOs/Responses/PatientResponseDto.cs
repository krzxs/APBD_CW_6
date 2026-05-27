namespace APBD_CW_6.DTOs.Responses;

public record PatientResponseDto(
    string Pesel,
    string FirstName,
    string LastName,
    int Age,
    string Sex,
    IEnumerable<AdmissionResponseDto> Admissions,
    IEnumerable<BedAssignmentResponseDto> BedAssignments
);