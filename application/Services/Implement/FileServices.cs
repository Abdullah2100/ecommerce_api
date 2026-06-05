using api.application.Services.Interface;
using api.util;

namespace api.application.Services.Implement
{
    public class FileServices(IWebHostEnvironment host) : IFileServices
    {
        private const string LocalPath = "images";

        private static string GetFileExtension(IFormFile filename) => Path.GetExtension(filename.FileName);

        private static bool CreateDirectory(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("this the error from creating file to save image on it " + ex.Message);
                return false;
            }
        }

        public async Task<string?> SaveFile(IFormFile file, EnImageType type)
        {
            var filePath = Path.Combine(host.ContentRootPath, LocalPath, type.ToString());
            try
            {
                if (!Directory.Exists(filePath))
                {
                    if (!CreateDirectory(filePath))
                    {
                        return null;
                    }
                }


                var fileFullName = Path.Combine(filePath, ClsUtil.GenerateGuid() + GetFileExtension(file));

                await using (var stream = new FileStream(fileFullName, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return fileFullName.Split("images")[1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("this the error from saving image to local" + ex.Message);
                return null;
            }
        }

        public async Task<List<string>?> SaveFile(List<IFormFile> file, EnImageType type)
        {
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

            return images;
        }


        public bool DeleteFile(string filePath)
        {
            try
            {
                var newFilPath = ClsUtil.RemoveAdditionalPath(filePath);
                var fileRealPath = Path.Combine(host.ContentRootPath, "images/", newFilPath);
                if (!File.Exists(fileRealPath)) return false;
                File.Delete(fileRealPath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("this the error from delete image  " + ex.Message);
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