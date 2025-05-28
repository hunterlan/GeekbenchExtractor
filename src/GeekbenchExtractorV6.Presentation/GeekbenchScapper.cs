using HtmlAgilityPack;

namespace GeekbenchExtractorV6.Presentation;

public class GeekbenchScapper
{
    public async Task<GeekbenchReport> ScrapResults(string url)
    {
        GeekbenchReport report = new();
        
        var htmlPageString = await GetContentAsync(url);
        var page = new HtmlDocument();
        page.LoadHtml(htmlPageString);

        var primaryDiv = page.DocumentNode.Descendants("div")
            .FirstOrDefault(div => div.GetAttributeValue("class", "").Contains("primary"));
        if (primaryDiv is null)
        {
            return new GeekbenchReport
            {
                SingleCoreScore = -1,
                MultiCoreScore = -1,
                ErrorMessage = "Can't parse HTML page properly."
            };
        }

        var informationData = primaryDiv.Elements("div").ToList();
        var singleCoreSectionIndex = informationData
            .FindIndex(div =>
                div.GetAttributeValue("class", "").Equals("heading") &&
                div.InnerText.Contains("Single-Core Performance"));
        if (singleCoreSectionIndex is -1)
        {
            return new GeekbenchReport("Can't find single-core performance section.");
        }

        var singleCoreBenchmarkData = informationData[singleCoreSectionIndex + 1];

        int? singleCoreScore = GetCoreScore(singleCoreBenchmarkData);
        if (singleCoreScore is null)
        {
            return new GeekbenchReport("Information about single-core total score is missing.");
        }
        
        report.SingleCoreScore = singleCoreScore.Value;
        var singleCoreResults = GetBenchmarkResults(singleCoreBenchmarkData).ToList();

        if (singleCoreResults.Count is 0)
        {
            return new GeekbenchReport("Information about single-core tests result are missing.");
        }
        
        report.SingleCoreBenchmarkResults = singleCoreResults;
        
        var multiCoreBenchmarkData = informationData[singleCoreSectionIndex + 3];
        int? multiCoreScore = GetCoreScore(multiCoreBenchmarkData);
        if (multiCoreScore is null)
        {
            return new GeekbenchReport("Information about multi-core total score is missing.");
        }
        
        report.MultiCoreScore = multiCoreScore.Value;
        var multiCoreResults = GetBenchmarkResults(multiCoreBenchmarkData).ToList();

        if (multiCoreResults.Count is 0)
        {
            return new GeekbenchReport("Information about multi-core tests result are missing."); 
        }
        report.MultiCoreBenchmarkResults = multiCoreResults;
        report.RecordId = GetRecordId(url);

        return report;
    }

    private static async Task<string> GetContentAsync(string url)
    {
        var client = new HttpClient();
        var response = await client.GetStringAsync(url);
        return response;
    }

    private static int? GetCoreScore(HtmlNode singleCoreSection)
    {
        var singleCoreScoreRow = singleCoreSection.Descendants("tr")
            .SingleOrDefault(tr => tr.GetAttributeValue("class", "") == "stacked-heading");
        if (singleCoreScoreRow is null)
        {
            return null;
        }

        var singleScoreElem = singleCoreScoreRow.Descendants("th")
            .SingleOrDefault(th => th.GetAttributeValue("class", "").Equals("score"));
        return singleScoreElem is null ? null : int.Parse(singleScoreElem.InnerText);
    }

    private static IEnumerable<BenchmarkResult> GetBenchmarkResults(HtmlNode resultRows)
    {
        List<BenchmarkResult> benchmarkResults = [];
        var tableBody = resultRows.ChildNodes
            .SingleOrDefault(c => c.Name.Equals("table"))?
            .ChildNodes.SingleOrDefault(c => c.Name.Equals("tbody"));
        if (tableBody is null)
        {
            return benchmarkResults;
        }

        var rows = tableBody.Descendants("tr");
        foreach (var row in rows)
        {
            var benchmarkNameNode = row.Descendants("td")
                .SingleOrDefault(td => td.GetAttributeValue("class", "").Equals("name"));
            var benchmarkScoreNode = row.Descendants("td")
                .SingleOrDefault(td => td.GetAttributeValue("class", "").Equals("score"));

            if (benchmarkNameNode is null || benchmarkScoreNode is null)
            {
                continue;
            }
            
            var descriptionNode = benchmarkScoreNode.ChildNodes.Single(n => n.Name.Equals("span"));
            var operationScore = descriptionNode.InnerText.Split(" ");

            var benchmarkResult = new BenchmarkResult
            {
                Name = benchmarkNameNode.InnerText.TrimEnd().TrimStart(),
                ScoreValue = int.Parse(benchmarkScoreNode.FirstChild.InnerText),
                OperationValue = double.Parse(operationScore[0]),
                OperationUnit = operationScore[1].TrimEnd().TrimStart(),
            };
            benchmarkResults.Add(benchmarkResult);
        }
        
        return benchmarkResults;
    }

    private static int GetRecordId(string url) => int.Parse(url.Split("/").Last());
}