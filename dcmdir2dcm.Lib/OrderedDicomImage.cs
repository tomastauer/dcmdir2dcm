using Dicom;
using Dicom.Imaging;

namespace dcmdir2dcm.Lib
{
    /// <summary>
    /// Represents ordered Dicom image. Contains additional property <see cref="Distance"/> specifying image distance in Z-axis from the origin.
    /// </summary>
    internal class OrderedDicomImage : DicomImage
    {
        /// <summary>
        /// Instantiates new instance of <see cref="OrderedDicomImage"/>.
        /// </summary>
        /// <param name="dataset">Dataset containing Dicom image data</param>
        /// <param name="frame">Number of the frame</param>
        public OrderedDicomImage(DicomDataset dataset, int frame = 0) : base(dataset, frame)
        {
        }


        /// <summary>
        /// Instantiates new instance of <see cref="OrderedDicomImage"/>.
        /// </summary>
        /// <param name="fileName">File path to the Dicom file</param>
        /// <param name="frame">Number of the frame</param>
        public OrderedDicomImage(string fileName, int frame = 0) : base(fileName, frame)
        {
        }


        /// <summary>
        /// Instantiates new instance of <see cref="OrderedDicomImage"/>.
        /// </summary>
        /// <param name="image">Original Dicom image</param>
        public OrderedDicomImage(DicomImage image) : base(image.Dataset, 0)
        {
        }


        /// <summary>
        /// Gets or sets the distance of the image in Z-axis from the origin.
        /// </summary>
        public double Distance
        {
            get;
            set;
        }
    }
}