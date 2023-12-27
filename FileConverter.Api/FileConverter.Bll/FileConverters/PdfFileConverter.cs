using FileConverter.DataLayer;
using FileConverter.DataLayer.Enums;
using FileConverter.DataLayer.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PuppeteerSharp;

namespace FileConverter.Bll.FileConverters;

public class PdfFileConverter(IOptions<AppSettings> options, ILogger<PdfFileConverter> logger, UnitOfWork unitOfWork) : IFileConverter
{
    private const string Extension = ".pdf";
    
    private readonly AppSettings _appSettings = options.Value;

    public async Task Convert(FileModel file)
    {
        try
        {
            var currentPath = Path.Combine(_appSettings.PathToWorkDir, file.SessionKey.ToString(), file.FileId.ToString());

            if (!Directory.Exists(currentPath))
                Directory.CreateDirectory(currentPath);

            var fileInfo = new FileInfo(file.FileName);
            var fileName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty) + Extension;

            var fileData = await File.ReadAllTextAsync(Path.Combine(currentPath, file.FileName));
            var downloadResult = await new BrowserFetcher().DownloadAsync();

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = downloadResult.GetExecutablePath()
            });

            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(fileData);
            await page.PdfAsync(Path.Combine(currentPath, fileName));

            file.ResultFileName = fileName;
            file.Status = FileStatus.Completed;

            await unitOfWork.FileRepository.UpdateAsync(file);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message, e);
        }
    }
}