using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.QueryServices;

public class CompanyQueryService : ICompanyQueryService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyQueryService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        return await _companyRepository.GetAllAsync();
    }

    public async Task<Company?> GetByIdAsync(int id)
    {
        return await _companyRepository.GetByIdAsync(id);
    }

    public async Task<Company?> GetByUserIdAsync(Guid userId)
    {
        return await _companyRepository.GetByUserIdAsync(userId);
    }
}