using System.Text;

namespace GeekbenchExtractorV6.Presentation;

public class GeekbenchReport
{
    public GeekbenchReport()
    {
        
    }
    public GeekbenchReport(string errorMessage)
    {
        SingleCoreScore = -1;
        MultiCoreScore = -1;
        ErrorMessage = errorMessage;
    }

    public int SingleCoreScore { get; set; }

    public IEnumerable<BenchmarkResult> SingleCoreBenchmarkResults { get; set; } = [];
    
    public int MultiCoreScore { get; set; }
    
    public IEnumerable<BenchmarkResult> MultiCoreBenchmarkResults { get; set; } = [];
    
    public string? ErrorMessage { get; set; }

    public override string ToString()
    {
        if (ErrorMessage != null)
        {
            return ErrorMessage;
        }
        else
        {
            var sb = new StringBuilder();
            sb.AppendLine("Single-Core perfomance: " + SingleCoreScore);
            foreach (var result in SingleCoreBenchmarkResults)
            {
                sb.AppendLine($"{result.Name}: {result.ScoreValue} ({result.OperationValue} { result.OperationUnit})");
            }

            sb.AppendLine("\n");
            
            sb.AppendLine("Multi-Core perfomance: " + MultiCoreScore);
            foreach (var result in MultiCoreBenchmarkResults)
            {
                sb.AppendLine($"{result.Name}: {result.ScoreValue} ({result.OperationValue} { result.OperationUnit})");
            }
            
            return sb.ToString();
        }
    }
}