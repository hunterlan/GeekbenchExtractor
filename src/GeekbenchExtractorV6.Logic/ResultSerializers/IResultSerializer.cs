using System.Collections.Generic;
using GeekbenchExtractorV6.Logic.Models;

namespace GeekbenchExtractorV6.Logic.ResultSerializers;

public interface IResultSerializer
{
    void SerializeCpuScore(IEnumerable<GeekbenchReport> reports, string path);
    void SerializeCoreTests(IList<GeekbenchReport> reports, string path, bool isWriteSingleCore);
}