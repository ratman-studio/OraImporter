using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.szczuro.importer.ora
{
    // <image version="0.0.1" yres="100" xres="100" w="128" h="128">
    // <stack opacity="1" composite-op="svg:src-over" y="0" name="root" isolation="isolate" visibility="visible" x="0">
    // <layer opacity="1" composite-op="svg:src-over" y="16" name="Layer 11" src="data/layer2.png" visibility="hidden" x="27"/>
    // <layer opacity="1" composite-op="svg:src-over" y="25" name="Layer 2" src="data/layer1.png" visibility="hidden" x="25"/>
    // <layer opacity="1" composite-op="svg:src-over" y="0" name="Background" src="data/layer0.png" visibility="visible" x="0"/>
    // </stack>
    // </image>

    
    
    public class Ora
    {
        // https://www.openraster.org/baseline/layer-stack-spec.html#introduction
        public enum Visibility
        {
            Visible,
            Hidden
        }
        
        // ora blending functio as in specification
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
        
        // cpmpositie-op strings  
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
        
        // cpmpositie to blending map 
        private static Dictionary<string, Blending> blending_dict = new Dictionary<string, Blending>()
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

        
        public static Blending getBlendingFromCompositeOp(string composite)
        {
            var result = Blending.Normal;
            blending_dict.TryGetValue(composite, out result);
            return result;
        }

        public static Compositing getCompositingOperatorFromCompositeOp(string compositeOp)
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
    public class OraXMLImage
    {
        [XmlAttribute("version")]
        public string Version;
        [XmlAttribute("yres")]
        public int YRes;
        [XmlAttribute("xres")]
        public int XRes;
        [XmlAttribute("w")]
        public int Width;
        [XmlAttribute("h")]
        public int Height;
        
        [XmlElement("stack")]
        public OraXMLStack stack;
        public struct OraXMLStack
        {
            [XmlAttribute("name")]
            public string Name;
            [XmlAttribute("opacity")]
            public float Opacity;
            [XmlAttribute("x")]
            public int X;

            [XmlAttribute("y")]
            public int Y;
            
            [XmlAttribute("visibility")]
            private string _visibility;
        
            public Ora.Visibility Visibility
            {
                get =>_visibility.Equals("visible") ? Ora.Visibility.Visible : Ora.Visibility.Hidden;
                set => _visibility = value == Ora.Visibility.Visible ? "visible" : "hidden";
            }

            [XmlAttribute("composite-op")]
            private string compositeOp;
        }

        struct LayerTag
        {
        }
    }

    internal interface IMultiLayerFile
    {
        Sprite getThumbnailSprite();
        Sprite getMergedLayers();
        List<Sprite> getLayers();
    }

    /// <summary> this class unzip ora file and parse all items and provide ILayeredImageFile </summary>
    [Serializable]
    internal class OraFile : IMultiLayerFile
    {
        [SerializeField] public List<Sprite> layers = new List<Sprite>();
        [SerializeField] private Sprite thumbnail;
        [SerializeField] private Sprite mergedLayers;

        private const string ThumbnailName = "thumbnail.png";
        private const string MergeLayersName = "mergedimage.png";
        public OraFile(string path)
        {
            Debug.Log($"Ora import {path}");
            var textureList = getTextureList(path);
            foreach (var tex in textureList) layers.Add(SpriteFromTexture(tex));

            //spritesLib = GenerateSpriteList(layers);
            thumbnail = findSpriteByName(ThumbnailName);
            mergedLayers = findSpriteByName(MergeLayersName);
        }

        public Sprite getThumbnailSprite()
        {
            if (thumbnail == null)
            {
                Debug.LogWarning("Thumbnail not found searching ... ");
                thumbnail = findSpriteByName(ThumbnailName);
            }

            return thumbnail;
        }

        public Sprite getMergedLayers()
        {
            if (mergedLayers == null)
            {
                Debug.LogWarning("Merged Layers not found searching ...");
                mergedLayers = findSpriteByName(MergeLayersName);
            }

            return mergedLayers;
        }

        private Sprite findSpriteByName(string imageName)
        {
            foreach (var sprite in layers)
                if (sprite.name == imageName)
                    return sprite;
            Debug.LogWarning($"{imageName} search failed");
            return null;
        }

        public List<Sprite> getLayers()
        {
            return layers;
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

        #region ZipReader

        private static List<Texture2D> getTextureList(string zipPath)
        {
            var archives = new List<Texture2D>();
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                Debug.Log($"{archive.Mode} archive {zipPath}");
                foreach (var entry in archive.Entries)
                    if (entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                        archives.Add(getTextureFromEntry(entry));
                    else
                        Debug.LogWarning($"skip entry {entry}");
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