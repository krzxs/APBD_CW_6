using APBD_CW_6.DTOs.Requests;
using APBD_CW_6.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_CW_6.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly IPatientsService _service;

    public PatientsController(IPatientsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? search)
    {
        var patients = await _service.GetPatientsAsync(search);
        return Ok(patients);
    }

    [HttpPost("{pesel}/bedassignments")]
    public async Task<IActionResult> AssignBed([FromRoute] string pesel, [FromBody] AssignBedRequestDto request)
    {
        var result = await _service.AssignBedAsync(pesel, request);
        return Created($"/api/patients/{pesel}/bedassignments/{result.Id}", result);
    }
}