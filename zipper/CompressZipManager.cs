using System.IO;
using System.IO.Compression;

namespace zipper
{
    public class CompressZipManager : ZipManager
    {
        private const CompressionMode CompressionMode = System.IO.Compression.CompressionMode.Compress;

        public CompressZipManager(string inputFileName, string outputFileName)
            : base(CompressionMode, inputFileName, outputFileName)
        {

        }

        public override void ProcessData()
        {
            while (!ReadedQueue.Completed() && !ForceStopped)
            {
                var bytesBlock = ReadedQueue.Pop();
                if (bytesBlock == null) continue;

                using (var memoryStream = new MemoryStream())
                {
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode))
                    using (var binaryWriter = new BinaryWriter(gZipStream))
                    {
                        binaryWriter.Write(bytesBlock.BytesArray, 0, bytesBlock.BytesArray.Length);
                    }

                    OutputQueue.Push(new BytesBlock(bytesBlock.OrderNum, memoryStream.ToArray()));
                }
            }
        }
    }
}