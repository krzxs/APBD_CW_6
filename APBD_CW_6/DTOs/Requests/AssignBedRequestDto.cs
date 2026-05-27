using System.ComponentModel.DataAnnotations;

namespace APBD_CW_6.DTOs.Requests;

public class AssignBedRequestDto
{
    [Required] public DateTime From { get; set; }

    public DateTime? To { get; set; }

    [Required]
    [StringLength(300, MinimumLength = 1)]
    public string BedType { get; set; } = null!;

    [Required]
    [StringLength(300, MinimumLength = 1)]
    public string Ward { get; set; } = null!;
}