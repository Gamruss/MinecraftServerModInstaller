using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace MinecraftServerModInstaller
{
    public class MinecraftInstaller
    {
        private string downloadFolder = "downloads";
        private JArray downloadLinks;
        private string linkToJson;

        public void Start(string[] args)
        {
            // Construct the download path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string downloadFolderPath = Path.Combine(appDataPath, ".minecraft", "mods");
            downloadFolder = downloadFolderPath;

            EnterGithubLink();
        }

        private void EnterGithubLink()
        {
            Console.Clear();
            Console.WriteLine("Please enter rawgithub link to json");
            string answer = Console.ReadLine();
            linkToJson = answer;
            IsThisCorrectGithubLink();
        }

        private void IsThisCorrectGithubLink()
        {
            Console.Clear();
            Console.WriteLine("Is this the correct github json url? " + linkToJson);
            Console.WriteLine("[y/n]");
            string answer = Console.ReadLine();

            switch (answer.ToLower())
            {
                case "y":
                    IsThisCorrectDownloadPath();
                    break;
                case "n":
                    EnterGithubLink();
                    break;
                default:
                    Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                    break;
            }
        }

        private void IsThisCorrectDownloadPath()
        {
            Console.Clear();
            Console.WriteLine(downloadFolder);
            Console.WriteLine("Is this the correct download path? " + downloadFolder);
            Console.WriteLine("[y/n]");
            string answer = Console.ReadLine();

            switch (answer.ToLower())
            {
                case "y":
                    DoYouClearDownloadFolder();
                    break;
                case "n":
                    ChangeDownloadPath();
                    break;
                default:
                    Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                    break;
            }
        }

        private void ChangeDownloadPath()
        {
            Console.Clear();
            Console.WriteLine("Write down downloadFolder Path");
            string answer = Console.ReadLine();
            downloadFolder = answer;
            IsThisCorrectDownloadPath();
        }

        private void DoYouClearDownloadFolder()
        {
            Console.Clear();
            Console.WriteLine("Do you want to clear the download folder?");
            Console.WriteLine("[y/n]");
            string answer = Console.ReadLine();
            if (answer.ToLower() == "y")
            {
                Console.WriteLine("Are you sure?");
                Console.WriteLine("[y/n]");
                string answerSecond = Console.ReadLine();
                if (answerSecond.ToLower() == "y")
                {
                    ClearDownloadFolder(downloadFolder);
                }
                else if (answerSecond == "n")
                {
                    DoYouDownload();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                }

            }
            else if (answer == "n")
            {
                DoYouDownload();
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
            }
        }

        private void ClearDownloadFolder(string folderPath)
        {
            try
            {
                // Get all files in the directory
                string[] files = Directory.GetFiles(folderPath);

                // Delete each file
                foreach (string file in files)
                {
                    File.Delete(file);
                    Console.WriteLine($"Deleted: {file}");
                }

                Console.WriteLine("Download folder cleared successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clear download folder. Error: {ex.Message}");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            DoYouDownload();

        }

        private void DoYouDownload()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("Do you want to download these files to? " + downloadFolder);

                WebClient client = new WebClient();
                string jsonContent = client.DownloadString(linkToJson);
                JObject jsonData = JObject.Parse(jsonContent);
                downloadLinks = (JArray)jsonData["download_links"];

                // Get the GitHub URL from the configuration


                foreach (var downloadLink in downloadLinks)
                {
                    string url = downloadLink.ToString();
                    Console.WriteLine(url);
                }

                Console.WriteLine("[y/n]");
                string answer = Console.ReadLine();


                switch (answer.ToLower())
                {
                    case "y":
                        Download();
                        break;
                    case "n":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                        break;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }

        }


        private void Download()
        {
            Console.Clear();
            using (WebClient client = new WebClient())
            {
                try
                {
                    // Download each file
                    foreach (var downloadLink in downloadLinks)
                    {
                        string url = downloadLink.ToString();
                        string fileName = Path.GetFileName(url);
                        string filePath = Path.Combine(downloadFolder, fileName);

                        client.DownloadFile(url, filePath);
                        Console.WriteLine($"Downloaded: {url}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download files. Error: {ex.Message}");
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
