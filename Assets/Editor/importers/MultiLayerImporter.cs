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
            Atlas
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
            switch (importAs)
            {
                // texture as is 
                case ImportType.Single:
                    SpriteImportAsSingle(ctx);
                    break;

                case ImportType.Atlas:
                    SpriteImportAsAtlas(ctx);
                    break;

                default:
                    throw new NotImplementedException($"Import asset not as ImportAs {importAs} is not implemented");
            }
        }

        private void SpriteImportAsSingle(AssetImportContext ctx)
        {
            var merged = _fileData.GetMergedLayers();
            var importName = _fileData.GetFileName();
            // merged images as main texture
            ctx.AddObjectToAsset(importName, merged);
            ctx.SetMainObject(merged);
            // create sprite from texture
            var sprite = SpriteFromTexture(merged, importName);
            ctx.AddObjectToAsset(sprite.name, sprite);
        }

        private void SpriteImportAsAtlas(AssetImportContext ctx)
        {
            atlas = CreateAtlas(_fileData, out var sprites);
            ctx.AddObjectToAsset(atlas.name, atlas);
            ctx.SetMainObject(atlas);
            // sprites and sprite renderers as sub objects 
            foreach (var s in sprites)
            {
                var go = new GameObject();
                var spriteRenderer = go.AddComponent<SpriteRenderer>();

                spriteRenderer.sprite = s;
                ctx.AddObjectToAsset(s.name, s);

                go.name = s.name;
                ctx.AddObjectToAsset(s.name, go);
            }
        }

        /// <summary> add layers as atlas </summary>
        private Texture2D CreateAtlas(
            MultiLayerImageFileData multiLayerImageFileData, out List<Sprite> sprites)
        {
            var textures = multiLayerImageFileData.GetLayers();
            var atlasTexture = CrateAtlasTexture(textures, out var rects);
            var fileName = multiLayerImageFileData.GetFileName();


            sprites = new List<Sprite>();

            for (var i = 0; i < rects.Length; i++)
            {
                var rect = rects[i];
                var sprite = CreateAtlasSprite(atlasTexture, rect);
                // sprites
                sprite.name = multiLayerImageFileData.GetTextureName(textures[i]);
                sprites.Add(sprite);
            }

            atlasTexture.name = fileName;
            return atlasTexture;
        }

        private static Sprite CreateAtlasSprite(Texture2D atlasTexture, Rect rect)
        {
            var w = atlasTexture.width;
            var h = atlasTexture.height;
            var area = new Rect(rect.x * w, rect.y * h, rect.width * w, rect.height * h);
            return Sprite.Create(atlasTexture, area, Vector2.zero);
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

        private static Texture2D CrateAtlasTexture(List<Texture2D> textures, out Rect[] rects)
        {
            var atlas = new Texture2D(1, 1);
            atlas.alphaIsTransparency = true;
            rects = atlas.PackTextures(textures.ToArray(), 1);

            return atlas;
        }
    }
}