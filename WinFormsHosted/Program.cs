using System.Net;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly.Extensions.Http;
using Polly;

namespace WinFormsHosted;

public static class Program {
  private static readonly Lazy<IHost> LazyHost = new(BuildHost);

  private static readonly Lazy<Settings> LazySettings = new(() => Services.GetService<Settings>()!);

  static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() {
    return HttpPolicyExtensions
      .HandleTransientHttpError()
      .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound || msg.StatusCode == HttpStatusCode.TooManyRequests)
      .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) + Random.Shared.NextDouble() * 0.2));
  }

  private static void BuildHttpClient(IServiceCollection services) {
    services
      .AddHttpClient("STANDARD", client => {
        client.DefaultRequestHeaders.Add("User-Agent", "AssemblyName/version");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
      })
      .ConfigurePrimaryHttpMessageHandler(() => {
        HttpClientHandler handler = new HttpClientHandler();

        handler.CookieContainer = new CookieContainer();

        return handler;
      })
      .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
      .AddPolicyHandler(GetRetryPolicy());
  }

  private static IHost BuildHost() {
    string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? "";
    string prefix = Assembly.GetEntryAssembly()?.GetName().Name ?? "";

    // https://code-maze.com/using-httpclientfactory-in-asp-net-core-applications/
    // https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests

    return new HostBuilder()
        .ConfigureHostConfiguration(configHost => {
          configHost.SetBasePath(basePath);
          configHost.AddJsonFile("HostSettings.json", true);
          configHost.AddEnvironmentVariables(prefix);
          configHost.AddCommandLine(Environment.GetCommandLineArgs());
        })
        .ConfigureAppConfiguration((hostContext, configApp) => {
          configApp.SetBasePath(basePath);
          configApp.AddEnvironmentVariables(prefix);
          configApp.AddJsonFile("AppSettings.json", true);
          configApp.AddJsonFile($"AppSettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
          configApp.AddCommandLine(Environment.GetCommandLineArgs());
        })
        .ConfigureServices((hostContext, services) => {
          services.AddLogging();
          
          services.Configure<Settings>(hostContext.Configuration.GetSection("Application"));

          StartUp.Configure(services);

          BuildHttpClient(services);

          services.AddHostedService<MasterHostedService>();
        })
        .ConfigureLogging((hostContext, configLogging) => {
          configLogging.AddDebug();
          configLogging.AddConsole();
        })
        .Build();
  }

  /// <summary>
  ///     Host
  /// </summary>
  public static IHost Host => LazyHost.Value;

  /// <summary>
  ///     Services
  /// </summary>
  public static IServiceProvider Services => Host.Services;

  /// <summary>
  ///     Logger
  /// </summary>
  public static ILogger<T> Logger<T>() {
    return Host.Services.GetService<ILogger<T>>() ?? NullLogger<T>.Instance;
  }

  /// <summary>
  ///     Settings
  /// </summary>
  public static Settings Settings => LazySettings.Value;

  /// <summary>
  ///     Http Client
  /// </summary>
  public static HttpClient Client(string? name = default) {
    var factory = Services.GetService<IHttpClientFactory>();

    return factory!.CreateClient(name?.Trim().ToUpper() ?? "STANDARD");
  }

  [STAThread]
  private static async Task Main() {
    await Host.StartAsync();
  }
}