using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ActivityReport
{
    public class Files : IFiles
    {
        public IEnumerable<string> Find(string basePath)
        {
            return Directory.EnumerateFiles(basePath, "*.xml")
                .Concat(Directory.EnumerateDirectories(basePath).SelectMany(Find));
        }
    }
}