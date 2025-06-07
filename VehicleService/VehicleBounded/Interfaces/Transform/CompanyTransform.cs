using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Interfaces.Resources;

namespace VehicleService.VehicleBounded.Interfaces.Transform;

public static class CompanyTransform
{
    public static CompanyResource ToResourceFromEntity(Company company) =>
        new(company.Name, company.RUC);
}