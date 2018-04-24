using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace zipper
{
    public class CompressFileManager : FileManager
    {
        private readonly long _fileSize;

        public CompressFileManager(string fileSource, string fileDestination, long blockProcessingLength, long fileSize, ConcurrentQueue<BytesBlock> inputQueue,
            ConcurrentQueue<BytesBlock> outputQueue)
            : base(fileSource, fileDestination, blockProcessingLength, inputQueue, outputQueue)
        {
            if (fileSize <= 0) throw new ArgumentOutOfRangeException(nameof(fileSize));

            _fileSize = fileSize;
        }

        public override void ReadBytes()
        {
            using (var fsInput = new FileStream(FileSource, FileMode.Open, FileAccess.Read))
            {
                var length = _fileSize;
                var blockLength = BlockProcessingLength;
               
                while (blockLength > 0 && !ForceStopped)
                {
                    var bytesArray = new byte[blockLength];
                    var readCount = fsInput.Read(bytesArray, 0, bytesArray.Length);

                    InputQueue.Push(new BytesBlock(ReadedBlockCount, bytesArray));

                    ++ReadedBlockCount;
                    length -= readCount;

                    blockLength = length < blockLength ? length : blockLength;
                }

                if (ForceStopped) return;
                InputQueue.Complete();
            }
        }

        public override void WriteBytes()
        {
            var writedBlockCount = 0;
            while (!OutputQueue.Completed() && !ForceStopped)
            {
                var bytesBlock = OutputQueue.Pop();
                if (bytesBlock == null) continue;
                
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(FsOutput, bytesBlock);

                ++writedBlockCount;
                if (InputQueue.Completed() && ReadedBlockCount == writedBlockCount)
                    OutputQueue.Complete();
            }
        }
    }
}
