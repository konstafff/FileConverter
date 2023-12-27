using FileConverter.Bll;
using FileConverter.Bll.FileConverters;
using FileConverter.DataLayer;
using FileConverter.DataLayer.Enums;

namespace FileConverter.Api.HostedServices;

public class ConvertFileHostedService(IServiceProvider services) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = services.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
        var fileConverter = scope.ServiceProvider.GetRequiredService<IFileConverter>();
        var convertFileProcess = scope.ServiceProvider.GetRequiredService<ConvertFileQueue>();

        var files = unitOfWork.FileRepository.Filter(x => x.Status == FileStatus.New)
            .ToArray();

        var tasks = new Task[files.Length];

        for (var i = 0; i < files.Length; i++)
        {
            tasks[i] = fileConverter.Convert(files[i]);
        }

        Task.WaitAll(tasks);

        while (true)
        {
            if (convertFileProcess.QueueFiles.IsEmpty)
            {
                await Task.Delay(2000, stoppingToken);
                continue;
            }

            while (!convertFileProcess.QueueFiles.IsEmpty)
            {
                if (!convertFileProcess.QueueFiles.TryDequeue(out var nextFile))
                    continue;

                await fileConverter.Convert(nextFile);
            }
        }
    }
}