using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace DriveCLISync.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly string _credentialsPath;
        private readonly string _tokenStorePath;

        private static readonly string[] Scopes = { DriveService.Scope.Drive };

        public GoogleAuthService(string credentialsPath, string tokenStorePath)
        {
            _credentialsPath = credentialsPath;
            _tokenStorePath = tokenStorePath;
        }

        public async Task<DriveService> GetDriveServiceAsync()
        {
            UserCredential credential;
            using (var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(_tokenStorePath, fullPath: true) // Store tokens in a specific directory
                );
            }

            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "DriveSync-CLI"
            });
        }
    }
}
