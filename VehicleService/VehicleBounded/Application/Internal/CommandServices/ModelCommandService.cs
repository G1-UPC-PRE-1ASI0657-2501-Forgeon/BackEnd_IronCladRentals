using IronClead.SharedKernel.Shared.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.CommandServices;


public class ModelCommandService : IModelCommandService
{
    private readonly IModelRepository _modelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ModelCommandService(IModelRepository modelRepository, IUnitOfWork unitOfWork)
    {
        _modelRepository = modelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Model> CreateModelAsync(Model model)
    {
        await _modelRepository.AddAsync(model);
        await _unitOfWork.CompleteAsync();
        return model;
    }

    public async Task<Model> UpdateModelAsync(int modelId, Model updatedModel)
    {
        var existingModel = await _modelRepository.GetByIdAsync(modelId);
        if (existingModel is null)
            throw new KeyNotFoundException($"Model with ID {modelId} not found.");

        existingModel.UpdateCarModel(updatedModel.CarModel);
        _modelRepository.Update(existingModel);
        await _unitOfWork.CompleteAsync();
        return existingModel;
    }

    public async Task<bool> DeleteModelAsync(int modelId)
    {
        var model = await _modelRepository.GetByIdAsync(modelId);
        if (model is null) return false;

        _modelRepository.Remove(model);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}
