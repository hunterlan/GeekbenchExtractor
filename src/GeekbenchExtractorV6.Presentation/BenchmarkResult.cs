namespace GeekbenchExtractorV6.Presentation;

public class BenchmarkResult
{
    /// <summary>
    /// Name of benchmark, e.g. "HDR"
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Value, determined by benchmark, e.g. "3123"
    /// </summary>
    public required int ScoreValue { get; set; }
    
    /// <summary>
    /// Real value that was got as a result of the operation, e.g. "333.8"
    /// </summary>
    public required double OperationValue { get; set; }
    
    /// <summary>
    /// e.g. "MB/s"
    /// </summary>
    public required string OperationUnit { get; set; }
}