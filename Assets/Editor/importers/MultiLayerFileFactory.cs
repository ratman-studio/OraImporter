namespace studio.ratman.importer
{
    /// <summary> provider of files that parse and provide data </summary>
    
    public static class MultiLayerFileFactory
    {
        public static MultiLayerImageFileData CreteFileFromPath(string path)
        {
            return OraImageFileData.CreateFromFile(path);
        }
    }
}