using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace com.szczuro.importer.ora
{
    [ScriptedImporter(1, "ora")]
    public class OraImporter : ScriptedImporter
    {
        enum ImportType
        {
            Single,
            Multi
        }

        [SerializeField] private ImportType ImportAs = ImportType.Single;
        
        private OraFile _oraFile;
        

        #region ScriptedImporter implementation

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx == null) return;

            var path = ctx.assetPath;
            Debug.Log($"Importing Ora Object {path}");
            var fileInfo = new FileInfo(path);
            
            // Register root prefab that will be visible in project window instead of file
            var filePrefab = registerMainPrefab(ctx, fileInfo.Name);
            
            // dummy file helper
            Debug.Log("Create Orafile");
            _oraFile = new OraFile(path);
            
            // storage place for sprites  
            Debug.Log("Create spritelib");
            var spritesLib = createSpritesLib();
            Debug.Log($"SpriteLib length {spritesLib.entries.Count}");
            //ctx.AddObjectToAsset("spriteLib", spritesLib);
            
            Debug.Log($"add spriteRenderers to prefab");
            addSpritesToPrefab(ctx, filePrefab, spritesLib);
            
            Debug.Log($"set main prefab");
            ctx.SetMainObject(filePrefab);
        }

        private SimpleSpriteLib createSpritesLib()
        {
            var spritesLib = ScriptableObject.CreateInstance<SimpleSpriteLib>();
            
            Debug.Log("Storing Sprites in Scriptable Object");
            var imageList = _oraFile.layers;
            foreach (var texture in imageList)
            {
                Debug.Log($"Storing {texture.name} in {texture.width}x{texture.width} {texture.format}");
                spritesLib.entries.Add(SpriteFromTexture(texture));
            }
            return spritesLib;
        }

        private void addSpritesToPrefab(AssetImportContext ctx, GameObject filePrefab, SimpleSpriteLib sprites)
        {
            foreach (var sprite in sprites.entries)
            {
                var texGO = new GameObject(sprite.name);
                var spriteRenderer = texGO.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                Debug.Log(spriteRenderer.sprite);
                ctx.AddObjectToAsset(sprite.name,sprite);
                //texGO.transform.SetParent(filePrefab.transform);
            }
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

        private static GameObject registerMainPrefab(AssetImportContext ctx, string name)
        {
            
            var filePrefab = new GameObject($"{name}_GO");
            ctx.AddObjectToAsset("main", filePrefab);
            
            return filePrefab;
        }

        #endregion
    }

    [Serializable]
    public class SimpleSpriteLib:ScriptableObject
    {
        [SerializeField]
        public List<Sprite> entries = new List<Sprite>();
    }

    [Serializable]
    class OraFile 
    {
        [SerializeField]
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