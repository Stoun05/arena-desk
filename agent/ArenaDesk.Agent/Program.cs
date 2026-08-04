using ArenaDesk.Agent;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options => options.ServiceName = "ArenaDesk Agent");
builder.Services
    .AddOptions<AgentOptions>()
    .Bind(builder.Configuration.GetSection(AgentOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddSingleton<AgentCommandProcessor>();
builder.Services.AddHostedService<AgentWorker>();

await builder.Build().RunAsync();
