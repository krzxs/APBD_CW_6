namespace APBD_CW_6.DTOs.Responses;

public record RoomDto(
    string Id,
    bool HasTv,
    WardDto Ward
);