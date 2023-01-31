using Domain.DTO;

namespace Domain.RepositoryDefinitions;

public interface OpenDroneDispatchCollectionSettings
{
    public IDroneRepository GetFleetCollection();

    public IDeliveriesRepository GetDeliveriesCollection();
    string GetApiKey();
    GeoLocation GetHomeLocation();
    string GetDispatchUrl();
}