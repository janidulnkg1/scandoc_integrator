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

        try
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            string sourceDirectory = configuration["SourceDirectory"];
            string destinationDirectory = configuration["DestinationDirectory"];

            // Pattern to match the desired format and capture groups
            string pattern = @"^(\d{9})-(\d{2})-(.*)-(\d{3})\.(.*)$";


            // Recursively search for all files in subdirectories of the source directory.
            string[] allDirectories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);

            foreach (string directory in allDirectories)
            {
                try
                {
                    string fileName = Path.GetFileName(directory);
                    Match match = Regex.Match(fileName, pattern);

                    if (match.Success)
                    {
                        string caseNumber = match.Groups[1].Value;
                        string companyNumber = match.Groups[2].Value;
                        string docName = match.Groups[3].Success ? match.Groups[3].Value : "";
                        string docNo = match.Groups[4].Success ? match.Groups[4].Value : "";
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

                        File.Move(directory, newFilePath);
                        logging.Log($"Moved: {fileName} to {newFilePath}");
                    }
                }
                catch (Exception ex)
                {
                    logging.Log($"Error processing file: {directory}. Error: {ex.Message}");
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