using Google.Apis.Drive.v3;

namespace DriveCLISync.Services
{
    public interface IGoogleAuthService
    {
        Task<DriveService> GetDriveServiceAsync();
    }
}
