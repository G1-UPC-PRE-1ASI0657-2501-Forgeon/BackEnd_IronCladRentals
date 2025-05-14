using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.QueryServices;

public class ModelQueryService : IModelQueryService
{
    private readonly IModelRepository _modelRepository;

    public ModelQueryService(IModelRepository modelRepository)
    {
        _modelRepository = modelRepository;
    }

    public async Task<IEnumerable<Model>> GetAllModelsAsync()
    {
        return await _modelRepository.GetAllAsync();
    }

    public async Task<Model?> GetModelByIdAsync(int id)
    {
        return await _modelRepository.GetByIdAsync(id);
    }
}