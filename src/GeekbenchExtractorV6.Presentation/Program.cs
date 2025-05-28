using GeekbenchExtractorV6.Presentation;

var scapper = new GeekbenchScapper();

var geekbenchReport = await scapper.ScrapResults("https://browser.geekbench.com/v6/cpu/12171915");

Console.WriteLine(geekbenchReport);

var csvSerializer = new CsvResultSerializer();
csvSerializer.SerializeCpuScore([geekbenchReport], "D:\\PetProjectsFilesStorage\\Test");
csvSerializer.SerializeCoreTests([geekbenchReport], "D:\\PetProjectsFilesStorage\\Test", true);
csvSerializer.SerializeCoreTests([geekbenchReport], "D:\\PetProjectsFilesStorage\\Test", false);