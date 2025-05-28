using System.CommandLine;

namespace GeekbenchExtractorV6.Presentation;

class Program
{
    static async Task Main(string[] args)
    {
        IEnumerable<string> linkReports = [];
        string savePath = string.Empty;
        var fileOption = new Option<FileInfo>(
            name: "--file",
            description: "The file that contains URLs to GeekBench reports.")
            {IsRequired = true};
        fileOption.AddAlias("-f");

        var delayOption = new Option<int>(
            name: "--delay",
            description: "Delay (in milliseconds) between parsing results from URL reports.",
            getDefaultValue: () => 500
        );

        var savePathOptions = new Option<string>(
            name: "--save-path",
            description: "The save path for csv files of GeekBench reports."
            )
            {IsRequired = true};

        var rootCommand = new RootCommand("App for quick getting results from GeekBench site to .csv file.");
        rootCommand.AddOption(fileOption);
        rootCommand.AddOption(delayOption);
        rootCommand.AddOption(savePathOptions);

        rootCommand.SetHandler((fileOptionValue, delayOptionValue, savePathOptionValue) =>
        {
            linkReports = ReadFile(fileOptionValue);
            savePath = savePathOptionValue;
        }, fileOption, delayOption, savePathOptions);
        
        await rootCommand.InvokeAsync(args);
        
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
            }
        }

        var csvSerializer = new CsvResultSerializer();
        try
        {
            csvSerializer.SerializeCpuScore(scarpedGeekbenchReports, savePath);
            csvSerializer.SerializeCoreTests(scarpedGeekbenchReports, savePath, isWriteSingleCore: true);
            csvSerializer.SerializeCoreTests(scarpedGeekbenchReports, savePath, isWriteSingleCore: false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to write records to file. {ex.Message}");
        }
    }

    static IEnumerable<string> ReadFile(FileInfo file)
    {
        return File.ReadLines(file.FullName);
    }
}