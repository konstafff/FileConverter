using System.Collections.Concurrent;
using FileConverter.DataLayer.Model;

namespace FileConverter.Bll;

public class ConvertFileQueue
{
    public ConcurrentQueue<FileModel> QueueFiles = new ();
}