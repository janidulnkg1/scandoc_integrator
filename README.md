# scandoc_integrator

This is a console application that performs file operations, 
specifically moving files from a source directory to a destination directory while renaming them based on a specific pattern. 

## Purpose
The purpose of this code is to organize files in the source directory into subdirectories based on the case number and company name extracted from the file names. 
It assumes that the source directory contains files with names following a specific format and renames them as described while moving them to the destination directory.
Any errors encountered during the process are logged using the ILogger interface.

## Steps Involved

The code begins by importing necessary namespaces: file operations, configuration management, and regular expressions.
It also uses a custom ILogger interface for logging.

An ILogger instance called logging is created, presumably for logging messages.

Inside a try-catch block, the main logic of the program is executed.

It starts by building a configuration using the ConfigurationBuilder class. 
This configuration is typically loaded from a JSON file named "config.json" located in the current directory. 
It retrieves two configuration values: SourceDirectory and DestinationDirectory, which presumably specify the source and destination directories for file operations.

It checks if the DestinationDirectory exists, and if not, it creates the directory.

It defines a regular expression pattern pattern that is used to match file names in a specific format. 
The format appears to be a 9-digit number followed by a hyphen and a two-digit number, an underscore, and a filename extension. 
The regular expression pattern captures the case number and company name (assuming this is what the filename represents).

It uses Directory.GetFiles to get an array of file paths in the source directory.

It then iterates through each file path in the allFiles array.

Inside the loop, it attempts to extract information from the file name using the regular expression pattern. 
If the match is successful, it extracts the case number and company name from the file name.

It constructs a new file name in the format: caseNumberCompanyNo_companyName.extension.

It creates a destination folder with the caseNumberCompanyNo as the name if it does not already exist.

It constructs a new file path in the destination directory for the file.

It uses File.Move to move the file from the source path to the new destination path. 
This effectively renames the file in the process.

It logs a message indicating that the file has been moved.

If there is an exception during any step of the file processing (e.g., invalid file name or file operations fail), it logs an error message.

After processing all files, it logs a message indicating that the processing is complete.

If any exceptions occur at the top level (e.g., configuration loading or directory creation fails), the program logs an error message.
