using ScanDoc.Logger;
using System;
using System.IO;
using System.Linq;

class Program
{
    public required ILogger logging;

    static void Main()
    {
        string directoryPath = @"C:\DirectoryPath"; 
        string filePattern = "*.pdf"; 

        // Getting all files in the directory
        string[] files = Directory.GetFiles(directoryPath, filePattern);

        // Grouping files by their name without extension
        var fileGroups = files.GroupBy(file =>
            Path.GetFileNameWithoutExtension(file));

        foreach (var group in fileGroups)
        {
            int counter = 0;

            foreach (var oldFileName in group)
            {
                counter++;
                string fileExtension = Path.GetExtension(oldFileName);
                string newFileName = $"{Path.GetFileNameWithoutExtension(oldFileName)}-{counter:D3}{fileExtension}";

                string newFilePath = Path.Combine(Path.GetDirectoryName(oldFileName), newFileName);

                // Renaming the file according to intended format and moving to intended destination
                File.Move(oldFileName, newFilePath);

                logging.Log($"Renamed: {oldFileName} to {newFileName}");
            }
        }
    }
}
