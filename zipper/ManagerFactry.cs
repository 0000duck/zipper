using System;
using System.ComponentModel;
using System.IO.Compression;

namespace zipper
{
    public class ManagerFactry
    {
        public static ZipManager CreateZipManager(Enums.CompressionMode compressionMode, string inputFileName, string outputFileName)
        {
            if (!Enum.IsDefined(typeof(Enums.CompressionMode), compressionMode))
                throw new InvalidEnumArgumentException(nameof(compressionMode), (int) compressionMode,
                    typeof(Enums.CompressionMode));
            if (string.IsNullOrEmpty(inputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(inputFileName));
            if (string.IsNullOrEmpty(outputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(outputFileName));
            if (string.IsNullOrWhiteSpace(inputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputFileName));
            if (string.IsNullOrWhiteSpace(outputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFileName));

            switch (compressionMode)
            {
                case Enums.CompressionMode.Compress:
                    return CreateCompressZipManager(inputFileName, outputFileName);

                case Enums.CompressionMode.Decompress:
                    return CreateDecompressZipManager(inputFileName, outputFileName);

                default: return null;
            }
        }

        public static FileManager CreateFileManager(CompressionMode compressionMode, string inputFileName, string outputFileName, long blockProcessingLength, 
            long fileSize, ConcurrentQueue<BytesBlock> readedQueue, ConcurrentQueue<BytesBlock> outputQueue)
        {
            if (!Enum.IsDefined(typeof(CompressionMode), compressionMode))
                throw new InvalidEnumArgumentException(nameof(compressionMode), (int)compressionMode,
                    typeof(CompressionMode));
            if (string.IsNullOrEmpty(inputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(inputFileName));
            if (string.IsNullOrEmpty(outputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(outputFileName));
            if (string.IsNullOrWhiteSpace(inputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputFileName));
            if (string.IsNullOrWhiteSpace(outputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFileName));
            if (blockProcessingLength <= 0) throw new ArgumentOutOfRangeException(nameof(blockProcessingLength));
            if (fileSize <= 0) throw new ArgumentOutOfRangeException(nameof(fileSize));
            if (readedQueue == null) throw new ArgumentNullException(nameof(readedQueue));
            if (outputQueue == null) throw new ArgumentNullException(nameof(outputQueue));

            switch (compressionMode)
            {
                case CompressionMode.Compress:
                    return CreateCompressFileManager(inputFileName, outputFileName, blockProcessingLength, fileSize, readedQueue, outputQueue);

                case CompressionMode.Decompress:
                    return CreateDecompressFileManager(inputFileName, outputFileName, blockProcessingLength, readedQueue, outputQueue);

                default: return null;
            }
        }

        private static CompressZipManager CreateCompressZipManager(string inputFileName, string outputFileName)
        {
            if(string.IsNullOrEmpty(inputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(inputFileName));
            if (string.IsNullOrEmpty(outputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(outputFileName));
            if (string.IsNullOrWhiteSpace(inputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputFileName));
            if (string.IsNullOrWhiteSpace(outputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFileName));

            return new CompressZipManager(inputFileName, outputFileName);
        }

        private static DecompressZipManager CreateDecompressZipManager(string inputFileName, string outputFileName)
        {
            if (string.IsNullOrEmpty(inputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(inputFileName));
            if (string.IsNullOrEmpty(outputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(outputFileName));
            if (string.IsNullOrWhiteSpace(inputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputFileName));
            if (string.IsNullOrWhiteSpace(outputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFileName));

            return new DecompressZipManager(inputFileName, outputFileName);
        }

        private static CompressFileManager CreateCompressFileManager(string inputFileName, string outputFileName, long blockProcessingLength, long fileSize,
            ConcurrentQueue<BytesBlock> readedQueue, ConcurrentQueue<BytesBlock> outputQueue)
        {
            if (string.IsNullOrEmpty(inputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(inputFileName));
            if (string.IsNullOrEmpty(outputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(outputFileName));
            if (string.IsNullOrWhiteSpace(inputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputFileName));
            if (string.IsNullOrWhiteSpace(outputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFileName));
            if (blockProcessingLength <= 0) throw new ArgumentOutOfRangeException(nameof(blockProcessingLength));
            if (fileSize <= 0) throw new ArgumentOutOfRangeException(nameof(fileSize));
            if (readedQueue == null) throw new ArgumentNullException(nameof(readedQueue));

            return new CompressFileManager(inputFileName, outputFileName, blockProcessingLength, fileSize, readedQueue,
                outputQueue);
        }

        private static DecompressFileManager CreateDecompressFileManager(string inputFileName, string outputFileName, long blockProcessingLength, 
            ConcurrentQueue<BytesBlock> readedQueue, ConcurrentQueue<BytesBlock> outputQueue)
        {
            if (string.IsNullOrEmpty(inputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(inputFileName));
            if (string.IsNullOrEmpty(outputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(outputFileName));
            if (string.IsNullOrWhiteSpace(inputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputFileName));
            if (string.IsNullOrWhiteSpace(outputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFileName));
            if (blockProcessingLength <= 0) throw new ArgumentOutOfRangeException(nameof(blockProcessingLength));
            if (readedQueue == null) throw new ArgumentNullException(nameof(readedQueue));

            return new DecompressFileManager(inputFileName, outputFileName, blockProcessingLength, readedQueue, outputQueue);
        }
    }
}