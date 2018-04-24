using System;
using System.IO;
using System.Linq;

namespace zipper
{
    public class ConsoleManager
    {
        public Enums.CompressionMode CompressionMode;
        public string InputFileName;
        public string OutputFileName;

        private const string InfoMessage = "Enter through a space button Compress/Decompress command, input file name with extension and output file name with extension.";

        public ConsoleManager()
        {
            GetData();
        }

        public ConsoleManager(string[] args)
        {
            GetDataFromArray(args);
        }

        private void GetDataFromArray(string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            try
            {
                CompressionMode = args[0].ToEnum(Enums.CompressionMode.None);
                InputFileName = args[1];
                OutputFileName = args[2];
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(InfoMessage);
            }
        }

        private void GetDataFromConsole()
        {
            GetDataFromArray(Console.ReadLine().Split(' ').ToArray());
        }

        private void GetData()
        {
            Console.WriteLine(InfoMessage);
            GetDataFromConsole();

            CheckData();
        }

        private void CheckData()
        {
            while (true)
            {
                if (!CheckCompressionMode())
                    continue;

                if(!CheckInputFileName())
                    continue;

                if(!CheckOutputFileName())
                    continue;

                break;
            }
        }

        private bool CheckCompressionMode()
        {
            if (CompressionMode != Enums.CompressionMode.None)
                return true;

            Console.WriteLine("Enter compression mode in first position.");
            GetData();
            return false;
        }

        private bool CheckInputFileName()
        {
            if (InputFileName.Length == 0)
            {
                Console.WriteLine("Enter input file name in second position.");
                GetData();
                return false;
            }

            if (!File.Exists(InputFileName))
            {
                Console.WriteLine("There is no input file in zipper directory.");
                GetData();
                return false;
            }

            var inputFileInfo = new FileInfo(InputFileName);

            if (string.IsNullOrEmpty(inputFileInfo.Extension))
            {
                Console.WriteLine("Input file name must be with extension.");
                GetData();
                return false;
            }

            if (inputFileInfo.Extension == ".gz" && CompressionMode == Enums.CompressionMode.Compress)
            {
                Console.WriteLine("You can't compress already compressed file.");
                GetData();
                return false;
            }

            if (inputFileInfo.Extension != ".gz" && CompressionMode == Enums.CompressionMode.Decompress)
            {
                Console.WriteLine("You can't decompress uncompressed file.");
                GetData();
                return false;
            }

            return true;
        }

        private bool CheckOutputFileName()
        {
            if (OutputFileName.Length == 0)
            {
                Console.WriteLine("Enter output file name in third position.");
                GetData();
                return false;
            }

            var outputFileInfo = new FileInfo(OutputFileName);

            if (InputFileName == OutputFileName || outputFileInfo.Exists)
            {
                Console.WriteLine("Output file must not be created.");
                GetData();
                return false;
            }

            if (string.IsNullOrEmpty(outputFileInfo.Extension))
            {
                Console.WriteLine("Output file name must be with extension.");
                GetData();
                return false;
            }

            if (outputFileInfo.Extension != ".gz" && CompressionMode == Enums.CompressionMode.Compress)
            {
                Console.WriteLine("Output file extension must not be \".gz\".");
                GetData();
                return false;
            }

            if (outputFileInfo.Extension == ".gz" && CompressionMode == Enums.CompressionMode.Decompress)
            {
                Console.WriteLine("Output file extension must be \".gz\".");
                GetData();
                return false;
            }

            return true;
        }
    }
}