using data.util;

namespace business.Services.Interface;

public interface IFileServices
{
    Task<string?> SaveFile(byte[] file, EnImageType type,string contentRoot);
    Task<List<string>?> SaveFile(List<byte[]> file, EnImageType type,string contentRoot);

    bool DeleteFile(string filePath,string contentRoot);
    bool DeleteFile(List<string> filePaths, string contentRoot);
}