using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Data;
using UMOApi.Models;

namespace UMOApi.Controllers;

[ApiController]
[Route("")]
public class ClientsController : ControllerBase
{
    private readonly UMOApiDbContext _context;

    public ClientsController(UMOApiDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a paginated list of clients.
    /// </summary>
    [HttpGet("clients")]
    public async Task<ActionResult<PaginatedResponse<ClientListDto>>> GetClients(
        [FromQuery] int startIndex = 0,
        [FromQuery] int count = 20,
        [FromQuery] string? filterOperators = null)
    {
        var query = _context.ClientDetails.AsQueryable();

        // Apply filters if provided
        if (!string.IsNullOrEmpty(filterOperators))
        {
            // Parse and apply filters (simplified implementation)
            var filters = filterOperators.Split(',');
            foreach (var filter in filters)
            {
                var parts = filter.Split(':');
                if (parts.Length == 2)
                {
                    var field = parts[0].ToLower();
                    var value = parts[1];
                    
                    query = field switch
                    {
                        "status" => query.Where(c => c.Status != null && c.Status.Description == value),
                        "lastname" => query.Where(c => c.LastName != null && c.LastName.Contains(value)),
                        "firstname" => query.Where(c => c.FirstName != null && c.FirstName.Contains(value)),
                        _ => query
                    };
                }
            }
        }

        var totalCount = await query.CountAsync();
        var pageCount = (int)Math.Ceiling((double)totalCount / count);

        var clients = await query
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Skip(startIndex)
            .Take(count)
            .Select(c => new ClientListDto
            {
                Id = c.Id,
                MandantId = c.MandantId,
                Nummer = c.Nummer,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Sex = c.Sex,
                BirthDay = c.BirthDay,
                Status = c.Status != null ? c.Status.Description : null,
                StartContractDate = c.StartContractDate,
                PolicyNumber = c.PolicyNumber,
                Note = c.Note,
                FreeFeld = c.FreeFeld,
                BooleanOptions = c.BooleanOptions
            })
            .ToListAsync();

        return Ok(new PaginatedResponse<ClientListDto>
        {
            Count = totalCount,
            Start = startIndex,
            PageCount = pageCount,
            Results = clients
        });
    }

    /// <summary>
    /// Retrieves detailed information about a specific client.
    /// </summary>
    [HttpGet("clientdetails")]
    public async Task<ActionResult<ClientDetailsDto>> GetClientDetails(
        [FromQuery] int mandantId,
        [FromQuery] int id)
    {
        var client = await _context.ClientDetails
            .Include(c => c.Titel)
            .Include(c => c.Prefix)
            .Include(c => c.Status)
            .Include(c => c.MaritalStatus)
            .Include(c => c.Language)
            .Include(c => c.Priority)
            .Include(c => c.InvoiceMethod)
            .Include(c => c.Classification)
            .Include(c => c.Reason)
            .Include(c => c.FinancialGroup)
            .Include(c => c.Address)
                .ThenInclude(a => a!.City)
            .Include(c => c.Address)
                .ThenInclude(a => a!.District)
            .Include(c => c.Address)
                .ThenInclude(a => a!.Country)
            .Include(c => c.Tarif)
                .ThenInclude(t => t!.VatTax)
            .Include(c => c.Phones)
            .Include(c => c.Notations)
            .Include(c => c.Features)
            .Include(c => c.Diseases)
            .Include(c => c.Costs)
            .Include(c => c.StatusHistory)
            .Include(c => c.Medicin)
                .ThenInclude(m => m.ProfessionalProvider)
            .FirstOrDefaultAsync(c => c.MandantId == mandantId && c.Id == id);

        if (client == null)
        {
            return NotFound($"Client with MandantId {mandantId} and Id {id} not found.");
        }

        var dto = MapToClientDetailsDto(client);
        return Ok(dto);
    }

    /// <summary>
    /// Creates a new client.
    /// </summary>
    [HttpPost("clientdetails")]
    public async Task<ActionResult<ClientDetailsDto>> CreateClient([FromBody] ClientDetailsCreateDto createDto)
    {
        var client = new ClientDetails
        {
            MandantId = createDto.MandantId,
            Nummer = createDto.Nummer,
            TitelId = createDto.TitelId,
            PrefixId = createDto.PrefixId,
            SocialSecurityNumber = createDto.SocialSecurityNumber,
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Sex = createDto.Sex,
            BirthDay = createDto.BirthDay,
            Memo = createDto.Memo,
            StatusId = createDto.StatusId,
            StartContractDate = createDto.StartContractDate,
            EndContractDate = createDto.EndContractDate,
            PolicyNumber = createDto.PolicyNumber,
            Note = createDto.Note,
            FreeFeld = createDto.FreeFeld,
            BooleanOptions = createDto.BooleanOptions,
            MedecinPlace = createDto.MedecinPlace,
            Bill = createDto.Bill,
            BankNumber = createDto.BankNumber,
            BankName = createDto.BankName,
            PostBankNumber = createDto.PostBankNumber,
            InsuresCare = createDto.InsuresCare,
            ClassificationId = createDto.ClassificationId,
            InvoiceMethodId = createDto.InvoiceMethodId,
            ReasonId = createDto.ReasonId,
            PriorityId = createDto.PriorityId,
            MaritalStatusId = createDto.MaritalStatusId,
            LanguageId = createDto.LanguageId,
            FinancialGroupId = createDto.FinancialGroupId,
            AddressId = createDto.AddressId,
            TarifId = createDto.TarifId,
            CreateId = "system",
            CreateDate = DateTime.UtcNow
        };

        _context.ClientDetails.Add(client);
        await _context.SaveChangesAsync();

        // Reload with includes
        var createdClient = await _context.ClientDetails
            .Include(c => c.Titel)
            .Include(c => c.Status)
            .FirstOrDefaultAsync(c => c.Id == client.Id);

        return CreatedAtAction(nameof(GetClientDetails), 
            new { mandantId = client.MandantId, id = client.Id }, 
            MapToClientDetailsDto(createdClient!));
    }

    /// <summary>
    /// Updates an existing client.
    /// </summary>
    [HttpPut("clientdetails")]
    public async Task<ActionResult<ClientDetailsDto>> UpdateClient([FromBody] ClientDetailsCreateDto updateDto, [FromQuery] int id)
    {
        var client = await _context.ClientDetails.FindAsync(id);
        if (client == null)
        {
            return NotFound($"Client with Id {id} not found.");
        }

        // Update properties
        client.Nummer = updateDto.Nummer ?? client.Nummer;
        client.TitelId = updateDto.TitelId ?? client.TitelId;
        client.PrefixId = updateDto.PrefixId ?? client.PrefixId;
        client.SocialSecurityNumber = updateDto.SocialSecurityNumber ?? client.SocialSecurityNumber;
        client.FirstName = updateDto.FirstName ?? client.FirstName;
        client.LastName = updateDto.LastName ?? client.LastName;
        client.Sex = updateDto.Sex ?? client.Sex;
        client.BirthDay = updateDto.BirthDay ?? client.BirthDay;
        client.Memo = updateDto.Memo ?? client.Memo;
        client.StatusId = updateDto.StatusId ?? client.StatusId;
        client.StartContractDate = updateDto.StartContractDate ?? client.StartContractDate;
        client.EndContractDate = updateDto.EndContractDate ?? client.EndContractDate;
        client.PolicyNumber = updateDto.PolicyNumber ?? client.PolicyNumber;
        client.Note = updateDto.Note ?? client.Note;
        client.FreeFeld = updateDto.FreeFeld ?? client.FreeFeld;
        client.BooleanOptions = updateDto.BooleanOptions ?? client.BooleanOptions;
        client.MedecinPlace = updateDto.MedecinPlace ?? client.MedecinPlace;
        client.Bill = updateDto.Bill ?? client.Bill;
        client.BankNumber = updateDto.BankNumber ?? client.BankNumber;
        client.BankName = updateDto.BankName ?? client.BankName;
        client.PostBankNumber = updateDto.PostBankNumber ?? client.PostBankNumber;
        client.InsuresCare = updateDto.InsuresCare ?? client.InsuresCare;
        client.ClassificationId = updateDto.ClassificationId ?? client.ClassificationId;
        client.InvoiceMethodId = updateDto.InvoiceMethodId ?? client.InvoiceMethodId;
        client.ReasonId = updateDto.ReasonId ?? client.ReasonId;
        client.PriorityId = updateDto.PriorityId ?? client.PriorityId;
        client.MaritalStatusId = updateDto.MaritalStatusId ?? client.MaritalStatusId;
        client.LanguageId = updateDto.LanguageId ?? client.LanguageId;
        client.FinancialGroupId = updateDto.FinancialGroupId ?? client.FinancialGroupId;
        client.AddressId = updateDto.AddressId ?? client.AddressId;
        client.TarifId = updateDto.TarifId ?? client.TarifId;
        client.UpdateId = "system";
        client.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Reload with includes
        var updatedClient = await _context.ClientDetails
            .Include(c => c.Titel)
            .Include(c => c.Status)
            .FirstOrDefaultAsync(c => c.Id == client.Id);

        return Ok(MapToClientDetailsDto(updatedClient!));
    }

    /// <summary>
    /// Deletes a client.
    /// </summary>
    [HttpDelete("clientdetails")]
    public async Task<IActionResult> DeleteClient([FromQuery] int mandantId, [FromQuery] int id)
    {
        var client = await _context.ClientDetails
            .FirstOrDefaultAsync(c => c.MandantId == mandantId && c.Id == id);

        if (client == null)
        {
            return NotFound($"Client with MandantId {mandantId} and Id {id} not found.");
        }

        _context.ClientDetails.Remove(client);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Client deleted successfully." });
    }

    private ClientDetailsDto MapToClientDetailsDto(ClientDetails client)
    {
        return new ClientDetailsDto
        {
            Id = client.Id,
            MandantId = client.MandantId,
            Nummer = client.Nummer,
            Titel = client.Titel?.Description,
            Prefix = client.Prefix?.Description,
            SocialSecurityNumber = client.SocialSecurityNumber,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Sex = client.Sex,
            BirthDay = client.BirthDay,
            Memo = client.Memo,
            Status = client.Status?.Description,
            StartContractDate = client.StartContractDate,
            EndContractDate = client.EndContractDate,
            PolicyNumber = client.PolicyNumber,
            Note = client.Note,
            FreeFeld = client.FreeFeld,
            BooleanOptions = client.BooleanOptions,
            MedecinPlace = client.MedecinPlace,
            Bill = client.Bill,
            BankNumber = client.BankNumber,
            BankName = client.BankName,
            PostBankNumber = client.PostBankNumber,
            InsuresCare = client.InsuresCare,
            Classification = client.Classification?.Description,
            InvoiceMethod = client.InvoiceMethod?.Description,
            Reason = client.Reason?.Description,
            Priority = client.Priority?.Description,
            MaritalStatus = client.MaritalStatus?.Description,
            Language = client.Language?.Description,
            FinancialGroup = client.FinancialGroup?.Description,
            CreateId = client.CreateId,
            CreateDate = client.CreateDate,
            UpdateId = client.UpdateId,
            UpdateDate = client.UpdateDate,
            Address = client.Address != null ? new AddressDto
            {
                Id = client.Address.Id,
                MandantId = client.Address.MandantId,
                Street = client.Address.Street,
                HouseNumber = client.Address.HouseNumber,
                ZipCode = client.Address.ZipCode,
                City = client.Address.City?.Name,
                District = client.Address.District?.Name,
                Country = client.Address.Country?.Name,
                AdditionalInfo = client.Address.AdditionalInfo
            } : null,
            Tarif = client.Tarif != null ? new TarifDto
            {
                Id = client.Tarif.Id,
                MandantId = client.Tarif.MandantId,
                Name = client.Tarif.Name,
                Description = client.Tarif.Description,
                BasePrice = client.Tarif.BasePrice,
                VatPercentage = client.Tarif.VatTax?.Percentage,
                TotalPrice = client.Tarif.TotalPrice,
                ValidFrom = client.Tarif.ValidFrom,
                ValidTo = client.Tarif.ValidTo,
                IsActive = client.Tarif.IsActive
            } : null,
            Phones = client.Phones?.Select(p => new PhoneDto
            {
                Id = p.Id,
                PhoneType = p.PhoneType,
                PhoneNumber = p.PhoneNumber,
                IsPrimary = p.IsPrimary
            }).ToList(),
            Notations = client.Notations?.Select(n => new NotationDto
            {
                Id = n.Id,
                Text = n.Text,
                CreatedDate = n.CreatedDate,
                CreatedBy = n.CreatedBy
            }).ToList(),
            Features = client.Features?.Select(f => new FeatureDto
            {
                Id = f.Id,
                FeatureName = f.FeatureName,
                FeatureValue = f.FeatureValue
            }).ToList(),
            Diseases = client.Diseases?.Select(d => new DiseaseDto
            {
                Id = d.Id,
                DiseaseName = d.DiseaseName,
                IcdCode = d.IcdCode,
                DiagnosisDate = d.DiagnosisDate,
                Notes = d.Notes
            }).ToList(),
            OneTimeCost = client.Costs?.Where(c => c.CostType == "OneTime").Select(c => new CostDto
            {
                Id = c.Id,
                CostType = c.CostType,
                Description = c.Description,
                Amount = c.Amount,
                Date = c.Date
            }).ToList(),
            RegularCost = client.Costs?.Where(c => c.CostType == "Regular").Select(c => new CostDto
            {
                Id = c.Id,
                CostType = c.CostType,
                Description = c.Description,
                Amount = c.Amount,
                Date = c.Date
            }).ToList(),
            StatusHistory = client.StatusHistory?.Select(s => new StatusHistoryDto
            {
                Id = s.Id,
                Description = s.Description,
                ChangeDate = s.ChangeDate,
                ChangedBy = s.ChangedBy
            }).ToList(),
            Medicin = client.Medicin?.Select(m => new MedecinDto
            {
                Id = m.Id,
                Name = m.ProfessionalProvider != null ? 
                    $"{m.ProfessionalProvider.FirstName} {m.ProfessionalProvider.LastName}" : null,
                Specialty = m.ProfessionalProvider?.Specialty,
                Role = m.Role
            }).ToList()
        };
    }
}
