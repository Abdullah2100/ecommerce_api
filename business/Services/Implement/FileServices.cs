using business.Services.Interface;
using data.util;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement
{
    public class FileServices(ILogger<FileServices> logger) : IFileServices
    {
        private const string LocalPath = "images";


        private static bool CreateDirectory(string dir, ILogger<FileServices> logger)
        {
            try
            {
                Directory.CreateDirectory(dir);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from create directory to api storage");
                return false;
            }
        }

        public async Task<string?> SaveFile(byte[] file, EnImageType type , string contentRoot)
        {
            logger.LogInformation("start saving one image");
            var filePath = Path.Combine(contentRoot, LocalPath, type.ToString());
            try
            {
                if (!Directory.Exists(filePath))
                {
                    if (!CreateDirectory(filePath, logger: logger))
                    {
                        logger.LogError("Could not create directory " + filePath);
                        return null;
                    }
                }

                var fileFullName = Path.Combine(filePath, ClsUtil.GenerateGuid() + ".jpg");

                await File.WriteAllBytesAsync(fileFullName, file);

                logger.LogInformation("end saving one image");

                return fileFullName.Split("images")[1];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from saving image");
                return null;
            }
        }

        public async Task<List<string>?> SaveFile(List<byte[]> file, EnImageType type , string contentRoot)
        {
            logger.LogInformation("start saving list of image to local api storage");
            List<string> images = [];

            foreach (var t in file)
            {
                var path = await SaveFile(t, type,contentRoot);
                if (path is null)
                {
                    DeleteFile(images,contentRoot);
                    return null;
                }

                images.Add(path);
            }
            logger.LogInformation("end saving list of image to local api storage");

            return images;
        }


        public bool DeleteFile(string filePath,string contentRoot)
        {
            logger.LogInformation("start deleting image");
            try
            {
                var newFilPath = ClsUtil.RemoveAdditionalPath(filePath);
                var fileRealPath = Path.Combine(contentRoot, "images/", newFilPath);
                if (!File.Exists(fileRealPath)) return false;
                File.Delete(fileRealPath);
                logger.LogInformation("end delete image");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "this the error from delete image");
                return false;
            }
        }

        public bool DeleteFile(List<string> filePaths,string contentRoot)
        {
            try
            {
                foreach (var filePath in filePaths)
                {
                    DeleteFile(filePath,contentRoot);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("this the error from delete image  " + ex.Message);
                return false;
            }
        }
    }
}