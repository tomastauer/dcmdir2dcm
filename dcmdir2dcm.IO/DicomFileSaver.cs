using System;
using System.IO;

using Dicom;
using Dicom.Imaging;

namespace dcmdir2dcm.IO
{
    /// <summary>
    /// Provides method for saving dicom image to the file system.
    /// </summary>
    public class DicomFileSaver
    {
        /// <summary>
        /// Saves given <paramref name="image"/> to the provided <see cref="file"/>.
        /// </summary>
        /// <param name="file">File the image is to be saved to</param>
        /// <param name="image">Dicom image to be saved</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="file"/> is null </para>
        /// <para>or</para>
        /// <para><paramref name="image"/> is null</para>
        /// </exception>
        public void SaveImage(FileInfo file, DicomImage image)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var dicomFile = new DicomFile(image.Dataset);
            using (var stream = file.Create())
            {
                dicomFile.Save(stream);
            }
        }
    }
}
