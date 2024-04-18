using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading.Tasks;


namespace MinecraftServerModInstaller
{
    internal class Program
    {
        public static string configFilePath;
        public static string downloadFolder = "downloads";
        public static JArray downloadLinks;
        static void Main(string[] args)
        {
            // Construct the download path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);           
            string downloadFolderPath = Path.Combine(appDataPath, ".minecraft", "mods");
            downloadFolder = downloadFolderPath;

            IsThisCorrectGithubLink();
        }

        private static void IsThisCorrectGithubLink ()
        {
            Console.Clear();
            configFilePath = "config.json";
            string configJson = File.ReadAllText(configFilePath);
            JObject config = JObject.Parse(configJson);
            string githubUrl = (string)config["github_url"];
            Console.WriteLine("Is this the correct github json url? " + githubUrl);
            Console.WriteLine("[y/n]");
            string answer = Console.ReadLine();

            switch (answer.ToLower())
            {
                case "y":
                    IsThisCorrectDownloadPath();
                    break;
                case "n":
                    Console.WriteLine("Change the github link in config.json");
                    break;
                default:
                    Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                    break;
            }
        }

        private static void IsThisCorrectDownloadPath ()
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

        private static void ChangeDownloadPath()
        {
            Console.Clear();
            Console.WriteLine("Write down downloadFolder Path");
            string answer = Console.ReadLine();
            downloadFolder = answer;
            IsThisCorrectDownloadPath();
        }

        private static void DoYouClearDownloadFolder()
        {
            Console.Clear();
            Console.WriteLine("Do you want to clear the download folder?");
            Console.WriteLine("[y/n]");
            string answer = Console.ReadLine();
            if(answer.ToLower() == "y")
            {
                Console.WriteLine("Are you sure?");
                Console.WriteLine("[y/n]");
                string answerSecond = Console.ReadLine();
                if(answerSecond.ToLower() == "y")
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

        private static void ClearDownloadFolder(string folderPath)
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

        private static void DoYouDownload()
        {
            Console.Clear();
            Console.WriteLine("Do you want to download these files to? " + downloadFolder);

            string configJson = File.ReadAllText(configFilePath);
            JObject config = JObject.Parse(configJson);            
            string githubUrl = (string)config["github_url"];

            WebClient client = new WebClient();
            string jsonContent = client.DownloadString(githubUrl);
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


        private static void Download()
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
