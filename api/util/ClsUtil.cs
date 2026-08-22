namespace api.util;

public static class ClsUtil
{
    extension (IFormFile data) {
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            data.CopyToAsync(stream);

            return stream.ToArray();
        }
    }
}