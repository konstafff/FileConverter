﻿using System.Text;
using FileConverter.Bll.Dto;
using FileConverter.Bll.Services.Interfaces;
using FileConverter.DataLayer;
using FileConverter.DataLayer.Enums;
using FileConverter.DataLayer.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileConverter.Bll.Services;

public class FileService(ILogger<FileService> logger, UnitOfWork unitOfWork, 
    IOptions<AppSettings> options, ConvertFileQueue convertFileQueue) : IFileService
{
    private readonly AppSettings _appSettings = options.Value;

    public async Task<FileStatusResponseDio> AddNewFile(byte[] fileData, string fileName, Guid sessionKey, Guid fileId)
    {
        logger.LogInformation($"Start {nameof(AddNewFile)}: {fileName}");

        var currentPath = Path.Combine(_appSettings.PathToWorkDir, sessionKey.ToString(), fileId.ToString());

        if (!Directory.Exists(currentPath))
            Directory.CreateDirectory(currentPath);

        await File.WriteAllTextAsync(Path.Combine(currentPath, fileName), Encoding.UTF8.GetString(fileData));

        var fileModel = new FileModel()
        {
            Status = FileStatus.New,
            FileName = fileName,
            SessionKey = sessionKey,
            FileId = fileId
        };

        await unitOfWork.FileRepository.CreateAsync(fileModel);

        convertFileQueue.QueueFiles.Enqueue(fileModel);

        logger.LogInformation($"End {nameof(AddNewFile)}: {fileName}");

        return new FileStatusResponseDio(fileId, string.Empty, FileStatus.New);
    }

    public async Task<FileStatusResponseDio> GetFileStatus(Guid sessionKey, Guid fileId)
    {
        var model = await unitOfWork.FileRepository.GetFile(sessionKey, fileId);

        if (model is null)
            throw new ArgumentException("File not found");

        return new FileStatusResponseDio(model.FileId, model.ResultFileName, model.Status);
    }

    public async Task<FileResponseDto> GetFile(Guid sessionKey, Guid fileId)
    {
        var fileStatus = await GetFileStatus(sessionKey, fileId);

        if (string.IsNullOrEmpty(fileStatus.ResultFileName))
            throw new ArgumentException("File not found");
        
        logger.LogInformation($"Start {nameof(GetFile)}: {fileStatus.ResultFileName}");

        var currentPath = Path.Combine(_appSettings.PathToWorkDir, sessionKey.ToString(), fileId.ToString(), fileStatus.ResultFileName);

        if (!File.Exists(currentPath))
            throw new ArgumentException("File not found");

        var file = await File.ReadAllBytesAsync(currentPath);

        logger.LogInformation($"End {nameof(GetFile)}: {fileStatus.ResultFileName}");

        return new FileResponseDto(fileStatus.ResultFileName, file);
    }

    public async Task DeleteFile(Guid sessionKey, Guid fileId)
    {
        var currentPath = Path.Combine(_appSettings.PathToWorkDir, sessionKey.ToString(), fileId.ToString());

        if (Directory.Exists(currentPath))
        {
            Directory.Delete(currentPath, true);
        }

        var fileModel = await unitOfWork.FileRepository.Filter(x => x.FileId == fileId && x.SessionKey == sessionKey).FirstOrDefaultAsync();

        if (fileModel is not null)
        {
            await unitOfWork.FileRepository.RemoveAsync(fileModel);
            logger.LogInformation($"{nameof(DeleteFile)}: {fileModel.FileName}");
        }
    }
}