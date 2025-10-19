using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using PriceFeed.Models;

namespace PriceFeed.DAL;

public class UnitOfWork(IOptions<DbSettings> dbSettings): IDisposable
{
    private NpgsqlConnection _connection;
    
    public async Task<NpgsqlConnection> GetConnection(CancellationToken token)
    {
        if (_connection is not null)
        {
            return _connection;
        }
        
        var dataSource = new NpgsqlDataSourceBuilder(dbSettings.Value.ConnectionString);
        
        dataSource.MapComposite<PricesDal>("v1_prices_raw");
       
        _connection = dataSource.Build().CreateConnection();
        _connection.StateChange += (sender, args) =>
        {
            if (args.CurrentState == ConnectionState.Closed)
                _connection = null;
        };
        
        await _connection.OpenAsync(token);

        return _connection;
    }

    public async ValueTask<NpgsqlTransaction> BeginTransactionAsync(CancellationToken token)
    {
        _connection ??= await GetConnection(token);
        return await _connection.BeginTransactionAsync(token);
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
        _connection?.Dispose();
        _connection = null;
    }
}