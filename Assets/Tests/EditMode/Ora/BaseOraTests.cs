﻿using System.IO;
using System.Xml.Serialization;
using com.szczuro.importer.ora;
using NUnit.Framework;
using UnityEngine;

namespace com.szczuro.OraImport.Test
{
    [TestFixture]
    public class BaseOraTests
    {
        [Test]
        public void not_valid_compositeOp_return_valid_Blending()
        {
            //ARRANGE
            var notValidCompositeOp = "svg:not-valid";

            //ACT
            var blending = Ora.getBlendingFromCompositeOp(notValidCompositeOp);
            //ASSERT
            Assert.AreEqual(blending, Ora.Blending.Normal);
        }

        [Test]
        public void not_valid_compositeOp_return_valid_Compositing()
        {
            //ARRANGE
            var notValidCompositeOp = "svg:not-valid";
            //ACT
            var compositing = Ora.getCompositingOperatorFromCompositeOp(notValidCompositeOp);
            //ASSERT
            Assert.AreEqual(compositing, Ora.Compositing.SourceOver);
        }
        //
        // [Test]
        // public void read_StackElement_From_XML()
        // {
        //     //ARRANGE
        //     // <stack opacity="1" composite-op="svg:src-over" y="0" name="root" isolation="isolate" visibility="visible" x="0">
        //     var xmlString =
        //         "<stack opacity=\"1\" composite-op=\"svg:src-over\" y=\"0\" name=\"root\" isolation=\"isolate\" visibility=\"visible\" x=\"0\">";
        //     //ACT
        //     var stackElement = OraXMLImage.XMLStackElement.createInstance(xmlString);
        //     //ASSERT
        //     Assert.NotNull(stackElement);
        //     Assert.AreEqual("root", stackElement.name);
        // }
    }

    public class StackParsingTest
    {
        [Test]
        public void simple_stackXml_parsing_test()
        {
            //ARRANGE
            var serializer = new XmlSerializer(typeof(OraXMLImage));
            OraXMLImage i;
            var filename = "Assets/Tests/EditMode/Ora/stack.xml";

            //ACT
            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                i = (OraXMLImage) serializer.Deserialize(reader);
            }

            //ASSERT
            Assert.IsNotNull(i);
            Assert.AreEqual(128,i.Width);
            Assert.AreEqual(128,i.Height);
            Assert.AreEqual(100,i.XRes);
            Assert.AreEqual(100,i.YRes);
            Assert.AreEqual("0.0.1",i.Version);
            
            
        }
    }
}