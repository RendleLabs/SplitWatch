using System.Collections.Generic;

namespace ActivityReport
{
    public interface IFiles
    {
        IEnumerable<string> Find(string basePath);
    }
}