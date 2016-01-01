using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Dicom.Imaging;

namespace dcmdir2dcm.IO
{
    /// <summary>
    /// Provides method for loading all the dicom images from given directory.
    /// </summary>
    public class DicomFileLoader
    {
        /// <summary>
        /// Loads all dicom images from the given <see cref="directory"/>.
        /// </summary>
        /// <param name="directory">Directory containing the dicom images</param>
        /// <exception cref="ArgumentNullException"><paramref name="directory"/> is null</exception>
        /// <returns>Collection of all loaded dicom images.</returns>
        public IEnumerable<DicomImage> LoadImages(DirectoryInfo directory)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }
            
            return LoadImages(directory.GetFiles("*.*", SearchOption.AllDirectories));
        }


        /// <summary>
        /// Loads dicom images from the given collection of <paramref name="files"/>.
        /// </summary>
        /// <param name="files">Collection containing all the files to be loaded</param>
        /// <exception cref="ArgumentNullException"><paramref name="files"/> is null</exception>
        /// <returns>Collection of all loaded dicom images.</returns>
        public IEnumerable<DicomImage> LoadImages(IEnumerable<FileInfo> files)
        {
            if (files == null)
            {
                throw new ArgumentNullException(nameof(files));
            }

            return files.Select(file =>
            {
                try
                {
                    return new DicomImage(file.FullName);
                }
                catch (Exception)
                {
                    // Images that could not be loaded are skipped
                    return null;
                }
            }).Where(c => c != null);
        }
    }
}