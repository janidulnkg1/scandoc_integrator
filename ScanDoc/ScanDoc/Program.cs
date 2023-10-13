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
        int filesCopiedCount = 0;


        try
            {
                IConfigurationBuilder builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json", optional: true, reloadOnChange: true);

                IConfiguration configuration = builder.Build();

                string sourceDirectory = configuration["SourceDirectory"];
                string destinationDirectory = configuration["DestinationDirectory"];

                // Defining the patterns to match the desired formats and capture groups in the Initial Source Folder files
                string pattern1 = @"^(\d{9})-(\d{2})-([^-]+)-(\d{3})\.(\w+)$"; // {case no}-{company no}-{doc name}-{doc no eg:001}.{extension}
                string pattern2 = @"^(\d{9})-(\d{2})-([^.]+)\.(\w+)$"; // {case no}-{company no}-{doc name}.{extension}
                string pattern3 = @"^(\d{9})-(\d{2})-(\d{3})\.(\w+)$"; // {case no}-{company no}-{doc no eg:001}.{extension}
                string pattern4 = @"^(\d{9})-(\d{2})\.(\w+)$"; // {case no}-{company no}.{extension}

                // Recursively search for all subdirectories.
                string[] allDirectories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);

                foreach (string directory in allDirectories)
                {
                    try
                    {
                        // Find all files within the current subdirectory.
                        string[] filesInDirectory = Directory.GetFiles(directory);

                        // Dictionary to keep track of encountered file names and their counts
                        Dictionary<string, int> encounteredFileNames = new Dictionary<string, int>();

                        foreach (string filePath in filesInDirectory)
                        {
                            string fileName = Path.GetFileName(filePath);
                            Match? match = null;
                            string? newFileName = null;
                            string? baseFileName = null;

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

                                baseFileName = $"{companyNumber}{caseNumber}_{docName}";

                                if (encounteredFileNames.ContainsKey(baseFileName))
                                {
                                    // If there's a duplicate, increment the count in 3 digits (001 etc)
                                    int fileCount = encounteredFileNames[baseFileName];
                                    encounteredFileNames[baseFileName] = fileCount + 1;
                                    newFileName = $"{baseFileName}-{fileCount:D3}.{extension}";
                                }
                                else
                                {
                                    // If it's the first occurrence, marking it as 1
                                    encounteredFileNames[baseFileName] = 1;
                                    newFileName = baseFileName;
                                }
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

                                baseFileName = $"{companyNumber}{caseNumber}";

                                if (encounteredFileNames.ContainsKey(baseFileName))
                                {
                                    // If there's a duplicate, increment the count in 3 digits (001 etc)
                                    int fileCount = encounteredFileNames[baseFileName];
                                    encounteredFileNames[baseFileName] = fileCount + 1;
                                    newFileName = $"{baseFileName}-{fileCount:D3}.{extension}";
                                }
                                else
                                {
                                    // If it's the first occurrence, mark it as 1
                                    encounteredFileNames[baseFileName] = 1;
                                    newFileName = baseFileName;
                                }
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
                                logging.Log($"Copied: {fileName} to {newFilePath}");
                                filesCopiedCount++; //Incrementing count
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logging.Log($"Error processing directory: {directory}. Error: {ex.Message}");
                    }
                }

                logging.Log("Processing complete.");
                Console.WriteLine("Peocessing Completed Successfully!");
                Console.WriteLine($" Total Files Copied: {filesCopiedCount}");

        }
        catch (Exception ex)
            {
                logging.Log($"Error: {ex.Message}");
            }

            
          
    }
}
