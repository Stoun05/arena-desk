using ArenaDesk.Agent;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options => options.ServiceName = "ArenaDesk Agent");
builder.Services.AddOptions<AgentOptions>()
    .Bind(builder.Configuration.GetSection(AgentOptions.SectionName))
    .Validate(options => Uri.TryCreate(options.ApiBaseUrl, UriKind.Absolute, out _), "ArenaDesk:ApiBaseUrl must be an absolute URL.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.AgentToken) || (options.BootstrapKey?.Length ?? 0) >= 32,
        "Set ArenaDesk:AgentToken or an ArenaDesk:BootstrapKey with at least 32 characters.")
    .ValidateOnStart();

builder.Services.AddHttpClient<ArenaApiClient>((services, client) =>
{
    var options = services.GetRequiredService<IOptions<AgentOptions>>().Value;
    client.BaseAddress = new Uri(options.ApiBaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddSingleton<CommandExecutor>();
builder.Services.AddHostedService<AgentWorker>();

await builder.Build().RunAsync();
