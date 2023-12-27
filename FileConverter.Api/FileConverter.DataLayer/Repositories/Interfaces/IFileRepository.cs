using FileConverter.DataLayer.Model;

namespace FileConverter.DataLayer.Repositories.Interfaces;

public interface IFileRepository : IAbstractRepository<FileModel>
{
    Task<FileModel?> GetFile(Guid sessionKey, Guid fileId);
}