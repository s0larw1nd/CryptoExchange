using Dapper;
using WalletService.DAL;
using WalletService.DAL.Interfaces;
using WalletService.DAL.Repositories;
using WalletService.Migrations;

var builder = WebApplication.CreateSlimBuilder(args);

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                      throw new InvalidOperationException("ASPNETCORE_ENVIRONMENT in not set");

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{environmentName}.json")
    .Build();

var connectionString = config["DbSettings:MigrationConnectionString"];
var migrationRunner = new MigratorRunner(connectionString);
migrationRunner.Migrate();

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection(nameof(DbSettings)));

DefaultTypeMap.MatchNamesWithUnderscores = true;
builder.Services.AddSingleton<UnitOfWork>();

builder.Services.AddSingleton<IWalletRepository, WalletRepository>();
builder.Services.AddSingleton<WalletService.BLL.Services.WalletService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run("http://localhost:5035");