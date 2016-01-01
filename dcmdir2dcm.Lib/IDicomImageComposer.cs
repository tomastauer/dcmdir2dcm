using System;
using System.Collections.Generic;
using System.IO;

namespace dcmdir2dcm.Lib
{
    /// <summary>
    /// Provides method for composing collection of DICOM images into single multiframe dicom image.
    /// </summary>
    public interface IDicomImageComposer
    {
        /// <summary>
        /// Composes all the dicom images from given <paramref name="dicomDirPath"/> to single dicom multiframe file and stores it in location provided by <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="dicomDirPath">Path to input directory containing all the dicom images to be composed</param>
        /// <param name="destinationPath">Destination path when the composed image should be saved</param>
        void Compose(string dicomDirPath, string destinationPath);


        /// <summary>
        /// Composes all the dicom images from given <paramref name="dicomFilePaths"/> to single dicom multiframe file and stores it in location provided by <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="dicomFilePaths">Collection of paths refering to all dicom images to be composed</param>
        /// <param name="destinationPath">Destination path when the composed image should be saved</param>
        void Compose(IEnumerable<string> dicomFilePaths, string destinationPath);
    }
}