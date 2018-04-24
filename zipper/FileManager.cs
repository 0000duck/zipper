using System;
using System.IO;

namespace zipper
{
    public abstract class FileManager : IDisposable
    {
        protected bool ForceStopped;
        protected readonly long BlockProcessingLength;
        protected readonly string FileSource;
        protected readonly ConcurrentQueue<BytesBlock> InputQueue;
        protected readonly ConcurrentQueue<BytesBlock> OutputQueue;
        protected readonly FileStream FsOutput;
        protected int ReadedBlockCount;

        protected FileManager(string fileSource, string fileDestination, long blockProcessingLength, ConcurrentQueue<BytesBlock> inputQueue, ConcurrentQueue<BytesBlock> outputQueue)
        {
            if (string.IsNullOrEmpty(fileSource))
                throw new ArgumentException("Value cannot be null or empty.", nameof(fileSource));
            if (string.IsNullOrEmpty(fileDestination))
                throw new ArgumentException("Value cannot be null or empty.", nameof(fileDestination));
            if (string.IsNullOrWhiteSpace(fileSource))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileSource));
            if (string.IsNullOrWhiteSpace(fileDestination))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileDestination));
            if (blockProcessingLength <= 0) throw new ArgumentOutOfRangeException(nameof(blockProcessingLength));
            if (inputQueue == null) throw new ArgumentNullException(nameof(inputQueue));
            if (outputQueue == null) throw new ArgumentNullException(nameof(outputQueue));

            FileSource = fileSource;

            FsOutput = new FileStream(fileDestination, FileMode.Create, FileAccess.Write);

            BlockProcessingLength = blockProcessingLength;

            InputQueue = inputQueue;
            OutputQueue = outputQueue;
        }

        public abstract void ReadBytes();
        public abstract void WriteBytes();

        public void ForceStop()
        {
            ForceStopped = true;
        }

        public void Dispose()
        {
            FsOutput.Close();
            FsOutput.Dispose();
        }
    }
}