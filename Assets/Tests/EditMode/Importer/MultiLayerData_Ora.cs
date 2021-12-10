using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace studio.ratman.importer.Tests
{
    [TestFixture]
    public class MultiLayerData_Ora
    {
        // setup 
        private const string FILENAME = "Assets/Tests/EditMode/Importer/files/ora/LibertyBell.ora";
        private const int EXPECTED_LAYERS = 5;
        private const string EXPECTED_FILENAME = "LibertyBell";
        private const string EXPECTED_TEXTURE_NAME = "horsShoe";

        [Test]
        public void CreatedFile_exits()
        {
            var multiLayerData = MultiLayerFileFactory.CreteFileFromPath(FILENAME);
            Assert.NotNull(multiLayerData);
        }

        [Test]
        public void GetThumbNail_ret_Texture2d()
        {
            // Arrange
            var multiLayerData = MultiLayerFileFactory.CreteFileFromPath(FILENAME);
            // Act 
            var thumbnail = multiLayerData.GetThumbnail();
            // Assert
            Assert.IsTrue(thumbnail is Texture2D);
        } 
        
        [Test]
        public void GetMergedLayers_ret_Texture2d()
        {
            // Arrange
            var multiLayerData = MultiLayerFileFactory.CreteFileFromPath(FILENAME);
            // Act 
            var mergedLayers = multiLayerData.GetMergedLayers();
            // Assert
            Assert.IsTrue(mergedLayers is Texture2D);
        }

        [Test]
        public void GetLayers_ret_Layers()
        {
            var multiLayerData = MultiLayerFileFactory.CreteFileFromPath(FILENAME);
            var layers = multiLayerData.GetLayers();
            Assert.NotNull(layers);
            Assert.AreEqual(EXPECTED_LAYERS, layers.Count);
        }

        [Test]
        public void GetLayers_ret_ListOfTexture2d()
        {
            var multiLayerData = MultiLayerFileFactory.CreteFileFromPath(FILENAME);
            var layers = multiLayerData.GetLayers();

            Assert.Greater(layers.Count, 0);
            foreach (var layer in layers)
                Assert.NotNull(layer is Texture2D);
        }

        [Test]
        public void GetTextureName_ret_ProperTextureName()
        {
            var multiLayerData = MultiLayerFileFactory.CreteFileFromPath(FILENAME);
            var layers = multiLayerData.GetLayers();
            var textureName = multiLayerData.GetTextureName(layers[0]);
            Assert.AreEqual(EXPECTED_TEXTURE_NAME,textureName);
        }

        [Test]
        public void GetFileName_ret_FileName()
        {
            var multiLayerData = MultiLayerFileFactory.CreteFileFromPath(FILENAME);
            var filename = multiLayerData.GetFileName();
            Assert.AreEqual(EXPECTED_FILENAME, filename);
        }
    }
}