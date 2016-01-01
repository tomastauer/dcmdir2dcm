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

            var filteredFiles = directory.GetFiles("*.*", SearchOption.AllDirectories);
            return filteredFiles.Select(file =>
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
            }).Where(c=>c!=null);
        }
    }
}
