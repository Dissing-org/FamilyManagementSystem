var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.FamilyManagement_Api>("api");

var web = builder.AddProject<Projects.FamilyManagement_Web>("web")
    .WithReference(api);

builder.Build().Run();
