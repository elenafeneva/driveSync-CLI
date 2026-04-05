using DriveCLISync.Services;


Console.WriteLine("Starting DriveClISync...");
Console.WriteLine("Authenticating with Google Drive...");
var tokenFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "DriveCLISync",
    "token"
);

var authService = new GoogleAuthService("client_secret.json", tokenFolder);
var driveService = await authService.GetDriveServiceAsync();

Console.WriteLine("Authentication successful.");
Console.WriteLine("Send command for action. Commands: ");
Console.WriteLine("sync -> Downloads al files from the connected Google Drive to a local Downloads directory.");
Console.WriteLine("search -> Searches for files or folders by name.");
Console.WriteLine("upload -> Uploads a file from the local file system to a specific folder path in Google Drive.");
var command = Console.ReadLine()?.Trim().ToLower();

var downloadService = new GoogleDriveService(driveService);
if (command == "sync")
{
    await downloadService.DownloadAllFilesAsync();
}
else if (command == "search")
{
    Console.WriteLine("Enter the name of the file or folder to search for:");
    var fileName = Console.ReadLine()?.Trim();
    if(!string.IsNullOrWhiteSpace(fileName))
        await downloadService.SearchFilesByNameAsync(fileName);
}
