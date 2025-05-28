using System.CommandLine;

namespace GeekbenchExtractorV6.Presentation;

class Program
{
    static async Task<int> Main(string[] args)
    {
        IEnumerable<string> linkReports = [];
        var fileOption = new Option<FileInfo?>(
            name: "--file",
            description: "The file that contains URLs to GeekBench reports.")
            {IsRequired = true};
        fileOption.AddAlias("-f");

        var rootCommand = new RootCommand("Sample app for System.CommandLine");
        rootCommand.AddOption(fileOption);

        rootCommand.SetHandler((file) => 
            { 
                linkReports = ReadFile(file!); 
            },
            fileOption);
        
        var scapper = new GeekbenchScapper();
        
        IList<GeekbenchReport> scarpedGeekbenchReports = [];

        foreach (var linkReport in linkReports)
        {
            try
            {

                scarpedGeekbenchReports.Add(await scapper.ScrapResults(linkReport));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to parse report. {ex.Message}");
                return -1;
            }
        }

        var csvSerializer = new CsvResultSerializer();
        try
        {
            csvSerializer.SerializeCpuScore(scarpedGeekbenchReports, @"D:\PetProjectsFilesStorage\Test");
            csvSerializer.SerializeCoreTests(scarpedGeekbenchReports, @"D:\PetProjectsFilesStorage\Test",
                isWriteSingleCore: true);
            csvSerializer.SerializeCoreTests(scarpedGeekbenchReports, @"D:\PetProjectsFilesStorage\Test",
                isWriteSingleCore: false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to write records to file. {ex.Message}");
            return -1;
        }

        return await rootCommand.InvokeAsync(args);
    }

    static IEnumerable<string> ReadFile(FileInfo file)
    {
        return File.ReadLines(file.FullName);
    }
}