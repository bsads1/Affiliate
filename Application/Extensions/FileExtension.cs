using Spark.Library.Extensions;

namespace Affiliate.Application.Extensions;

public static class FileExtension
{
    public static bool Check(this IFormFile? file)
    {
        return file is { Length: > 0 } && !string.IsNullOrEmpty(file.FileName);
    }

    public static async Task<(bool HasData, string Path)> SaveAsync(this IFormFile? file,
        DateTime? date = null, bool isChangeName = true, string customPath = "", string customName = "")
    {
        if (!file.Check())
            return (false, "");
        (bool HasData, string Path) imageData = (false, "");
        date ??= DateTime.Now;
        var uploadPath = "wwwroot/uploads/" + date.Value.Year + "/" + date.Value.Month + "/" + date.Value.Day;
        if (!string.IsNullOrEmpty(customPath))
        {
            uploadPath = customPath;
        }

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        try
        {
            var fileName = file.FileName;
            if (isChangeName)
            {
                fileName = file.FileName.ToSlug();
            }

            if (!string.IsNullOrWhiteSpace(customName))
            {
                fileName = customName;
            }

            var filePath = Path.Combine(uploadPath, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            imageData.HasData = true;
            imageData.Path = filePath.GetRelativePath();
        }
        catch (Exception e)
        {
            imageData.HasData = false;
        }

        return imageData;
    }
}