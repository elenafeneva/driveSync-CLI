# DriveCLISync

.NET CLI tool for Google Drive using OAuth 2.0 authentication, file synchronization, and thread-safe operations. This command-line inteface supprot downloading, searching, and uploading files directly from the terminal. 

## Setup: client_secret.json

This file contains a sensitive data and is excluded from the project using `.gitignore`. Before running the project place it manually. To do that, you need to follow these steps: 

1. Go to [Google Cloud Console](https://console.cloud.google.com)
2. Choose **APIs & Services → Credentials**
3. Create an Auth 2.0 client
4. Rename it to `client_secret.json`
5. Place the file into the project. 

A `client_secret.example.json` file is included in the project to show the required fields. 


## How to Build

Navigate to the project folder and run: 
```bash
cd DriveCLISync/DriveCLISync
dotnet build
```

## How to Run

From the project folder, use `dotnet run --` followed by the command:

```bash
dotnet run -- <command> [arguments]
```

## First Run 

On the first run the app will open a browser window asing you to log in into your Google account and grant access to your Google Drive. After compliting this, the token is saved locally and you will not be asked again for it.

The token will be stored at: 

 ```
C:\Users\<you>\AppData\Roaming\DriveCLISync\token\
```
Make sure your Google account is added as a **Test User** in the Google Cloud Console. 

## Available Commands

### `sync` — Download all files from Google Drive

Downloads all files from your Google Drive to a local `Downloads/` directory next to the executable.

```bash
dotnet run -- sync
```
### `search <query>` — Search for files or folders by name

Searches Google Drive for files or folders matching the given name and shows whether each result has been downloaded locally.

### `upload <local_path> <drive_folder>` — Upload a file to Google Drive

Uploads a local file to a specified folder in Google Drive. If the target folder does not exist, it will be created automatically.

