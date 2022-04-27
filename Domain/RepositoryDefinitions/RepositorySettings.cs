namespace Domain.RepositoryDefinitions;

public class RepositorySettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string Fleet{get;set;}
    public string Orders{get;set;}
    public string Assignments{get;set;}
}