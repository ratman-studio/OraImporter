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
            
            var imageList = oraFile.layers;
            foreach (var texture in imageList)
            {
                var tex_go = new GameObject(texture.name);
                tex_go.transform.SetParent(filePrefab.transform);
            }
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
        public List<Texture2D> layers;
        public string path;

        public OraFile (string path)
        {
            Debug.Log($"Ora import {path}");
            var zipList = getZipList(path);
            foreach (var entry in zipList)
            {
                //layers.Add(ZipArchiveEntryToTexure2D(entry));                
            }
        }

        private Texture2D ZipArchiveEntryToTexure2D(ZipArchiveEntry entry)
        {
            Debug.Log($"ZipArch Entry to Tex2d entry: {entry.Name} \n {entry}");
            return new Texture2D(1, 1);
        }

        #region ZipReader

        private static List<Texture2D> getZipList(string zipPath)
        {
            var archives = new List<Texture2D>();
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                Debug.Log($"open archive {archive.Mode} {zipPath}");
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    Debug.Log($"parse entry {entry}");
                    
                    var texture = new Texture2D(2, 2);
                    //using (var  fileStream =  new FileStream(entry.FullName, FileMode.OpenOrCreate, FileAccess.Read))
                    
                    using (var fileStream = entry.Open())
                    {
                        Debug.Log(entry.Length);
                        Debug.Log(entry.Archive);
                        
                        //Debug.Log(getMimeFromStream(new FileStream(fileStream, FileAccess.Read)));
                        // Debug.Log($"fileStream {fileStream} ");
                        byte[] imageData = new byte[entry.Length];
                        fileStream.Read(imageData, 0, (int) entry.Length);
                        texture.LoadImage(imageData);
                        archives.Add(texture);
                        Debug.Log($"fileStream complete {texture.format} ");
                    }
                }
            }
            return archives;
        }
       
        #endregion

    }
}