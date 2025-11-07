using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using UserAuth.BLL.Services;
using UserAuth.DAL;
using UserAuth.DAL.Interfaces;
using UserAuth.DAL.Repositories;
using UserAuth.Migrations;
using UserAuth.Validators;

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

builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));
builder.Services.AddScoped<ValidatorFactory>();

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection(nameof(DbSettings)));

DefaultTypeMap.MatchNamesWithUnderscores = true;
builder.Services.AddSingleton<UnitOfWork>();

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<TokenService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run("http://localhost:5034");