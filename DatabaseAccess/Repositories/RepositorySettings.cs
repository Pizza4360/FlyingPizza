namespace DatabaseAccess.Repositories;

public class RepositorySettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string DroneRecordsCollectionName { get; set; } = null!;
}