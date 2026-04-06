namespace DriveCLISync.Services
{
    public interface IGoogleDriveService
    {
        Task DownloadAllFilesAsync();
        Task SearchFilesByNameAsync(string query);
        Task UploadAsync(string localPath, string drivePath);
    }
}
