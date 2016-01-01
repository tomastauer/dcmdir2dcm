using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace dcmdir2dcm.IO.Tests
{
    [TestFixture]
    public class DicomFileLoaderTests
    {
        [Test]
        public void LoadImages_FromDirectory_SkipsInvalidFiles()
        {
            // Arrange
            var dicomFileLoader = new DicomFileLoader();
            
            // Act
            var dicomFiles = dicomFileLoader.LoadImages(new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets"))).ToList();

            // Assert
            Assert.That(dicomFiles.Count, Is.EqualTo(3));
        }


        [Test]
        public void LoadImages_FromFilesList_SkipsInvalidFiles()
        {
            // Arrange
            var dicomFileLoader = new DicomFileLoader();
            var files = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets")).GetFiles();
            
            // Act
            var dicomFiles = dicomFileLoader.LoadImages(files).ToList();

            // Assert
            Assert.That(dicomFiles.Count, Is.EqualTo(3));
        }
    }
}
