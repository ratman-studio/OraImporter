using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

using UnityEditor;
using UnityEngine;

namespace studio.ratman.importer
{
    /// <summary> parse *.ora file and provide it items vie interface ILayeredImageFile </summary>
    [Serializable]
    public class OraImageFileData : MultiLayerImageFileData
    {
        public static OraImageFileData CreateFromFile(string path)
        {
            return new OraImageFileData(path);
        }

        [SerializeField] public List<Texture2D> layers = new List<Texture2D>();
        [SerializeField] private Texture2D thumbnail;
        [SerializeField] public Texture2D mergedLayers;
        [SerializeField] private OraXML.Main _structure;
        [SerializeField] private string path;

        private const string ThumbnailName = "Thumbnails/thumbnail.png";
        private const string MergeLayersName = "mergedimage.png";

        private OraImageFileData(string path)
        {
            this.path = path;
            Debug.Log($"Ora import {this.path}");

            _structure = GetStructure(this.path);
            layers = GETTextureList(this.path);

            thumbnail = FindSpriteByName(ThumbnailName);
            if (thumbnail) layers.Remove(thumbnail);

            mergedLayers = FindSpriteByName(MergeLayersName);
            if (mergedLayers) layers.Remove(mergedLayers);
        }

        // interface IMultiLayerFile

        public override Texture2D GetThumbnail()
        {
            if (thumbnail == null)
            {
                Debug.LogWarning("Thumbnail not found searching ... ");
                thumbnail = FindSpriteByName(ThumbnailName);
            }

            return thumbnail;
        }

        public override Texture2D GetMergedLayers()
        {
            if (mergedLayers == null)
            {
                Debug.LogWarning("Merged Layers not found searching ...");
                mergedLayers = FindSpriteByName(MergeLayersName);
            }

            return mergedLayers;
        }


        public override List<Texture2D> GetLayers()
        {
            return layers;
        }

        public override string GetTextureName(Texture2D texture)
        {
            if (_structure != null)
                return OraXML.Main.GetNameFromTexture(_structure, texture.name);
            return texture.name;
        }

        public override string GetFileName()
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public override List<string> GetLayerList()
        {
            throw new NotImplementedException();
        }

        public override string GetTexture(string textureName)
        {
            throw new NotImplementedException();
        }

        // interface IMultiLayerFile End

        private Texture2D FindSpriteByName(string imageName)
        {
            foreach (var sprite in layers)
                if (sprite.name == imageName)
                    return sprite;
            Debug.LogWarning($"{imageName} search failed");
            return null;
        }

        private static OraXML.Main GetStructure(string zipPath)
        {
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                Debug.Log($"{archive.Mode} archive {zipPath}");
                foreach (var entry in archive.Entries)
                    if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        return GetXMLFromEntry(entry);
                    else
                        Debug.LogWarning($"skip entry {entry}");
            }

            return null;
        }

        private static OraXML.Main GetXMLFromEntry(ZipArchiveEntry entry)
        {
            var serializer = new XmlSerializer(typeof(OraXML.Main));
            using (var fileStream = entry.Open())
            {
                return (OraXML.Main)serializer.Deserialize(fileStream);
            }
        }

        private static List<Texture2D> GETTextureList(string zipPath)
        {
            var archives = new List<Texture2D>();
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                Debug.Log($"{archive.Mode} archive {zipPath}");
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))

                        archives.Add(GETTextureFromEntry(entry));
                    else
                        Debug.LogWarning($"skip entry {entry}");
                }
            }

            return archives;
        }

        private static Texture2D GETTextureFromEntry(ZipArchiveEntry entry)
        {
            var settings = new TextureImporterSettings();
            var platformSettings = new TextureImporterPlatformSettings();
            var format = TextureFormat.ARGB32;
            var mipmap = settings.mipmapEnabled;
            var linear = settings.linearTexture;

            var texture = new Texture2D(2, 2, format, mipmap, linear);

            texture.alphaIsTransparency = settings.alphaIsTransparency;
            using (var fileStream = entry.Open())
            {
                var imageData = new byte[entry.Length];
                fileStream.Read(imageData, 0, (int)entry.Length);
                texture.LoadImage(imageData);
                texture.alphaIsTransparency = true;
                texture.name = entry.FullName;
            }

            return texture;
        }
    }
}