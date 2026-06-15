using api.application.Services.Interface;
using api.util;

namespace api.application.Services.Implement
{
    public class FileServices(IWebHostEnvironment host, ILogger<FileServices> logger) : IFileServices
    {
        private const string LocalPath = "images";

        private static string GetFileExtension(IFormFile filename) => Path.GetExtension(filename.FileName);

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

        public async Task<string?> SaveFile(IFormFile file, EnImageType type)
        {
            logger.LogInformation("start saving one image");
            var filePath = Path.Combine(host.ContentRootPath, LocalPath, type.ToString());
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

                var fileFullName = Path.Combine(filePath, ClsUtil.GenerateGuid() + GetFileExtension(file));

                await using (var stream = new FileStream(fileFullName, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                logger.LogInformation("end saving one image");

                return fileFullName.Split("images")[1];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from saving image");
                return null;
            }
        }

        public async Task<List<string>?> SaveFile(List<IFormFile> file, EnImageType type)
        {
            logger.LogInformation("start saving list of image to local api storage");
            List<string> images = [];

            foreach (var t in file)
            {
                var path = await SaveFile(t, type);
                if (path is null)
                {
                    DeleteFile(images);
                    return null;
                }

                images.Add(path);
            }
            logger.LogInformation("end saving list of image to local api storage");

            return images;
        }


        public bool DeleteFile(string filePath)
        {
            logger.LogInformation("start deleting image");
            try
            {
                var newFilPath = ClsUtil.RemoveAdditionalPath(filePath);
                var fileRealPath = Path.Combine(host.ContentRootPath, "images/", newFilPath);
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

        public bool DeleteFile(List<string> filePaths)
        {
            try
            {
                foreach (var filePath in filePaths)
                {
                    DeleteFile(filePath);
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