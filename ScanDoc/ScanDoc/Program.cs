using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using ScanDoc.Logger;

public class Program
{
    private static void Main(string[] args)
    {
        ILogger logging = new Logger();

        while (true) // Infinite loop
        {
            try
            {
                IConfigurationBuilder builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json", optional: true, reloadOnChange: true);

                IConfiguration configuration = builder.Build();

                string sourceDirectory = configuration["SourceDirectory"];
                string destinationDirectory = configuration["DestinationDirectory"];

                // Define the pattern to match the desired format and capture groups
                string pattern = @"^(\d+)-(\d+)-([^.-]+)-(\d+)\.(\w+)$";

                // Recursively search for all subdirectories.
                string[] allDirectories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);

                foreach (string directory in allDirectories)
                {
                    try
                    {
                        // Find all files within the current subdirectory.
                        string[] filesInDirectory = Directory.GetFiles(directory);

                        foreach (string filePath in filesInDirectory)
                        {
                            string fileName = Path.GetFileName(filePath);
                            Match match = Regex.Match(fileName, pattern);

                            if (match.Success)
                            {
                                string caseNumber = match.Groups[1].Value;
                                string companyNumber = match.Groups[2].Value;
                                string docName = match.Groups[3].Value;
                                string docNo = match.Groups[4].Value;
                                string extension = match.Groups[5].Value;

                                string newFileName = string.Empty;

                                if (string.IsNullOrEmpty(docName) && string.IsNullOrEmpty(docNo))
                                {
                                    newFileName = $"{companyNumber}{caseNumber}.{extension}";
                                }
                                else if (string.IsNullOrEmpty(docName))
                                {
                                    newFileName = $"{companyNumber}{caseNumber}_{docNo}.{extension}";
                                }
                                else if (string.IsNullOrEmpty(docNo))
                                {
                                    newFileName = $"{companyNumber}{caseNumber}_{docName}.{extension}";
                                }
                                else if (!string.IsNullOrEmpty(docName) && !string.IsNullOrEmpty(docNo))
                                {
                                    newFileName = $"{companyNumber}{caseNumber}_{docName}_{docNo}.{extension}";
                                }

                                string destinationFolder = Path.Combine(destinationDirectory, $"{companyNumber}{caseNumber}");

                                if (!Directory.Exists(destinationFolder))
                                {
                                    Directory.CreateDirectory(destinationFolder);
                                }

                                string newFilePath = Path.Combine(destinationFolder, newFileName);

                                File.Copy(filePath, newFilePath); // Copy the file

                                logging.Log($"Copied: {fileName} to {newFilePath}");

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logging.Log($"Error processing directory: {directory}. Error: {ex.Message}");
                    }
                }

                logging.Log("Processing complete.");
            }
            catch (Exception ex)
            {
                logging.Log($"Error: {ex.Message}");
            }

            // Sleep for a while before the next iteration (e.g., 1 hour)
            System.Threading.Thread.Sleep(TimeSpan.FromHours(1));
            }
        }
    }

