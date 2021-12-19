using System;
using System.IO;

namespace studio.ratman.importer
{
    /// <summary> provider of files that parse and provide data </summary>
    
    public static class MultiLayerFileFactory
    {
        public static MultiLayerImageFileData CreteFileFromPath(string path)
        {
            var pathExtension = Path.GetExtension(path);
            switch (pathExtension.ToLower())
            {
                case ".ora":
                    return OraImageFileData.CreateFromFile(path);
                case ".kra":
                    return KraImageFileData.CreateFromFile(path);
                default:
                    throw new ArgumentException($"cannot create Data for extension {pathExtension}");
            }
        }
    }
}