using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Services;

public interface ICompanyCommandService
{
    Task<Company> CreateCompanyAsync(Company company);
    Task<Company> UpdateCompanyAsync(int companyId, Company updatedCompany);
    Task<bool> DeleteCompanyAsync(int companyId);
}
