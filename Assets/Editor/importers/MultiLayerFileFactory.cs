using System.Collections.Generic;
using com.szczuro.importer.ora;
using UnityEngine;

namespace com.szczuro.importer
{
    internal static class MultiLayerFileFactory
    {
        internal static IMultiLayerFile CreteFileFromPath(string path)
        {
            return OraFile.CreateInstance(path);
        }
    }
    
    internal interface IMultiLayerFile
    {
        Sprite GETThumbnailSprite();
        Sprite GETMergedLayers();
        List<Sprite> GETLayers();
    }

}