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

        while (true) //Continuosly keep Looping
        {
            try
            {
                IConfigurationBuilder builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json", optional: true, reloadOnChange: true);

                IConfiguration configuration = builder.Build();

                string sourceDirectory = configuration["SourceDirectory"];
                string destinationDirectory = configuration["DestinationDirectory"];

                // Define the patterns to match the desired formats and capture groups
                string pattern1 = @"^(\d{9})-(\d{2})-([^-]+)-(\d{3})\.(\w+)$";
                string pattern2 = @"^(\d{9})-(\d{2})-([^.]+)\.(\w+)$";
                string pattern3 = @"^(\d{9})-(\d{2})-(\d{3})\.(\w+)$";
                string pattern4 = @"^(\d{9})-(\d{2})\.(\w+)$";

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
                            Match match = null;
                            string newFileName = null;

                            if ((match = Regex.Match(fileName, pattern1)).Success)
                            {
                                string caseNumber = match.Groups[1].Value;
                                string companyNumber = match.Groups[2].Value;
                                string docName = match.Groups[3].Value;
                                string docNo = match.Groups[4].Value;
                                string extension = match.Groups[5].Value;

                                newFileName = $"{companyNumber}{caseNumber}_{docName}_{docNo}.{extension}";
                            }
                            else if ((match = Regex.Match(fileName, pattern2)).Success)
                            {
                                string caseNumber = match.Groups[1].Value;
                                string companyNumber = match.Groups[2].Value;
                                string docName = match.Groups[3].Value;
                                string extension = match.Groups[4].Value;

                                newFileName = $"{companyNumber}{caseNumber}_{docName}.{extension}";
                            }
                            else if ((match = Regex.Match(fileName, pattern3)).Success)
                            {
                                string caseNumber = match.Groups[1].Value;
                                string companyNumber = match.Groups[2].Value;
                                string docNo = match.Groups[3].Value;
                                string extension = match.Groups[4].Value;

                                newFileName = $"{companyNumber}{caseNumber}_{docNo}.{extension}";
                            }
                            else if ((match = Regex.Match(fileName, pattern4)).Success)
                            {
                                string caseNumber = match.Groups[1].Value;
                                string companyNumber = match.Groups[2].Value;
                                string extension = match.Groups[3].Value;

                                newFileName = $"{companyNumber}{caseNumber}.{extension}";
                            }

                            if (newFileName != null)
                            {
                                string caseNumber = match.Groups[1].Value;
                                string companyNumber = match.Groups[2].Value;
                                string destinationFolder = Path.Combine(destinationDirectory, $"{companyNumber}{caseNumber}");

                                if (!Directory.Exists(destinationFolder))
                                {
                                    Directory.CreateDirectory(destinationFolder);
                                }

                                string newFilePath = Path.Combine(destinationFolder, newFileName);

                                File.Copy(filePath, newFilePath);
                                logging.Log($"Copy: {fileName} to {newFilePath}");
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

            
            System.Threading.Thread.Sleep(1000); 
        }
    }
}
