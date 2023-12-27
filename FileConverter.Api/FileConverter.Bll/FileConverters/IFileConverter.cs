using FileConverter.DataLayer.Model;

namespace FileConverter.Bll.FileConverters;

public interface IFileConverter
{
    Task Convert(FileModel file);
}