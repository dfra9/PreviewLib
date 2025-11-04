using APIWebPreview.Services;
using Microsoft.Extensions.Options;

namespace APIWebPreview.EndPoints
{
    public static class FileEndpoints
    {
        public static async Task<IResult> UploadFile(
            HttpRequest request,
            IOptions<StorageOptions> storageOpt,
            LibreOfficeConvert converter,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("FileEndpoints");

            if (!request.HasFormContentType)
                return Results.BadRequest("Form-data required.");

            // cek dulu converter-nya bisa jalan apa nggak
            if (!converter.CanRun())
            {
                logger.LogError("LibreOffice tidak ditemukan di path/docker yang dikonfigurasi.");
                return Results.Problem(
                    detail: "LibreOffice tidak ditemukan. Periksa appsettings.json → LibreOffice:(UseDocker / SofficePath).",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            var form = await request.ReadFormAsync();
            var file = form.Files.GetFile("file");
            if (file is null || file.Length == 0)
                return Results.BadRequest("No file uploaded.");

            var storage = storageOpt.Value;
            var baseDir = storage.BasePath;
            var uploadDir = Path.Combine(baseDir, storage.Uploads);
            var previewDir = Path.Combine(baseDir, storage.Previews);
            Directory.CreateDirectory(uploadDir);
            Directory.CreateDirectory(previewDir);

            var id = Guid.NewGuid().ToString("N");
            var originalName = file.FileName;
            var ext = Path.GetExtension(originalName).ToLowerInvariant();
            var savedName = $"{id}{ext}";
            var savedPath = Path.Combine(uploadDir, savedName);

            using (var fs = File.Create(savedPath))
                await file.CopyToAsync(fs);

            string previewPath;
            var outDir = Path.Combine(previewDir, id);

            logger.LogInformation("Saved file: {SavedPath}", savedPath);
            logger.LogInformation("Output directory: {OutDir}", outDir);

            try
            {
                // Konversi semua file ke PDF karena lebih reliable
                previewPath = await converter.ConvertToPdfAsync(savedPath, outDir);
                logger.LogInformation("Preview created: {PreviewPath}", previewPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Conversion failed for file: {SavedPath}", savedPath);
                return Results.Problem(
                    detail: $"Konversi gagal: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            // bikin URL buat FE
            var rel = Path.GetRelativePath(storage.BasePath, previewPath).Replace("\\", "/");
            var publicUrl = $"/static/{rel}";

            logger.LogInformation("Public URL: {PublicUrl}", publicUrl);

            return Results.Ok(new
            {
                id,
                name = originalName,
                ext,
                previewUrl = publicUrl,
                type = "pdf" // Semua file dikonversi ke PDF
            });
        }

        public static IResult GetPreviewInfo(string id)
        {
            return Results.NotFound("Not implemented.");
        }
    }
}
