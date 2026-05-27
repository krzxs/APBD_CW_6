using APBD_CW_6.DTOs;
using APBD_CW_6.DTOs.Requests;
using APBD_CW_6.DTOs.Responses;

namespace APBD_CW_6.Services;

public interface IPatientsService
{
    Task<List<PatientResponseDto>> GetPatientsAsync(string? search);
    Task<BedAssignmentResponseDto> AssignBedAsync(string pesel, AssignBedRequestDto request);
}