using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using ScanDoc.Logger;

ILogger logging = new Logger();

try
{
    IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("config.json", optional: true, reloadOnChange: true);

    IConfiguration configuration = builder.Build();

    string sourceDirectory = configuration["SourceDirectory"];
    string destinationDirectory = configuration["DestinationDirectory"];

    if (!Directory.Exists(destinationDirectory))
    {
        Directory.CreateDirectory(destinationDirectory);
    }

    string pattern = @"^(\d{9}-\d{2})_(.+)\..+$"; // Check for files in intended format with any extension

    string[] allFiles = Directory.GetFiles(sourceDirectory); // Getting all available files

    foreach (string filePath in allFiles)
    {
        try
        {
            string fileName = Path.GetFileName(filePath);
            Match match = Regex.Match(fileName, pattern);

            if (match.Success)
            {
                string caseNumberCompanyNo = match.Groups[1].Value;
                string newFileName = caseNumberCompanyNo + "_" + match.Groups[2].Value + Path.GetExtension(fileName);
                string destinationFolder = Path.Combine(destinationDirectory, caseNumberCompanyNo);

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
