using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using ScanDoc.Logger;

class Program
{
    private static readonly ILogger logging = new Logger();
    private readonly IConfiguration _configuration;

    public Program(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    static void Main()
    {
  
        try
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            string sourceDirectory = configuration["SourceDirectory"];
            string destinationDirectory = configuration["DestinationDirectory"];


            // Create the destination directory if it doesn't exist.
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            // Defining a regular expression pattern to match file format.
            string pattern = @"^(\d{9}-\d{2})_(.+)\.pdf$";

            string[] pdfFiles = Directory.GetFiles(sourceDirectory, "*.pdf");

            foreach (string filePath in pdfFiles)
            {
                try
                {
                    string fileName = Path.GetFileName(filePath);

                    // Checking if the file name matches the expected format.
                    Match match = Regex.Match(fileName, pattern);

                    if (match.Success)
                    {
                        string caseNumberCompanyNo = match.Groups[1].Value;
                        string newFileName = caseNumberCompanyNo + "_" + match.Groups[2].Value + ".pdf";
                        string destinationFolder = Path.Combine(destinationDirectory, caseNumberCompanyNo);

                        // Create the destination folder if it doesn't exist.
                        if (!Directory.Exists(destinationFolder))
                        {
                            Directory.CreateDirectory(destinationFolder);
                        }

                        string newFilePath = Path.Combine(destinationFolder, newFileName);

                        // Move the file to the destination folder with the new name.
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
