using dcmdir2dcm.Lib;

namespace dcmdir2dcm
{
    /// <summary>
    /// Represents start-up point of the console application.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Represents start-up point of the console application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var inputArguments = new InputArguments();
            if (!CommandLine.Parser.Default.ParseArguments(args, inputArguments))
            {
                return;
            }

            var composer = new DicomImageComposer();
            composer.Compose(inputArguments.InputDirectory, inputArguments.OutputFile);
        }
    }
}
