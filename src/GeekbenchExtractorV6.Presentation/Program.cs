using GeekbenchExtractorV6.Presentation;

var scapper = new GeekbenchScapper();

var geekbenchReport = await scapper.ScrapResults("https://browser.geekbench.com/v6/cpu/12171915");

Console.WriteLine(geekbenchReport);