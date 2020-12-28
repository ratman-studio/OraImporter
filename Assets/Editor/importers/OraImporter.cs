using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

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
            
            //file helper
            Debug.Log($"Create Orafile {_oraFile}");
            _oraFile = new OraFile(path);
            

            // Register root prefab that will be visible in project window instead of file
            var filePrefab = registerMainPrefab(ctx, fileInfo.Name, _oraFile.getThumbnailSprite().texture);
            
            // storage place for sprites  
            Debug.Log("Create spritelib");
            var spritesLib = _oraFile.getLayers();
            Debug.Log($"SpriteLib length {spritesLib.Count}");

            //ctx.AddObjectToAsset("spriteLib", spritesLib);
            Debug.Log($"add spriteRenderers to prefab");
            addSpritesToPrefab(ctx, filePrefab, spritesLib);
            
            Debug.Log($"set main prefab");
            ctx.SetMainObject(filePrefab);
        }

        private void addSpritesToPrefab(AssetImportContext ctx, GameObject filePrefab, List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                var texGO = new GameObject(sprite.name);
                var spriteRenderer = texGO.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                Debug.Log(spriteRenderer.sprite);
                ctx.AddObjectToAsset(sprite.name,sprite);
                //texGO.transform.SetParent(filePrefab.transform);
            }
        }
        
        private static GameObject registerMainPrefab(AssetImportContext ctx, string name, Texture2D thumbNail)
        {
            
            var filePrefab = new GameObject($"{name}_GO");
            ctx.AddObjectToAsset("main", filePrefab, thumbNail);
            
            return filePrefab;
        }

        #endregion
    }
}