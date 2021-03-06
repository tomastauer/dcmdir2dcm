﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using dcmdir2dcm.IO;

using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Mathematics;
using Dicom.IO.Buffer;

namespace dcmdir2dcm.Lib
{
    /// <summary>
    /// Provides method for composing collection of DICOM images into single multiframe dicom image.
    /// </summary>
    public class DicomImageComposer : IDicomImageComposer
    {
        /// <summary>
        /// Composes all the dicom images from given <paramref name="dicomDirPath"/> to single dicom multiframe file and stores it in location provided by <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="dicomDirPath">Path to input directory containing all the dicom images to be composed</param>
        /// <param name="destinationPath">Destination path when the composed image should be saved</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="dicomDirPath"/> is null</para>
        /// <para>or</para>
        /// <para><paramref name="destinationPath"/> is null</para>
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">Directory with location <paramref name="dicomDirPath"/> was not found</exception>
        public void Compose(string dicomDirPath, string destinationPath)
        {
            if (dicomDirPath == null)
            {
                throw new ArgumentNullException(nameof(dicomDirPath));
            }
            if (destinationPath == null)
            {
                throw new ArgumentNullException(nameof(destinationPath));
            }

            var fileLoader = new DicomFileLoader();
            var input = fileLoader.LoadImages(new DirectoryInfo(dicomDirPath)).ToList();

            ComposeInternal(input, destinationPath);
        }


        /// <summary>
        /// Composes all the dicom images from given <paramref name="dicomFilePaths"/> to single dicom multiframe file and stores it in location provided by <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="dicomFilePaths">Collection of paths refering to all dicom images to be composed</param>
        /// <param name="destinationPath">Destination path when the composed image should be saved</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="dicomFilePaths"/> is null</para>
        /// <para>or</para>
        /// <para><paramref name="destinationPath"/> is null</para>
        /// </exception>
        public void Compose(IEnumerable<string> dicomFilePaths, string destinationPath)
        {
            if (dicomFilePaths == null)
            {
                throw new ArgumentNullException(nameof(dicomFilePaths));
            }
            if (destinationPath == null)
            {
                throw new ArgumentNullException(nameof(destinationPath));
            }

            var fileLoader = new DicomFileLoader();
            var input = fileLoader.LoadImages(dicomFilePaths.Select(filePath => new FileInfo(filePath))).ToList();

            ComposeInternal(input, destinationPath);
        }


        /// <summary>
        /// Composes all the dicom images from given <paramref name="dicomDirPath"/> to single dicom multiframe file and returns a stream containing the result.
        /// </summary>
        /// <param name="dicomDirPath">Path to input directory containing all the dicom images to be composed</param>
        /// <returns>Stream containing the composition result</returns>
        public Stream Compose(string dicomDirPath)
        {
            var fileLoader = new DicomFileLoader();
            var input = fileLoader.LoadImages(new DirectoryInfo(dicomDirPath)).ToList();

            var stream = new MemoryStream();
            new DicomFile(ComposeImages(input).Dataset).Save(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }


        /// <summary>
        /// Composes all the dicom images from given <paramref name="dicomFilePaths"/> to single dicom multiframe file and returns a stream containing the result.
        /// </summary>
        /// <param name="dicomFilePaths">Collection of paths refering to all dicom images to be composed</param>
        /// <returns>Stream containing the composition result</returns>
        public Stream Compose(IEnumerable<string> dicomFilePaths)
        {
            var fileLoader = new DicomFileLoader();
            var input = fileLoader.LoadImages(dicomFilePaths.Select(filePath => new FileInfo(filePath))).ToList();

            var stream = new MemoryStream();
            new DicomFile(ComposeImages(input).Dataset).Save(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }


        /// <summary>
        /// Composes all the dicom images given in <paramref name="inputImage"/> to single dicom multiframe file and stores it in location provided by <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="inputImage">Collection of dicom images to be composed</param>
        /// <param name="destinationPath">Destination path when the composed image should be saved</param>
        private void ComposeInternal(IList<DicomImage> inputImage, string destinationPath)
        {
            var fileSaver = new DicomFileSaver();
            var output = ComposeImages(inputImage);
            fileSaver.SaveImage(new FileInfo(destinationPath), output);
        }


        /// <summary>
        /// Composes the given images into single multiframe Dicom image. Images has to be from the same study with the same attributes.
        /// </summary>
        /// <param name="images">RawImage to be composed</param>
        /// <exception cref="ArgumentNullException"><paramref name="images"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="images"/> is empty</exception>
        /// <returns>Multiframe Dicom image containing all the input images</returns>
        private DicomImage ComposeImages(IList<DicomImage> images)
        {
            if (images == null)
            {
                throw new ArgumentNullException(nameof(images));
            }

            if (images.Count == 0)
            {
                throw new ArgumentException("Collection is empty", nameof(images));
            }

            var sorted = SortInputImages(images);
            
            var baseImage = sorted.First();
            var baseDataSet = baseImage.Dataset;
            
            var baseFrame = baseImage.PixelData.GetFrame(0);
            var pixelData = DicomPixelData.Create(baseDataSet, true);

            pixelData.AddFrame(baseFrame);

            foreach (var image in sorted.Skip(1))
            {
                var data = image.PixelData.GetFrame(0).Data;
                pixelData.AddFrame(new MemoryByteBuffer(data));
            }

            return new DicomImage(baseDataSet);
        }

        
        /// <summary>
        /// Sorts input images ascending with respect to the distance of slices from the initial position.
        /// </summary>
        /// <param name="images">Images collection to be sorted</param>
        /// <exception cref="ArgumentNullException"><paramref name="images"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="images"/> is empty</exception>
        /// <returns>Images sorted by distance of the slice from the initial position</returns>
        private List<OrderedDicomImage> SortInputImages(ICollection<DicomImage> images)
        {
            // Return only the first image if no other images are present
            if (images.Count == 1)
            {
                return new List<OrderedDicomImage>
                {
                    new OrderedDicomImage(images.First())
                };
            }

            var orientationNormal = GetOrientationNormal(images.First());

            var result = images.Select(image => new OrderedDicomImage(image)
            {
                Distance = GetSliceDistance(image, orientationNormal)
            });

            return result.OrderBy(image => image.Distance).ToList();
        }


        /// <summary>
        /// Gets distance of the slice from the initial patient position. Usable for slice ordering.
        /// </summary>
        /// <param name="image">Source image</param>
        /// <param name="orientationNormal">Orientation normal vector obtained from the image patient orientation</param>
        /// <returns>Distance in mm between the current image slice and the initial position</returns>
        private double GetSliceDistance(DicomImage image, Vector3D orientationNormal)
        {
            var ipp = image.Dataset.Get<double[]>(DicomTag.ImagePositionPatient);

            return
                orientationNormal.X * ipp[0] +
                orientationNormal.Y * ipp[1] +
                orientationNormal.Z * ipp[2];
        }


        /// <summary>
        /// Return vector of the orientation normal with the respect to the first rows and column cosines obtained from the RawImage Orientation Patient.
        /// </summary>
        /// <param name="image">Source image</param>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> is null</exception>
        /// <returns>3D vector of the orientation normal</returns>
        private Vector3D GetOrientationNormal(DicomImage image)
        {
            var iop = image.Dataset.Get<double[]>(DicomTag.ImageOrientationPatient);

            var firstRowCosine = new Vector3D(iop[0], iop[1], iop[2]);
            var firstColumnCosine = new Vector3D(iop[3], iop[4], iop[5]);

            return new Vector3D(
                firstRowCosine.Y * firstColumnCosine.Z - firstRowCosine.Z * firstColumnCosine.Y,
                firstRowCosine.Z * firstColumnCosine.X - firstRowCosine.X * firstColumnCosine.Z,
                firstRowCosine.X * firstColumnCosine.Z - firstRowCosine.Z * firstColumnCosine.X
            );
        }
    }
}