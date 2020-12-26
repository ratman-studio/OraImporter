using System;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

namespace com.szczuro.importer.ora
{
    /// <summary>
    /// this class unzip ora file and parse all items and provide ILayeredImageFile 
    /// </summary>
    [Serializable]
    class OraFile 
    {
        [SerializeField]
        public List<Sprite> layers = new List<Sprite>();
        [SerializeField]
        private Sprite _thumbnail;
        
        public string path;

        
        public OraFile (string path)
        {
            Debug.Log($"Ora import {path}");
            var textureList = getTextureList(path);
            foreach (var tex in textureList)
            {
                layers.Add(SpriteFromTexture(tex));                
            }

            //spritesLib = GenerateSpriteList(layers);
            _thumbnail = findThumbnailSprite();
        }
        
        public Sprite getThumbnail()
        {
            if (_thumbnail==null)
            {
                Debug.LogWarning("Thumbnail not found search thumbnail");
                _thumbnail = findThumbnailSprite();
            }
            return _thumbnail;
        }

        /// <summary> probably thumbnail in ora files is stored in Thumbnail/thumbnail.png </summary>
        /// <returns>thumbnail sprite</returns>
        private Sprite findThumbnailSprite()
        {
            foreach (var sprite in layers)
            {
                if (sprite.name == "thumbnail.png")
                    return sprite;

            }
            Debug.LogWarning("thumbnail search failed");
            return null;
        }
        
        public List<Sprite> getLayers()
        {
            return layers;
        }
        
        private Sprite SpriteFromTexture(Texture2D texture)
        {
            var rect = new Rect(0f, 0f, texture.width, texture.height);
            var pivot = new Vector2(.5f, .5f);
            var pixelPerUnit = 100f;
            var sprite = Sprite.Create(texture, rect, pivot, pixelPerUnit);
            sprite.name = texture.name;
            
            Debug.Log($"converted to sprite {sprite.name} {sprite}"); 
            return sprite;
        }
        
        #region ZipReader

        private static List<Texture2D> getTextureList(string zipPath)
        {
            var archives = new List<Texture2D>();
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                Debug.Log($"{archive.Mode} archive {zipPath}");
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        archives.Add(getTextureFromEntry(entry));
                    }
                    else
                    {
                        Debug.LogWarning($"skip entry {entry}");
                    }
                }
            }
            return archives;
        }

        private static Texture2D getTextureFromEntry(ZipArchiveEntry entry)
        {
            var texture = new Texture2D(2, 2);
            using (var fileStream = entry.Open())
            {
                var imageData = new byte[entry.Length];
                fileStream.Read(imageData, 0, (int) entry.Length);
                texture.LoadImage(imageData);
                texture.name = entry.Name;
            }
            return texture;
        }

        #endregion

       
    }
}