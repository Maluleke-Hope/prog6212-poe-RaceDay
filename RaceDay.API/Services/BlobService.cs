using System.IO;

namespace RaceDay.API.Services;

public interface IBlobService
{
    Task<string> UploadAsync(IFormFile file);
}

public class BlobService : IBlobService
{
    public async Task<string> UploadAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/images/{uniqueFileName}";
    }
}