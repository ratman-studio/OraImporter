using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace studio.ratman.importer
{
    /// <summary>
    /// base class to importing multi layer image files 
    /// </summary>
    public class MultiLayerImporter : ScriptedImporter
    {
        
        public enum TextureType
        {
            Default,
            Sprite
        }

        public enum ImportType
        {
            Single,
            Atlas,
            Sprites,
        }

        [SerializeField] public TextureType textureType = TextureType.Default;
        [SerializeField] public ImportType importAs = ImportType.Single;
        [SerializeField] public Texture2D atlas;
        private MultiLayerImageFileData _fileData;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx == null) return;
            var path = ctx.assetPath;

            Debug.Log($"Importing File {path}");
            var fileInfo = new FileInfo(path);

            //file import helper
            _fileData = MultiLayerFileFactory.CreteFileFromPath(path);

            // Register root prefab that will be visible in project window instead of file

            switch (textureType)
            {
                case TextureType.Default:
                    DefaultImport(ctx);
                    break;
                case TextureType.Sprite:
                    SpriteImport(ctx);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DefaultImport(AssetImportContext ctx)
        {
            // import as simple texture
            var merged = _fileData.GetMergedLayers();
            var textName = _fileData.GetTextureName(merged);
            ctx.AddObjectToAsset(textName, merged);
            ctx.SetMainObject(merged);
        }

        private void SpriteImport(AssetImportContext ctx)
        {
            var merged = _fileData.GetMergedLayers();
            var importName = _fileData.GetFileName();

            switch (importAs)
            {
                
                // texture as is 
                case ImportType.Single:
                    ctx.AddObjectToAsset(importName, merged);
                    ctx.SetMainObject(merged);
                    
                    var sprite = SpriteFromTexture(merged, importName);
                    ctx.AddObjectToAsset(sprite.name, sprite);
                    break;

                // import atlas and sprites 
                case ImportType.Sprites:
                    ctx.AddObjectToAsset(importName, merged);
                    ctx.SetMainObject(merged);
                    
                    AddSingleLayers(ctx, _fileData);
                    Debug.Log($"set main prefab");
                    break;

                // import only atlas
                case ImportType.Atlas:
                    var go = new GameObject();
                    ctx.AddObjectToAsset(importName, go);
                    ctx.SetMainObject(go);
                    
                    atlas = CreateAtlas(_fileData);
                    ctx.AddObjectToAsset(atlas.name, atlas);
                    break;
                
                default:
                    throw new NotImplementedException($"Import asset not as ImportAs {importAs} is not implemented");
            }
        }

        /// <summary> add layers as atlas </summary>
        private Texture2D CreateAtlas(MultiLayerImageFileData multiLayerImageFileData)
        {
            var textures = multiLayerImageFileData.GetLayers();
            var result = CrateAtlasTexture(textures);
            var fileName = multiLayerImageFileData.GetFileName();
            result.name = fileName;
            return result;
        }

        private void AddSingleLayers(AssetImportContext ctx, MultiLayerImageFileData multiLayerImageFileData)
        {
            Debug.Log("Create sprites lib");
            var textures = multiLayerImageFileData.GetLayers();
            foreach (var texture in textures)
            {
                var texName = multiLayerImageFileData.GetTextureName(texture);
                var sprite = SpriteFromTexture(texture, texName);
                //ctx.AddObjectToAsset(texName, texture);
                AddSpriteToAsset(ctx, sprite);
            }
        }


        private static Sprite SpriteFromTexture(Texture2D texture, string textureName = "")
        {
            var rect = new Rect(0f, 0f, texture.width, texture.height);
            var pivot = new Vector2(.5f, .5f);
            var pixelPerUnit = 100f;
            var sprite = Sprite.Create(texture, rect, pivot, pixelPerUnit);

            if (textureName != "")
                sprite.name = textureName;

            Debug.Log($"converted to sprite {sprite.name} {sprite}");
            return sprite;
        }

        private static Texture2D CrateAtlasTexture(List<Texture2D> textures)
        {
            var atlas = new Texture2D(1, 1);
            atlas.alphaIsTransparency = true;
            var packed = atlas.PackTextures(textures.ToArray(), 1);
            return atlas;
        }

        private static void AddSpriteToAsset(AssetImportContext ctx, Sprite sprite)
        {
            ctx.AddObjectToAsset(sprite.name, sprite);
        }

        private static GameObject RegisterMainPrefab(AssetImportContext ctx, string name,
            Texture2D thumbNail)
        {
            var filePrefab = new GameObject($"{name}_GO");
            ctx.AddObjectToAsset("main", filePrefab, thumbNail);

            return filePrefab;
        }
    }
}