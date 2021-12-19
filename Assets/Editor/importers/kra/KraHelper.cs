using System.Collections.Generic;
using System.Xml.Serialization;

namespace studio.ratman.importer
{
    /// <summary> constants, enums and statics for kra files </summary>
    public static class Kra
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
            { SrcOver, Blending.Normal },
            { Multiply, Blending.Multiply },
            { Screen, Blending.Screen },
            { Overlay, Blending.Overlay },
            { Darken, Blending.Darken },
            { Lighten, Blending.Lighten },
            { ColorDodge, Blending.ColorDodge },
            { ColorBurn, Blending.ColorBurn },
            { HardLight, Blending.HardLight },
            { SoftLight, Blending.SoftLight },
            { Difference, Blending.Difference },
            { Color, Blending.Color },
            { Luminosity, Blending.Luminosity },
            { Hue, Blending.Hue },
            { Saturation, Blending.Saturation },
            { Plus, Blending.Normal },
            { DstOut, Blending.Normal },
            { DstIn, Blending.Normal },
            { SrcAtop, Blending.Normal },
            { DstAtop, Blending.Normal }
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


    public class KraXML
    {
        /// <summary>
        /// kra stack xml parser helper
        /// </summary>
        ///
        public class MainDoc
        {
            /*    
            <DOC xmlns="http://www.calligra.org/DTD/krita" kritaVersion="4.3.0" editor="Krita" syntaxVersion="2">
                <IMAGE colorspacename="RGBA" description="" mime="application/x-kra" profile="sRGB-elle-V2-srgbtrc.icc" y-res="100" width="128" x-res="100" height="128" name="Unnamed">
                    <layers>
                       <layer locked="0" colorlabel="0" colorspacename="RGBA" onionskin="0" visible="1" channellockflags="1111" compositeop="normal" name="Layer 14" selected="true" filename="layer2" nodetype="paintlayer" y="0" opacity="255" x="0" channelflags="" intimeline="1" collapsed="0" uuid="{862360e3-2524-470f-ac48-6549e6704e4d}"/>
                       <layer colorlabel="0" channellockflags="1111" colorspacename="RGBA" compositeop="normal" onionskin="0" opacity="255" collapsed="0" channelflags="" intimeline="1" x="0" nodetype="paintlayer" uuid="{65db28fc-b9db-453e-a3f3-1230477966aa}" filename="layer3" visible="0" y="0" locked="0" name="Layer 13"/>
                       <layer colorlabel="0" channellockflags="1111" colorspacename="RGBA" compositeop="normal" onionskin="0" opacity="255" collapsed="0" channelflags="" intimeline="1" x="0" nodetype="paintlayer" uuid="{53326e8b-cda0-44c1-b0dc-02fbf81f58c1}" filename="layer4" visible="0" y="0" locked="0" name="Layer 12"/>
                       <layer colorlabel="0" channellockflags="1111" colorspacename="RGBA" compositeop="normal" onionskin="0" opacity="255" collapsed="0" channelflags="" intimeline="1" x="0" nodetype="paintlayer" uuid="{b22de5f1-9403-4a5d-ae12-321c00efddde}" filename="layer5" visible="0" y="0" locked="0" name="Layer 11"/>
                       <layer colorlabel="0" channellockflags="1111" colorspacename="RGBA" compositeop="normal" onionskin="0" opacity="255" collapsed="0" channelflags="" intimeline="1" x="0" nodetype="paintlayer" uuid="{d1f1f805-9939-4320-ac0d-b35dee9aa58a}" filename="layer6" visible="0" y="0" locked="0" name="Layer 2"/>
                       <layer colorlabel="0" channellockflags="1111" colorspacename="RGBA" compositeop="normal" onionskin="0" opacity="255" collapsed="0" channelflags="" intimeline="1" x="0" nodetype="paintlayer" uuid="{ea95b43d-4182-4744-b26e-7b7b8e58299f}" filename="layer7" visible="1" y="0" locked="0" name="Background"/>
                    </layers>
                    <ProjectionBackgroundColor ColorData="AAAAAA=="/>
                    <GlobalAssistantsColor SimpleColorData="176,176,176,255"/>
                    <Palettes/>
                    <animation>
                       <framerate value="24" type="value"/>
                       <range from="0" to="100" type="timerange"/>
                       <currentTime value="0" type="value"/>
                  </animation>
                </IMAGE>
            </DOC>
            */

            [XmlRoot("doc")]
            public class Doc
            {
                [XmlAttribute("kritaVersion")] public string KritaVersion; // "4.3.0"
                [XmlAttribute("editor")] public string Krita; // "Krita"
                [XmlAttribute("syntaxVersion")] public int SyntaxVersion; // "2"
                [XmlElement("image")] public ImageDto Image;
            }

            public class ImageDto
            {
                [XmlAttribute("colorspacename")] public string Colorspacename; // "RGBA"
                [XmlAttribute("description")] public string Description;
                [XmlAttribute("mime")] public string Mime; //"application/x-kra"
                [XmlAttribute("profile")] public string Profile; //"sRGB-elle-V2-srgbtrc.icc"

                [XmlAttribute("y-res")] public int YRes;
                [XmlAttribute("x-res")] public int XRes;

                [XmlAttribute("width")] public int Width;
                [XmlAttribute("height")] public int Height;

                [XmlAttribute("name")] public string Name; // Unamed

                [XmlElement("stack")] public LayersDto[] LayerGroup;
            }

            public struct LayersDto
            {
                [XmlElement("layer")] public LayerDto[] Layers;
            }


            // <layer locked="0" colorlabel="0" colorspacename="RGBA" onionskin="0" visible="1" channellockflags="1111" compositeop="normal" name="Layer 14" selected="true" filename="layer2" nodetype="paintlayer" y="0" opacity="255" x="0" channelflags="" intimeline="1" collapsed="0" uuid="{862360e3-2524-470f-ac48-6549e6704e4d}"/>
            public struct LayerDto
            {
                [XmlAttribute("colorspacename")] public string ColorSpaceName; //"RGBA"
                [XmlAttribute("opacity")] public float Opacity;
                [XmlAttribute("x")] public int X;
                [XmlAttribute("y")] public int Y;
                [XmlAttribute("compositeop")] public string CompositeOp; //"normal"
                [XmlAttribute("name")] public string Name; //"Layer 14"
                [XmlAttribute("filename")] public string FileName; //"Layer2"

                [XmlAttribute("nodetype")] public bool NodeType; //"paintlayer"

                [XmlAttribute("uuid")] public string UUID; //"{862360e3-2524-470f-ac48-6549e6704e4d}"

                [XmlAttribute("selected")] public bool Selected; //"true"
                [XmlAttribute("locked")] public int Locked; //"0"
                [XmlAttribute("visible")] private bool _visibility;
                [XmlAttribute("onionskin")] public int OnionSkin; //"0"
                [XmlAttribute("collapsed")] public string Collapsed; //"0"
                [XmlAttribute("intimeline")] public int InTimeLine; //"1"

                [XmlAttribute("colorlabel")] public int ColorLabel; // "0"
                [XmlAttribute("channellockflags")] public string ChannelLockFlags; //"1111"
                [XmlAttribute("channelflags")] public string ChannelFlags; //""

                [XmlAttribute("src")] public string Src;
            }

            public static string GetNameFromTexture(Doc xml, string textureName)
            {
                foreach (var group in xml.Image.LayerGroup)
                {
                    foreach (var layer in group.Layers)
                    {
                        if (textureName == layer.Src)
                        {
                            return layer.Name;
                        }
                    }
                }

                return textureName;
            }
        }
    }
}