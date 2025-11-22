var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server
var sqlServer = builder.AddSqlServer("sql")
    .WithDataVolume()
    .AddDatabase("PersonalFinanceDB");

builder.AddProject<Projects.PersonalFinance_API>("personalfinance-api")
    .WithReference(sqlServer)
    .WithExternalHttpEndpoints();

builder.Build().Run();
