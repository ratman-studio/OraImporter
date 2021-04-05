using System.Collections.Generic;
using System.IO.Pipes;
using System.Xml.Serialization;

namespace com.szczuro.importer.ora
{
    
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

    //////// Ora File example: 
    // <image version="0.0.1" yres="100" xres="100" w="128" h="128">
    // <stack opacity="1" composite-op="svg:src-over" y="0" name="root" isolation="isolate" visibility="visible" x="0">
    // <layer opacity="1" composite-op="svg:src-over" y="16" name="Layer 11" src="data/layer2.png" visibility="hidden" x="27"/>
    // <layer opacity="1" composite-op="svg:src-over" y="25" name="Layer 2" src="data/layer1.png" visibility="hidden" x="25"/>
    // <layer opacity="1" composite-op="svg:src-over" y="0" name="Background" src="data/layer0.png" visibility="visible" x="0"/>
    // </stack>
    // </image>
    
    /// <summary>
    /// ora stack xml parser helper
    /// </summary>
    /// 
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
}