using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace com.szczuro.importer.ora
{
    [ScriptedImporter(2, "ora")]
    public class OraImporter : ScriptedImporter
    {
        enum ImportType
        {
            Single,
            Multi
        }

        [SerializeField] private ImportType ImportAs = ImportType.Single;
        private OraFile oraFile;

        #region ScriptedImporter implementation

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx == null) return;

            var path = ctx.assetPath;
            Debug.Log($"Importing Ora Object {path}");
            var fileInfo = new FileInfo(path);
            
            var filePrefab = RegisterMainPrefab(ctx, fileInfo.Name);
            
            oraFile = new OraFile(path);
            
            AddTexturesToPrefab(filePrefab);
        }

        private void AddTexturesToPrefab(GameObject filePrefab)
        {
            var imageList = oraFile.layers;
            foreach (var texture in imageList)
            {
                var tex_go = new GameObject(texture.name);
                var spriteRenderer = tex_go.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = SpriteFromTexture(texture);
                tex_go.transform.SetParent(filePrefab.transform);
            }
        }

        private Sprite SpriteFromTexture(Texture2D texture)
        {
            var rect = new Rect(0f, 0f, texture.width, texture.height);
            var pivot = new Vector2(.5f, .5f);
            var pixelPerUnit = 100f;
            return Sprite.Create(texture, rect, pivot, pixelPerUnit);
        }

        private static GameObject RegisterMainPrefab(AssetImportContext ctx, string name)
        {
            var filePrefab = new GameObject($"{name}_GO");
            Debug.Log($"Register Main Object {filePrefab.name}");
            ctx.AddObjectToAsset("main", filePrefab);
            ctx.SetMainObject(filePrefab);
            return filePrefab;
        }

        #endregion
    }

    class OraFile 
    {
        public List<Texture2D> layers = new List<Texture2D>();
        public string path;

        public OraFile (string path)
        {
            Debug.Log($"Ora import {path}");
            var textureList = getTextureList(path);
            foreach (var tex in textureList)
            {
                layers.Add(tex);                
            }
        }

        #region ZipReader

        private static List<Texture2D> getTextureList(string zipPath)
        {
            var archives = new List<Texture2D>();
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                Debug.Log($"open archive {archive.Mode} {zipPath}");
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    
                    if (entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log($"parse entry {entry}");
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