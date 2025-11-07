namespace UserAuth.DAL;

public class DbSettings
{
    public string MigrationConnectionString { get; set; }
    
    public string ConnectionString { get; set; }
    
    public string ConnectionStringRedis { get; set; }
}