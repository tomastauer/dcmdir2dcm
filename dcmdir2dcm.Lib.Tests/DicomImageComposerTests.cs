using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using NUnit.Framework;

namespace dcmdir2dcm.Lib.Tests
{
    [TestFixture]
    public class DicomImageComposerTests
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
        public void Compose_FromDirectory_ToFile_ComposesInCorrectOrder([Values("CorrectOrder", "WrongOrder1", "WrongOrder2")] string assetDirectory)
        {
            // Arrange
            var composer = new DicomImageComposer();
            string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            byte[] expectedHash = GetMD5Hash(Path.Combine(assetsPath, "expectedResult.dcm"));

            // Act
            composer.Compose(Path.Combine(assetsPath, assetDirectory), "result.dcm");
            byte[] composedHash = GetMD5Hash("result.dcm");

            // Assert
            Assert.That(expectedHash, Is.EqualTo(composedHash));
        }

        [Test]
        public void Compose_FromDirectory_ToStream_ComposesInCorrectOrder([Values("CorrectOrder", "WrongOrder1", "WrongOrder2")] string assetDirectory)
        {
            // Arrange
            var composer = new DicomImageComposer();
            string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            byte[] expectedHash = GetMD5Hash(Path.Combine(assetsPath, "expectedResult.dcm"));

            // Act
            byte[] composedHash;
            using (var result = composer.Compose(Path.Combine(assetsPath, assetDirectory)))
            {
                composedHash = GetMD5Hash(result);
            }

            // Assert
            Assert.That(expectedHash, Is.EqualTo(composedHash));
        }



        [Test]
        public void Compose_FromFilesList_ToFile_ComposesInCorrectOrder([Values("CorrectOrder", "WrongOrder1", "WrongOrder2")] string assetDirectory)
        {
            // Arrange
            var composer = new DicomImageComposer();
            string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            byte[] expectedHash = GetMD5Hash(Path.Combine(assetsPath, "expectedResult.dcm"));
            var files = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", assetDirectory)).GetFiles().Select(file => file.FullName);

            // Act
            composer.Compose(files, "result.dcm");
            byte[] composedHash = GetMD5Hash("result.dcm");

            // Assert
            Assert.That(expectedHash, Is.EqualTo(composedHash));
        }


        [Test]
        public void Compose_FromFilesList_ToStream_ComposesInCorrectOrder([Values("CorrectOrder", "WrongOrder1", "WrongOrder2")] string assetDirectory)
        {
            // Arrange
            var composer = new DicomImageComposer();
            string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            byte[] expectedHash = GetMD5Hash(Path.Combine(assetsPath, "expectedResult.dcm"));
            var files = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", assetDirectory)).GetFiles().Select(file => file.FullName);

            // Act
            byte[] composedHash;
            using (var result = composer.Compose(files))
            {
                composedHash = GetMD5Hash(result);
            }
                
            // Assert
            Assert.That(expectedHash, Is.EqualTo(composedHash));
        }

        
        private byte[] GetMD5Hash(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }


        private byte[] GetMD5Hash(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(stream);
            }
        }
    }
}
