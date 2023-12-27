using FileConverter.Bll.Dto;

namespace FileConverter.Bll.Services.Interfaces;

public interface IFileService
{
    Task<FileStatusResponseDio> AddNewFile(byte[] fileData, string fileName, Guid sessionKey, Guid fileId);
    Task<FileStatusResponseDio> GetFileStatus(Guid sessionKey, Guid fileId);
    Task<FileResponseDto> GetFile(Guid sessionKey, Guid fileId);
    Task DeleteFile(Guid sessionKey, Guid fileId);
}
