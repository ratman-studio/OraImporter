using System.Collections.Generic;
using System.IO;
using com.szczuro.importer.ora;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace com.szczuro.importer
{
    public class MultiLayerImporter : ScriptedImporter
    {
        [SerializeField] public ImportType ImportAs = ImportType.Single;
        private IMultiLayerFile _multiLayerFile;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx == null) return;

            var path = ctx.assetPath;
            Debug.Log($"Importing Object {path}");
            var fileInfo = new FileInfo(path);

            //file helper
            Debug.Log($"Create file {_multiLayerFile}");
            _multiLayerFile = MultiLayerFileFactory.CreteFileFromPath(path);


            // Register root prefab that will be visible in project window instead of file
            var filePrefab = RegisterMainPrefab(ctx, fileInfo.Name, _multiLayerFile.GETThumbnailSprite().texture);

            // storage place for sprites  
            if (ImportAs == ImportType.Multi)
            {
                Debug.Log("Create sprites lib");
                var spritesLib = _multiLayerFile.GetLayers();
                addSpritesToPrefab(ctx, spritesLib);
            }

            if (ImportAs == ImportType.Single)
            {
                var sprite = _multiLayerFile.GETMergedLayers();
                addSpritesToPrefab(ctx, new List<Sprite>(){
                sprite});
            }

            Debug.Log($"set main prefab");
            ctx.SetMainObject(filePrefab);
        }

        #region ScriptedImporter implementation

        private void addSpritesToPrefab(AssetImportContext ctx, List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                var texGO = new GameObject(sprite.name);
                var spriteRenderer = texGO.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                Debug.Log(spriteRenderer.sprite);
                ctx.AddObjectToAsset(sprite.name, sprite);
                //texGO.transform.SetParent(filePrefab.transform);
            }
        }

        private static GameObject RegisterMainPrefab(AssetImportContext ctx, string name, Texture2D thumbNail)
        {
            var filePrefab = new GameObject($"{name}_GO");
            ctx.AddObjectToAsset("main", filePrefab, thumbNail);

            return filePrefab;
        }

        public enum ImportType
        {
            Single,
            Multi
        }

        #endregion
    }
}