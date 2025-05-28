using System.Text;

namespace GeekbenchExtractorV6.Presentation;

public class CsvResultSerializer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reports"></param>
    /// <param name="path">Should be path to the folder</param>
    public void SerializeCpuScore(IEnumerable<GeekbenchReport> reports, string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        using var csvWriter = new StreamWriter(Path.Combine(path, "scores.csv"));
        csvWriter.WriteLine("run_id,single_core,multi_core");
        foreach (var report in reports)
        {
            csvWriter.WriteLine($"{report.RecordId},{report.SingleCoreScore},{report.MultiCoreScore}");
        }
    }

    public void SerializeCoreTests(IList<GeekbenchReport> reports, string path, bool isWriteSingleCore)
    {
        string fileName;
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        if (isWriteSingleCore)
        {
            fileName = "single_core_tests.csv";
        }
        else
        {
            fileName = "multi_core_tests.csv";
        }

        using var csvWriter = new StreamWriter(Path.Combine(path, fileName));

        var testNames = isWriteSingleCore ? 
            reports.First().SingleCoreBenchmarkResults.Select(s => s.Name.ToLower()) : 
            reports.First().MultiCoreBenchmarkResults.Select(s => s.Name.ToLower());
        
        var sb = new StringBuilder();
        sb.Append("run_id,");
        sb.AppendJoin(",", testNames);
        
        csvWriter.WriteLine(sb.ToString());

        sb.Clear();
        
        foreach (var report in reports)
        {
            var scoreValues = isWriteSingleCore ? 
                report.SingleCoreBenchmarkResults.Select(s => s.ScoreValue).ToList() : 
                report.MultiCoreBenchmarkResults.Select(s => s.ScoreValue).ToList();
            scoreValues.Insert(0, report.RecordId ?? -1);
            sb.AppendJoin(",", scoreValues);
            sb.Append("\n");
        }
        
        csvWriter.WriteLine(sb.ToString());
    }
}