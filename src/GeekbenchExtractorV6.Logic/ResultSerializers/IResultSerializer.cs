using System.Collections.Generic;
using GeekbenchExtractorV6.Logic.Models;

namespace GeekbenchExtractorV6.Logic.ResultSerializers;

public interface IResultSerializer
{
    /// <summary>
    /// Writes CPU scores to file
    /// </summary>
    /// <param name="reports">Collection of geekbench reports.</param>
    /// <param name="path">Must be path to the folder</param>
    void SerializeCpuScore(IEnumerable<GeekbenchReport> reports, string path);
    /// <summary>
    /// Writes CPU cores score to file
    /// </summary>
    /// <param name="reports">Collection of geekbench reports.</param>
    /// <param name="path">Must be path to the folder</param>
    /// <param name="isWriteSingleCore">Should function write single-core results or multi-core</param>
    void SerializeCoreTests(IList<GeekbenchReport> reports, string path, bool isWriteSingleCore);
}