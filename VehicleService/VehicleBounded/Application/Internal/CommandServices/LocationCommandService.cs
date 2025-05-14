using IronClead.SharedKernel.Shared.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.CommandServices;

public class LocationCommandService : ILocationCommandService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LocationCommandService(ILocationRepository locationRepository, IUnitOfWork unitOfWork)
    {
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Location> CreateLocationAsync(Location location)
    {
        await _locationRepository.AddAsync(location);
        await _unitOfWork.CompleteAsync();
        return location;
    }

    public async Task<Location> UpdateLocationAsync(int locationId, Location updatedLocation)
    {
        var existingLocation = await _locationRepository.GetByIdAsync(locationId);
        if (existingLocation is null)
            throw new KeyNotFoundException($"Location with ID {locationId} not found.");

        existingLocation.UpdateDetails(updatedLocation.Address, updatedLocation.City,updatedLocation.Country,updatedLocation.LocationStatus, updatedLocation.Latitude, updatedLocation.Longitude);
        _locationRepository.Update(existingLocation);
        await _unitOfWork.CompleteAsync();
        return existingLocation;
    }

    public async Task<bool> DeleteLocationAsync(int locationId)
    {
        var location = await _locationRepository.GetByIdAsync(locationId);
        if (location is null) return false;

        _locationRepository.Remove(location);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}