using Google.Apis.Drive.v3;
using System.Diagnostics;
using File = Google.Apis.Drive.v3.Data.File;

namespace DriveCLISync.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _driveService;

        private readonly string _downloadPath; // Path to download files to
        private int _maxParallelDownloads = 5; // Maximum number of parallel downloads
        // Counters for tracking progress
        private int _totalFiles = 0;
        private int _successCount = 0;
        private int _failedCount = 0;
        private int _alreadyExistsCount = 0;


        public GoogleDriveService(DriveService driveService)
        {
            _driveService = driveService;
            // Creates Downloads folder in the application directory
            _downloadPath = Path.Combine(AppContext.BaseDirectory, "Downloads");
            Directory.CreateDirectory(_downloadPath);
        }

        public async Task SearchFilesByNameAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;
            //Get the files with that name
            var driveFiles = await GetAllFilesAsync(fileName);
            if (!driveFiles.Any())
            {
                Console.WriteLine($"No files found with the name: {fileName}.");
                return;
            }

            var localFiles = GetLocalFiles(fileName);

            foreach (var file in driveFiles)
            {
                var isDownloadedLocaly = localFiles.Contains(file.Name.ToLower());
                Console.WriteLine(isDownloadedLocaly ? $"{file.Name}  [Downloaded]" : $"{file.Name}  [Not Downloaded]");
            }
        }

        private List<string> GetLocalFiles(string fileOrFolderName)
        {
            if (!Directory.Exists(_downloadPath))
                return new List<string>();

            return Directory.GetFileSystemEntries(_downloadPath)
                  .Select(x => Path.GetFileName(x).ToLower())
                  .ToList();
        }

        public async Task DownloadAllFilesAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            Console.WriteLine("Fetching files list from Google Drive...");
            var files = await GetAllFilesAsync(string.Empty);
            _totalFiles = files.Count;

            Console.WriteLine($"Found {_totalFiles} files.\n");
            // Set up parallel options with a maximum number of parallelism
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = _maxParallelDownloads
            };

            await Parallel.ForEachAsync(files, options, async (file, token) =>
            {
                await DownloadFileParallelAsync(file);
            });

            stopwatch.Stop();
            DisplayStatistics(stopwatch.Elapsed);
        }

        private async Task<List<File>> GetAllFilesAsync(string? fileName)
        {
            var allFiles = new List<File>();
            var pageToken = string.Empty;

            do
            {
                var request = _driveService.Files.List();
                request.PageSize = 100;
                request.PageToken = pageToken;

                if (!string.IsNullOrWhiteSpace(fileName))
                    request.Q = $"name contains '{fileName}'";

                var result = await request.ExecuteAsync();
                allFiles.AddRange(result.Files);
                pageToken = result.NextPageToken;

            } while (!string.IsNullOrWhiteSpace(pageToken));

            return allFiles;
        }

        private async Task DownloadFileParallelAsync(File file)
        {
            try
            {
                var filePath = GetFilePath(file.Name);

                if (string.IsNullOrWhiteSpace(filePath))
                    return;

                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                var downloadRequest = _driveService.Files.Get(file.Id);
                await downloadRequest.DownloadAsync(fileStream);

                Interlocked.Increment(ref _successCount);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedCount);
                Console.WriteLine($"Failed: {file.Name} — {ex.Message}");
            }
        }

        private string GetFilePath(string fileName)
        {
            //Get a safe file path 
            var safeName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            var filePath = Path.Combine(_downloadPath, safeName);

            //Check if the file already exists
            if (System.IO.File.Exists(filePath))
            {
                _alreadyExistsCount++;
                return string.Empty;
            }

            return filePath;
        }

        private void DisplayStatistics(TimeSpan elapsed)
        {
            Console.WriteLine("=== DOWNLOAD SUMMARY ===");
            Console.WriteLine("\nDownload completed.");
            Console.WriteLine($"Total files: {_totalFiles}");
            Console.WriteLine($"Already Exist files: {_alreadyExistsCount}");
            Console.WriteLine($"Successful downloads: {_successCount}");
            Console.WriteLine($"Failed downloads: {_failedCount}");
            Console.WriteLine($"Elapsed time: {elapsed.TotalSeconds:F2} seconds");
        }

    }
}
