using IronClead.SharedKernel.Shared.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;


namespace VehicleService.VehicleBounded.Application.Internal.CommandServices;

public class CompanyCommandService : ICompanyCommandService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompanyCommandService(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Company> CreateCompanyAsync(Company company)
    {
        await _companyRepository.AddAsync(company);
        await _unitOfWork.CompleteAsync();
        return company;
    }

    public async Task<Company> UpdateCompanyAsync(int companyId, Company updatedCompany)
    {
        var existingCompany = await _companyRepository.GetByIdAsync(companyId);
        if (existingCompany is null)
            throw new KeyNotFoundException($"Company with ID {companyId} not found.");

        existingCompany.UpdateDetails(updatedCompany.Name, updatedCompany.RUC);
        _companyRepository.Update(existingCompany);
        await _unitOfWork.CompleteAsync();
        return existingCompany;
    }

    public async Task<bool> DeleteCompanyAsync(int companyId)
    {
        var company = await _companyRepository.GetByIdAsync(companyId);
        if (company is null) return false;

        _companyRepository.Remove(company);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}
