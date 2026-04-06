using DriveCLISync.Services;
using System.CommandLine;

var credentialsPath = Path.Combine(AppContext.BaseDirectory, "client_secret.json");
var tokenStorePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "DriveCLISync", "token"
);

var rootCommand = new RootCommand("DriveCLISync — Google Drive CLI Tool");

//Sync Command
var downloadCommand = new Command("sync", "Download all files from Google Drive");

downloadCommand.SetHandler(async () =>
{
    IGoogleAuthService authService = new GoogleAuthService(credentialsPath, tokenStorePath);
    var driveService = await authService.GetDriveServiceAsync();

    IGoogleDriveService service = new GoogleDriveService(driveService);
    await service.DownloadAllFilesAsync();
});

//Search Command
var searchArgument = new Argument<string>("query", "File or folder name to search for");
var searchCommand = new Command("search", "Search for files or folders in Google Drive");
searchCommand.AddArgument(searchArgument);

searchCommand.SetHandler(async (string query) =>
{
    IGoogleAuthService authService = new GoogleAuthService(credentialsPath, tokenStorePath);
    var driveService = await authService.GetDriveServiceAsync();

    IGoogleDriveService service = new GoogleDriveService(driveService);
    await service.SearchFilesByNameAsync(query);
}, searchArgument);

//Upload Command
var localPathArgument = new Argument<string>("local_path", "Path to the local file to upload");
var drivePathArgument = new Argument<string>("drive_path", "Target folder name in Google Drive");
var uploadCommand = new Command("upload", "Upload a file to Google Drive");
uploadCommand.AddArgument(localPathArgument);
uploadCommand.AddArgument(drivePathArgument);

uploadCommand.SetHandler(async (string localPath, string drivePath) =>
{
    IGoogleAuthService authService = new GoogleAuthService(credentialsPath, tokenStorePath);
    var driveService = await authService.GetDriveServiceAsync();

    IGoogleDriveService service = new GoogleDriveService(driveService);
    await service.UploadAsync(localPath, drivePath);
}, localPathArgument, drivePathArgument);

//Register Commands
rootCommand.AddCommand(downloadCommand);
rootCommand.AddCommand(searchCommand);
rootCommand.AddCommand(uploadCommand);

await rootCommand.InvokeAsync(args);
