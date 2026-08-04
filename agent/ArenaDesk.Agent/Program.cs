using ArenaDesk.Agent;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options => options.ServiceName = "ArenaDesk Agent");
builder.Services
    .AddOptions<AgentOptions>()
    .Bind(builder.Configuration.GetSection(AgentOptions.SectionName))
    .ValidateDataAnnotations()
    .Validate(
        options => string.Equals(options.CommandExecutionMode, "LogOnly", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(options.CommandExecutionMode, "System", StringComparison.OrdinalIgnoreCase),
        "Agent:CommandExecutionMode must be LogOnly or System.")
    .ValidateOnStart();
builder.Services.AddSingleton<ProcessedCommandStore>();
builder.Services.AddSingleton<WindowsCommandExecutor>();
builder.Services.AddSingleton<AgentCommandProcessor>();
builder.Services.AddSingleton<PlayerScreenChannel>();
builder.Services.AddHostedService(services => services.GetRequiredService<PlayerScreenChannel>());
builder.Services.AddHostedService<AgentWorker>();

await builder.Build().RunAsync();
