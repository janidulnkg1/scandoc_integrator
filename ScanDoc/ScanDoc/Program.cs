using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using ScanDoc.Logger;

internal class Program
{
    private static void Main(string[] args)
    {
        ILogger logging = new Logger();

        try
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            string sourceDirectory = configuration["SourceDirectory"];
            string destinationDirectory = configuration["DestinationDirectory"];

            // Check if the source directory exists.
            if (!Directory.Exists(sourceDirectory))
            {
                logging.Log($"Source directory does not exist: {sourceDirectory}");
                return; // Exit the program if the source directory is missing.
            }

            string pattern = @"^(\d{9}-\d{2})-(.+)\.(.+)$"; // Pattern to match the desired format

            // Recursively search for all files in subdirectories of the source directory.
            string[] allFiles = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

            foreach (string filePath in allFiles)
            {
                try
                {
                    string fileName = Path.GetFileName(filePath);
                    Match match = Regex.Match(fileName, pattern);

                    if (match.Success)
                    {
                        string caseNumber = match.Groups[1].Value;
                        string companyNumber = match.Groups[2].Value;
                        string extension = match.Groups[3].Value;
                        string newFileName = $"{companyNumber}{caseNumber}_{Path.GetFileNameWithoutExtension(fileName)}.{extension}";
                        string destinationFolder = Path.Combine(destinationDirectory, $"{companyNumber}{caseNumber}");

                        if (!Directory.Exists(destinationFolder))
                        {
                            Directory.CreateDirectory(destinationFolder);
                        }

                        string newFilePath = Path.Combine(destinationFolder, newFileName);

                        File.Move(filePath, newFilePath);
                        logging.Log($"Moved: {filePath} to {newFilePath}");
                    }
                }
                catch (Exception ex)
                {
                    logging.Log($"Error processing file: {filePath}. Error: {ex.Message}");
                }
            }

            logging.Log("Processing complete.");
        }
        catch (Exception ex)
        {
            logging.Log($"Error: {ex.Message}");
        }
    }
}
