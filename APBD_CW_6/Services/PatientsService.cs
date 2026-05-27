using APBD_CW_6.Data;
using APBD_CW_6.DTOs.Requests;
using APBD_CW_6.DTOs.Responses;
using APBD_CW_6.Exceptions;
using APBD_CW_6.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_CW_6.Services;

public class PatientsService : IPatientsService
{
    private readonly ApbdContext _context;

    public PatientsService(ApbdContext context)
    {
        _context = context;
    }

    public async Task<List<PatientResponseDto>> GetPatientsAsync(string? search)
    {
        IQueryable<Patient> query = _context.Patients
            .AsQueryable()
            .Include(p => p.Admissions).ThenInclude(a => a.Ward)
            .Include(p => p.BedAssignments).ThenInclude(ba => ba.Bed).ThenInclude(b => b.BedType)
            .Include(p => p.BedAssignments).ThenInclude(ba => ba.Bed).ThenInclude(b => b.Room).ThenInclude(r => r.Ward);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(p =>
                EF.Functions.Like(p.FirstName, pattern) ||
                EF.Functions.Like(p.LastName, pattern));
        }

        var patients = await query.ToListAsync();
        return patients.Select(p =>
            new PatientResponseDto(
                p.Pesel,
                p.FirstName,
                p.LastName,
                p.Age,
                p.Sex ? "Male" : "Female",
                p.Admissions.Select(a => new AdmissionResponseDto(
                    a.Id,
                    a.AdmissionDate,
                    a.DischargeDate,
                    new WardDto(a.Ward.Id, a.Ward.Name, a.Ward.Description)
                )),
                p.BedAssignments.Select(ba => new BedAssignmentResponseDto(
                    ba.Id,
                    ba.From,
                    ba.To,
                    new BedDto(
                        ba.Bed.Id,
                        new BedTypeDto(ba.Bed.BedType.Id, ba.Bed.BedType.Name, ba.Bed.BedType.Description),
                        new RoomDto(
                            ba.Bed.Room.Id,
                            ba.Bed.Room.HasTv,
                            new WardDto(ba.Bed.Room.Ward.Id, ba.Bed.Room.Ward.Name, ba.Bed.Room.Ward.Description)
                        )
                    )
                ))
            )
        ).ToList();
    }

    public async Task<BedAssignmentResponseDto> AssignBedAsync(string pesel, AssignBedRequestDto request)
    {
        if (request.To is not null && request.To <= request.From)
        {
            throw new BadRequestException("'To' must be later than 'from'.");
        }

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Pesel == pesel)
                      ?? throw new NotFoundException($"Patient with PESEL {pesel} doesn't exist.");
        var ward = await _context.Wards.FirstOrDefaultAsync(w => w.Name == request.Ward)
                   ?? throw new NotFoundException($"Ward {request.Ward} doesn't exist.");
        var bedType = await _context.BedTypes.FirstOrDefaultAsync(bt => bt.Name == request.BedType)
                      ?? throw new NotFoundException($"Bed type {request.BedType} doesn't exist.");

        var requestFrom = request.From;
        var requestTo = request.To;

        var availableBed = await _context.Beds
            .Include(b => b.BedType)
            .Include(b => b.Room).ThenInclude(r => r.Ward)
            .Where(b => b.BedTypeId == bedType.Id)
            .Where(b => b.Room.WardId == ward.Id)
            .Where(b => !b.BedAssignments.Any(ba =>
                (requestTo == null || ba.From <= requestTo) &&
                (ba.To == null || ba.To >= requestFrom)))
            .FirstOrDefaultAsync();

        if (availableBed is null)
        {
            throw new NotFoundException($"No bed of type {request.BedType} is available in ward {request.Ward} during these dates");
        }

        var assignment = new BedAssignment
        {
            PatientPesel = pesel,
            BedId = availableBed.Id,
            From = request.From,
            To = request.To
        };

        _context.BedAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        return new BedAssignmentResponseDto(
            assignment.Id,
            assignment.From,
            assignment.To,
            new BedDto(
                availableBed.Id,
                new BedTypeDto(availableBed.BedType.Id, availableBed.BedType.Name, availableBed.BedType.Description),
                new RoomDto(
                    availableBed.Room.Id,
                    availableBed.Room.HasTv,
                    new WardDto(availableBed.Room.Ward.Id, availableBed.Room.Ward.Name, availableBed.Room.Ward.Description)
                )
            )
        );
    }
}