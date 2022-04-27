namespace Domain.RepositoryDefinitions;

public class RepositorySettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string Fleet{get;set;} = null!;
    public string Orders{get;set;} = null!;
    public string Assignments{get;set;} = null!;
}