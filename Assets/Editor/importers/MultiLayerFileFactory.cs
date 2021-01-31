using System.Collections.Generic;
using com.szczuro.importer.ora;
using UnityEngine;

namespace com.szczuro.importer
{
    public static class MultiLayerFileFactory
    {
        public static IMultiLayerFile CreteFileFromPath(string path)
        {
            return OraFile.CreateInstance(path);
        }
    }

    public interface IMultiLayerFile
    {
        Sprite GETThumbnailSprite();
        Sprite GETMergedLayers();
        List<Sprite> GETLayers();
    }

}