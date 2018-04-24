using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace zipper
{
    public class DecompressFileManager : FileManager
    {
        public DecompressFileManager(string fileSource, string fileDestination, long blockProcessingLength, ConcurrentQueue<BytesBlock> inputQueue,
            ConcurrentQueue<BytesBlock> outputQueue)
            : base(fileSource, fileDestination, blockProcessingLength, inputQueue, outputQueue)
        {
        }

        public override void ReadBytes()
        {
            using (var fsInput = new FileStream(FileSource, FileMode.Open, FileAccess.Read))
            {
                var binaryFormatter = new BinaryFormatter();

                while (fsInput.Position < fsInput.Length && !ForceStopped)
                {
                    InputQueue.Push((BytesBlock)binaryFormatter.Deserialize(fsInput));
                    ++ReadedBlockCount;
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

                FsOutput.Seek(Convert.ToInt32(BlockProcessingLength) * bytesBlock.OrderNum, SeekOrigin.Begin);
                FsOutput.Write(bytesBlock.BytesArray, 0, bytesBlock.BytesArray.Length);

                ++writedBlockCount;
                if (InputQueue.Completed() && ReadedBlockCount== writedBlockCount)
                    OutputQueue.Complete();
            }
        }
    }
}
