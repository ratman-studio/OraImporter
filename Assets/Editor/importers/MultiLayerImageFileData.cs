using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace studio.ratman.importer
{
    [Serializable]
    public abstract class MultiLayerImageFileData: Object
    {
        public abstract Texture2D GetThumbnail();
        public abstract Texture2D GetMergedLayers();
        public abstract List<Texture2D> GetLayers();
        public abstract string GetTextureName(Texture2D texture);
        public abstract string GetFileName();

        public abstract List<string> GetLayerList();
        public abstract string GetTexture(string textureName);
    }
}