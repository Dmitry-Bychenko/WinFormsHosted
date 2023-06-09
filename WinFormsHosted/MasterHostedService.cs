using Microsoft.Extensions.Hosting;

namespace WinFormsHosted;

internal class MasterHostedService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());

        return Task.CompletedTask;
    }
}