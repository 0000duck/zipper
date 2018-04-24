using System;
using System.IO;
using System.IO.Compression;

namespace zipper
{
    public class DecompressZipManager : ZipManager
    {
        private const CompressionMode CompressionMode = System.IO.Compression.CompressionMode.Decompress;
        
        public DecompressZipManager(string inputFileName, string outputFileName)
            : base(CompressionMode, inputFileName, outputFileName)
        {
            
        }

        public override void ProcessData()
        {
            while (!ReadedQueue.Completed() && !ForceStopped)
            {
                var bytesBlock = ReadedQueue.Pop();
                if (bytesBlock == null) continue;
                
                using (var memoryStream = new MemoryStream(bytesBlock.BytesArray))
                {
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode))
                    {
                        var tempBytesArray = new byte[BlockProcessingLength];
                        var readedBytesCount = gZipStream.Read(tempBytesArray, 0, tempBytesArray.Length);

                        var bytesArray = new byte[readedBytesCount];
                        Buffer.BlockCopy(tempBytesArray, 0, bytesArray, 0, readedBytesCount);

                        OutputQueue.Push(new BytesBlock(bytesBlock.OrderNum, bytesArray));
                    }
                }
            }
        }
    }
}