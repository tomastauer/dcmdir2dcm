using System;
using System.IO;

using Dicom.Imaging;

using NUnit.Framework;

namespace dcmdir2dcm.IO.Tests
{
    [TestFixture]
    public class DicomFileSaverTests
    {
        [TearDown]
        public void TearDown()
        {
            if (File.Exists("result.dcm"))
            {
                File.Delete("result.dcm");
            }
        }

        [Test]
        public void SaveImage_StoresImageCorrectly()
        {
            // Arrange
            var imageToBeStored = new DicomImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/I0"));
            var inputImageData = imageToBeStored.PixelData.GetFrame(0).Data;

            var file = new FileInfo("result.dcm");
            var dicomFileSaver = new DicomFileSaver();
            
            // Act
            dicomFileSaver.SaveImage(file, imageToBeStored);
            var outputImage = new DicomImage("result.dcm");
            var outputImageData = outputImage.PixelData.GetFrame(0).Data;
            
            // Assert
            Assert.That(File.Exists("result.dcm"));
            Assert.That(inputImageData, Is.EqualTo(outputImageData));
        }
    }
}
