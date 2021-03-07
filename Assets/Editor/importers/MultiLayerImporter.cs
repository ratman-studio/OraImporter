using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace com.szczuro.importer
{
    public class MultiLayerImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        [SerializeField] public ImportType ImportAs = ImportType.Single;
        [SerializeField] private IMultiLayerFile _multiLayerFile;
        
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            if (ctx == null) return;

            var path = ctx.assetPath;
            Debug.Log($"Importing Object {path}");
            var fileInfo = new FileInfo(path);

            //file helper
            Debug.Log($"Create file {_multiLayerFile}");
            _multiLayerFile = MultiLayerFileFactory.CreteFileFromPath(path);

            // Register root prefab that will be visible in project window instead of file
            var filePrefab = RegisterMainPrefab(ctx, fileInfo.Name, _multiLayerFile.GetThumbnail());
              
            if (ImportAs == ImportType.Multi)
            {
                Debug.Log("Create sprites lib");
                var textures = _multiLayerFile.GetLayers();
                foreach (var texture in textures)
                {
                    var texName = _multiLayerFile.GetTextureName(texture);
                    var sprite = SpriteFromTexture(texture, texName);    
                    addSpritesToPrefab(ctx,sprite);
                }
            }
              
            if (ImportAs == ImportType.Single)
            {
                var texture = _multiLayerFile.GetMergedLayers();
                var texName = _multiLayerFile.GetTextureName(texture);
                var sprite = SpriteFromTexture(texture, texName);
                addSpritesToPrefab(ctx, sprite);
            }
            
            if (ImportAs == ImportType.Atlas)
            {
                var textures = _multiLayerFile.GetLayers();
                var atlas = crateAtlasTexture(textures);
                atlas.name = fileInfo.Name;
                ctx.AddObjectToAsset(fileInfo.Name, atlas);
            }
            
            Debug.Log($"set main prefab");
            ctx.SetMainObject(filePrefab);
        }
        
        // textureName = _multiLayerFile.GetTextureName(texture);
            private Sprite SpriteFromTexture(Texture2D texture, string textureName="")
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


        #region ScriptedImporter implementation

        private Texture2D crateAtlasTexture (List<Texture2D> textures)
        {
            
            Texture2D atlas = new Texture2D(1,1);
            atlas.alphaIsTransparency = true;
            atlas.PackTextures(textures.ToArray(), 1);
            return atlas;
        }
        
        

        private void addSpritesToPrefab(UnityEditor.AssetImporters.AssetImportContext ctx, Sprite sprite)
        {
            ctx.AddObjectToAsset(sprite.name, sprite);
        }

        private static GameObject RegisterMainPrefab(UnityEditor.AssetImporters.AssetImportContext ctx, string name, Texture2D thumbNail)
        {
            var filePrefab = new GameObject($"{name}_GO");
            ctx.AddObjectToAsset("main", filePrefab, thumbNail);

            return filePrefab;
        }

        public enum ImportType
        {
            Single,
            Multi,
            Atlas
        }

        #endregion
    }
}