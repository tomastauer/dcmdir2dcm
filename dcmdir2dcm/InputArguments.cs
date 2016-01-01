using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

namespace dcmdir2dcm
{
    /// <summary>
    /// Represents input arguments passed when starting the program.
    /// </summary>
    internal class InputArguments
    {
        /// <summary>
        /// Specifies name of the input directory. All files within the directory will be used as the composer input.
        /// </summary>
        [Option('i', "input", HelpText = "Specifies name of the input directory. All files within the directory will be used as the composer input.", Required = true)]
        public string InputDirectory
        {
            get;
            set;
        }


        /// <summary>
        /// Specifies name of the output file. Is file already exists, it is overwritten.
        /// </summary>
        [Option('o', "output", HelpText = "Specifies name of the output file. Is file already exists, it is overwritten.", Required = true)]
        public string OutputFile
        {
            get;
            set;
        }


        /// <summary>
        /// Represents the parser state.
        /// </summary>
        [ParserState]
        public IParserState LastParserState { get; set; }


        /// <summary>
        /// Generates help message.
        /// </summary>
        /// <returns></returns>
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
