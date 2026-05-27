namespace APBD_CW_6.DTOs.Responses;

public record BedDto(
    int Id,
    BedTypeDto BedType,
    RoomDto Room
);