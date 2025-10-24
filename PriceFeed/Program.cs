using System.Text.Json.Serialization;
using Dapper;
using PriceFeed.BLL.Services;
using PriceFeed.Connectors;
using PriceFeed.DAL;
using PriceFeed.DAL.Interfaces;
using PriceFeed.DAL.Repositories;
using PriceFeed.migrations;
using PriceFeed.Services;

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
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

DefaultTypeMap.MatchNamesWithUnderscores = true;
builder.Services.AddSingleton<UnitOfWork>();

builder.Services.AddSingleton<IPricesRepository, PricesRepository>();
builder.Services.AddSingleton<PricesService>();

builder.Services.AddHostedService<ExampleConnector>();
builder.Services.AddHostedService<KafkaConsumerService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();