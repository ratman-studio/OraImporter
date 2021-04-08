using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace com.szczuro.importer
{
    /// <summary>
    /// base class to importing multi layer image files 
    /// </summary>
    public class MultiLayerImporter : ScriptedImporter
    {
        public enum ImportType
        {
            Single,
            Multi,
            Atlas
        }

        [SerializeField] public ImportType ImportAs = ImportType.Single;
        [SerializeField] private IMultiLayerData _multiLayerData;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx == null) return;

            var path = ctx.assetPath;

            Debug.Log($"Importing Object {path}");
            var fileInfo = new FileInfo(path);

            //file import helper
            Debug.Log($"Create file {_multiLayerData}");
            _multiLayerData = MultiLayerFileFactory.CreteFileFromPath(path);

            // Register root prefab that will be visible in project window instead of file

            
            switch (ImportAs)
            {
                case ImportType.Multi:
                    var thumbnail = _multiLayerData.GetThumbnail();
                    var mainObject = RegisterMainPrefab(ctx, fileInfo.Name, thumbnail);
                    AddLayersAtlas(ctx, _multiLayerData);
                    AddSingleLayers(ctx, _multiLayerData);
                    ctx.SetMainObject(mainObject);
                    Debug.Log($"set main prefab");
                    break;
                case ImportType.Single:
                    var texture = _multiLayerData.GetMergedLayers();
                    var textName = _multiLayerData.GetTextureName(texture);
                    ctx.AddObjectToAsset(textName,texture);
                    ctx.SetMainObject(texture);
                    break;
            }
        }

        /// <summary> add layers as atlas </summary>
        private static void AddLayersAtlas(AssetImportContext ctx, IMultiLayerData multiLayerData)
        {
            var textures = multiLayerData.GetLayers();
            var atlas = CrateAtlasTexture(textures);
            
            var fileName = multiLayerData.GetFileName();
            atlas.name = fileName;
            ctx.AddObjectToAsset(fileName, atlas);
        }

        private void AddSingleLayers(AssetImportContext ctx, IMultiLayerData multiLayerData)
        {
            Debug.Log("Create sprites lib");
            var textures = multiLayerData.GetLayers();
            foreach (var texture in textures)
            {
                var texName = multiLayerData.GetTextureName(texture);
                var sprite = SpriteFromTexture(texture, texName);
                ctx.AddObjectToAsset(texName, texture);
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
            atlas.PackTextures(textures.ToArray(), 1);
            
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