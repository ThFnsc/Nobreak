using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace System.IO
{
    public static class StreamExtensions
    {
        public static MemoryStream CompressAsFile(this Stream toCompress, string fileName)
        {
            var memoryStream = new MemoryStream();
            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var entryItem = zip.CreateEntry(fileName, CompressionLevel.Optimal);
                using var inputStream = toCompress;
                using var entryStream = entryItem.Open();
                inputStream.CopyTo(entryStream);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
