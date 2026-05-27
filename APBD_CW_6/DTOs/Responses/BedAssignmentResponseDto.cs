namespace APBD_CW_6.DTOs.Responses;

public record BedAssignmentResponseDto(
    int Id,
    DateTime From,
    DateTime? To,
    BedDto Bed
);