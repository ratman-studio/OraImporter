using System.Collections.Generic;
using com.szczuro.importer.ora;
using UnityEngine;

namespace com.szczuro.importer
{
    /// <summary> provider of files that parse and provide data </summary>
    
    public static class MultiLayerFileFactory
    {
        public static IMultiLayerData CreteFileFromPath(string path)
        {
            return OraData.CreateFromFile(path);
        }
    }

    public interface IMultiLayerData
    {
        Texture2D GetThumbnail();
        Texture2D GetMergedLayers();
        List<Texture2D> GetLayers();
        
        string GetTextureName(Texture2D texture);
    }

}