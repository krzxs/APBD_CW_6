namespace APBD_CW_6.DTOs.Responses;

public record AdmissionResponseDto(
    int Id,
    DateTime AdmissionDate,
    DateTime? DischargeDate,
    WardDto Ward
);