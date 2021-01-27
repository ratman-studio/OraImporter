using com.szczuro.importer.ora;

namespace com.szczuro.importer
{
    internal static class MultiLayerFileFactory
    {
        internal static IMultiLayerFile CreteFileFromPath(string path)
        {
            return OraFile.CreateInstance(path);
        }
    }
}