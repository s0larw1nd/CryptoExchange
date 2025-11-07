using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using StackExchange.Redis;
using UserAuth.Models;

namespace UserAuth.DAL;

public class UnitOfWork(IOptions<DbSettings> dbSettings): IDisposable
{
    private NpgsqlConnection _connectionPostgres;
    private ConnectionMultiplexer _redis;
    private IDatabase _db;
    
    public async Task<NpgsqlConnection> GetConnectionPostgreSql(CancellationToken token)
    {
        if (_connectionPostgres is not null)
        {
            return _connectionPostgres;
        }
        
        var dataSource = new NpgsqlDataSourceBuilder(dbSettings.Value.ConnectionString);
        
        dataSource.MapComposite<UserDal>("v1_users");
       
        _connectionPostgres = dataSource.Build().CreateConnection();
        _connectionPostgres.StateChange += (sender, args) =>
        {
            if (args.CurrentState == ConnectionState.Closed)
                _connectionPostgres = null;
        };
        
        await _connectionPostgres.OpenAsync(token);

        return _connectionPostgres;
    }

    public async Task<IDatabase> GetConnectionRedis()
    {
        if (_db is not null)
        {
            return _db;
        }
        
        _redis = ConnectionMultiplexer.Connect(dbSettings.Value.ConnectionStringRedis);
        _db = _redis.GetDatabase();
        
        return _db;
    }

    public async ValueTask<NpgsqlTransaction> BeginTransactionAsync(CancellationToken token)
    {
        _connectionPostgres ??= await GetConnectionPostgreSql(token);
        return await _connectionPostgres.BeginTransactionAsync(token);
    }

    public void Dispose()
    {
        DisposeConnection();
        GC.SuppressFinalize(this);
    }
    
    ~UnitOfWork()
    {
        DisposeConnection();
    }
    
    private void DisposeConnection()
    {
        _connectionPostgres?.Dispose();
        _connectionPostgres = null;
        
        _db = null;
        _redis?.Dispose();
        _redis = null;
    }
}