using IronClead.SharedKernel.Shared.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.CommandServices;

public class VehicleCommandService : IVehicleCommandService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VehicleCommandService(IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
    {
        await _vehicleRepository.AddAsync(vehicle);
        await _unitOfWork.CompleteAsync();
        return vehicle;
    }

    public async Task<Vehicle> UpdateVehicleAsync(int vehicleId, Vehicle updatedVehicle)
    {
        var existing = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (existing == null) throw new KeyNotFoundException("Vehicle not found");

        existing.UpdateVehicleDetails(
            updatedVehicle.Passengers,
            updatedVehicle.LuggageCapacity,
            updatedVehicle.ModelId,
            updatedVehicle.BrandId,
            updatedVehicle.CompanyId
        );

        _vehicleRepository.Update(existing);
        await _unitOfWork.CompleteAsync();
        return existing;
    }

    public async Task<bool> DeleteVehicleAsync(int vehicleId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle == null) return false;

        _vehicleRepository.Remove(vehicle);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}
