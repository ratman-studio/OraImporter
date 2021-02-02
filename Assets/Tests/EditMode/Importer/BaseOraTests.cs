using System.IO;
using System.Xml.Serialization;
using com.szczuro.importer;
using com.szczuro.importer.ora;
using NUnit.Framework;


namespace com.szczuro.OraImport.Test
{
    [TestFixture]
    public class BaseOraTests
    {
        string filename = "Assets/Tests/EditMode/Importer/files/ora/LibertyBell.ora";
        [Test]
        public void CreatedFile_exits()
        {   
            var file = MultiLayerFileFactory.CreteFileFromPath(filename);
            Assert.NotNull(file);
        }
        
        
        [Test]
        public void CreatedFile_HasLayers()
        {
            var file = MultiLayerFileFactory.CreteFileFromPath(filename);
            var layers = file.GetLayers();
            Assert.NotNull(layers);
            Assert.AreEqual(5, layers.Count);
        }
        [Test]
        public void CreatedFile_HasProperLayerName()
        {
            var file = MultiLayerFileFactory.CreteFileFromPath(filename);
            var layers = file.GetLayers();
            
            Assert.AreEqual("horsShoe", layers[0].name);
        }

        [Test]
        public void not_valid_compositeOp_return_valid_Blending()
        {
            //ARRANGE
            var notValidCompositeOp = "svg:not-valid";

            //ACT
            var blending = Ora.GETBlendingFromCompositeOp(notValidCompositeOp);
            //ASSERT
            Assert.AreEqual(blending, Ora.Blending.Normal);
        }

        [Test]
        public void not_valid_compositeOp_return_valid_Compositing()
        {
            //ARRANGE
            var notValidCompositeOp = "svg:not-valid";
            //ACT
            var compositing = Ora.GETCompositingOperatorFromCompositeOp(notValidCompositeOp);
            //ASSERT
            Assert.AreEqual(compositing, Ora.Compositing.SourceOver);
        }
    }

    public class StackParsingTest
    {
        [Test]
        public void simple_stackXml_parsing_test()
        {
            //ARRANGE
            var serializer = new XmlSerializer(typeof(OraXMLMain));
            OraXMLMain i;
            var filename = "Assets/Tests/EditMode/Importer/files/ora/stack.xml";
            

            //ACT
            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                i = (OraXMLMain) serializer.Deserialize(reader);
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