using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO.Pipes;
using System.Xml.Serialization;
using UnityEngine;

namespace com.szczuro.importer.ora
{
    
    //////// Ora File example: 
    // <image version="0.0.1" yres="100" xres="100" w="128" h="128">
    // <stack opacity="1" composite-op="svg:src-over" y="0" name="root" isolation="isolate" visibility="visible" x="0">
    // <layer opacity="1" composite-op="svg:src-over" y="16" name="Layer 11" src="data/layer2.png" visibility="hidden" x="27"/>
    // <layer opacity="1" composite-op="svg:src-over" y="25" name="Layer 2" src="data/layer1.png" visibility="hidden" x="25"/>
    // <layer opacity="1" composite-op="svg:src-over" y="0" name="Background" src="data/layer0.png" visibility="visible" x="0"/>
    // </stack>
    // </image>

    /// <summary> constants, enums and statics for ora files </summary>
    public static class Ora
    {
        // https://www.openraster.org/baseline/layer-stack-spec.html#introduction
        public enum Visibility
        {
            Visible,
            Hidden
        }

        // ora blending function as in specification
        // https://www.openraster.org/baseline/layer-stack-spec.html#introduction
        public enum Blending
        {
            Normal,
            Multiply,
            Screen,
            Overlay,
            Darken,
            Lighten,
            ColorDodge,
            ColorBurn,
            HardLight,
            SoftLight,
            Difference,
            Color,
            Luminosity,
            Hue,
            Saturation
        }

        public enum Compositing
        {
            SourceOver,
            Lighter,
            DestinationIn,
            DestinationOut,
            SourceAtop,
            DestinationAtop
        }

        // composite-op strings  
        private const string SrcOver = "svg:src-over"; // Normal, Source Over
        private const string Multiply = "svg:multiply"; // Multiply, Source Over
        private const string Screen = "svg:screen"; // Screen, Source Over
        private const string Overlay = "svg:overlay"; // Overlay, Source Over
        private const string Darken = "svg:darken"; // Darken, Source Over
        private const string Lighten = "svg:lighten"; // Lighten, Source Over
        private const string ColorDodge = "svg:color-dodge"; // Color Dodge, Source Over
        private const string ColorBurn = "svg:color-burn"; // Color Burn, Source Over
        private const string HardLight = "svg:hard-light"; // Hard Light, Source Over
        private const string SoftLight = "svg:soft-light"; // Soft Light, Source Over
        private const string Difference = "svg:difference"; // Difference, Source Over
        private const string Color = "svg:color"; // Color, Source Over
        private const string Luminosity = "svg:luminosity"; // Luminosity, Source Over
        private const string Hue = "svg:hue"; // Hue, Source Over
        private const string Saturation = "svg:saturation"; // Saturation, Source Over
        private const string Plus = "svg:plus"; // Normal, Lighter
        private const string DstIn = "svg:dst-in"; // Normal, Destination In
        private const string DstOut = "svg:dst-out"; // Normal, Destination Out
        private const string SrcAtop = "svg:src-atop"; // Normal, Source Atop
        private const string DstAtop = "svg:dst-atop"; // Normal, Destination Atop

        // composite to blending map 
        private static readonly Dictionary<string, Blending> BlendingDict = new Dictionary<string, Blending>()
        {
            {SrcOver, Blending.Normal},
            {Multiply, Blending.Multiply},
            {Screen, Blending.Screen},
            {Overlay, Blending.Overlay},
            {Darken, Blending.Darken},
            {Lighten, Blending.Lighten},
            {ColorDodge, Blending.ColorDodge},
            {ColorBurn, Blending.ColorBurn},
            {HardLight, Blending.HardLight},
            {SoftLight, Blending.SoftLight},
            {Difference, Blending.Difference},
            {Color, Blending.Color},
            {Luminosity, Blending.Luminosity},
            {Hue, Blending.Hue},
            {Saturation, Blending.Saturation},
            {Plus, Blending.Normal},
            {DstOut, Blending.Normal},
            {DstIn, Blending.Normal},
            {SrcAtop, Blending.Normal},
            {DstAtop, Blending.Normal}
        };


        public static Blending GETBlendingFromCompositeOp(string composite)
        {
            BlendingDict.TryGetValue(composite, out var result);
            return result;
        }

        public static Compositing GETCompositingOperatorFromCompositeOp(string compositeOp)
        {
            switch (compositeOp)
            {
                case Plus: return Compositing.Lighter;
                case DstIn: return Compositing.DestinationIn;
                case DstOut: return Compositing.DestinationOut;
                case SrcAtop: return Compositing.SourceAtop;
                case DstAtop: return Compositing.DestinationAtop;

                default:
                    return Compositing.SourceOver;
            }
        }
    }

    [XmlRoot("image")]
    public class OraXMLMain
    {
        [XmlAttribute("version")] public string Version;
        [XmlAttribute("yres")] public int YRes;
        [XmlAttribute("xres")] public int XRes;
        [XmlAttribute("w")] public int Width;
        [XmlAttribute("h")] public int Height;

        [XmlElement("stack")] public OraXMLStack[] stacks;

        public struct OraXMLStack
        {
            [XmlAttribute("name")] public string Name;
            [XmlAttribute("opacity")] public float Opacity;
            [XmlAttribute("x")] public int X;

            [XmlAttribute("y")] public int Y;

            [XmlAttribute("visibility")] public string _visibility;
            
            [XmlElement("layer")] public OraXMLLayer[] layers;

            public Ora.Visibility Visibility
            {
                get => _visibility.Equals("visible") ? Ora.Visibility.Visible : Ora.Visibility.Hidden;
                set => _visibility = value == Ora.Visibility.Visible ? "visible" : "hidden";
            }

            [XmlAttribute("composite-op")] private string _compositeOp;
        }

        public struct OraXMLLayer
        {
            [XmlAttribute("name")] public string Name;
            [XmlAttribute("opacity")] public float Opacity;
            [XmlAttribute("x")] public int X;

            [XmlAttribute("y")] public int Y;

            [XmlAttribute("visibility")] private string _visibility;
            [XmlAttribute("src")] public string src;
        }

        public static string GetNameFromTexture(OraXMLMain oraXML, string textureName)
        {
            
            foreach (var stack in oraXML.stacks)
            {
                foreach (var layer in stack.layers)
                {
                    if (textureName == layer.src)
                    {
                        return layer.Name;
                    }
                }
            }

            return textureName;
        }
    }

    
    /// <summary> parse *.ora file and provide it items vie interface ILayeredImageFile </summary>
    [Serializable]
    internal class OraData : IMultiLayerData
    {
        public static OraData CreateFromFile(string path)
        {
            return new OraData(path);
        }

        [SerializeField] public List<Texture2D> layers = new List<Texture2D>();
        [SerializeField] private Texture2D thumbnail;
        [SerializeField] private Texture2D mergedLayers;
        [SerializeField] private OraXMLMain structure;
        
        private const string ThumbnailName = "Thumbnails/thumbnail.png";
        private const string MergeLayersName = "mergedimage.png";

        private OraData(string path)
        {
            Debug.Log($"Ora import {path}");
            
            structure = GetStructure(path);
            layers = GETTextureList(path);
            
            thumbnail = FindSpriteByName(ThumbnailName);
            if (thumbnail) layers.Remove(thumbnail);
            
            mergedLayers = FindSpriteByName(MergeLayersName);
            if (mergedLayers) layers.Remove(mergedLayers);
             
        }
     
        // interface IMultiLayerFile

        public Texture2D GetThumbnail()
        {
            if (thumbnail == null)
            {
                Debug.LogWarning("Thumbnail not found searching ... ");
                thumbnail = FindSpriteByName(ThumbnailName);
            }

            return thumbnail;
        }

        public Texture2D GetMergedLayers()
        {
            if (mergedLayers == null)
            {
                Debug.LogWarning("Merged Layers not found searching ...");
                mergedLayers = FindSpriteByName(MergeLayersName);
            }

            return mergedLayers;
        }
        

        public List<Texture2D> GetLayers()
        {
            return layers;
        }
        
        public string GetTextureName(Texture2D texture)
        {
            if (structure != null)
                return OraXMLMain.GetNameFromTexture(structure, texture.name);
            return texture.name;
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

        

        #region ZipReader

        private static OraXMLMain GetStructure(string zipPath)
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
        
        private static OraXMLMain GetXMLFromEntry(ZipArchiveEntry entry)
        {   
            var serializer = new XmlSerializer(typeof(OraXMLMain));
            using (var fileStream = entry.Open())
            {
                return (OraXMLMain) serializer.Deserialize(fileStream);
                
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
            var texture = new Texture2D(2, 2);
            using (var fileStream = entry.Open())
            {
                var imageData = new byte[entry.Length];
                fileStream.Read(imageData, 0, (int) entry.Length);
                texture.LoadImage(imageData);
                texture.name = entry.FullName;
                
                
            }

            return texture;
        }

        #endregion
    }
}