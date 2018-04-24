using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.IO.Compression;

namespace zipper
{
    public abstract class ZipManager
    {
        protected static long BlockProcessingLength;
        protected static ConcurrentQueue<BytesBlock> ReadedQueue = new ConcurrentQueue<BytesBlock>();
        protected static ConcurrentQueue<BytesBlock> OutputQueue = new ConcurrentQueue<BytesBlock>();
        protected bool ForceStopped;

        private readonly FileManager _fileManager;
        private readonly long _blocksCount;


        protected ZipManager(CompressionMode compressionMode, string inputFileName, string outputFileName)
        {
            if (!Enum.IsDefined(typeof(CompressionMode), compressionMode))
                throw new InvalidEnumArgumentException(nameof(compressionMode), (int) compressionMode,
                    typeof(CompressionMode));
            if (string.IsNullOrEmpty(inputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(inputFileName));
            if (string.IsNullOrEmpty(outputFileName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(outputFileName));
            if (string.IsNullOrWhiteSpace(inputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputFileName));
            if (string.IsNullOrWhiteSpace(outputFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFileName));

            BlockProcessingLength = Convert.ToInt64(ConfigurationManager.AppSettings["blockProcessingLength"]);

            var inputFileInfo = new FileInfo(inputFileName);

            _blocksCount = (inputFileInfo.Length + BlockProcessingLength - 1) / BlockProcessingLength;

            _fileManager = ManagerFactry.CreateFileManager(compressionMode, inputFileName, outputFileName, BlockProcessingLength, inputFileInfo.Length, 
                ReadedQueue, OutputQueue);

            if (_fileManager == null)
                throw new InvalidOperationException("Compression mode is invalid");
        }

        public void Run()
        {
            var processorCount = Environment.ProcessorCount - 2;
            var zipperThreadCount = Convert.ToInt32(_blocksCount) > processorCount
                ? processorCount
                : Convert.ToInt32(_blocksCount);

            var threadWorker = new ThreadWorker();

            threadWorker.CreateProcessThreads(new List<Action> {ProcessData}, new List<Action> {ForceStop}, zipperThreadCount);
            threadWorker.CreateProcessThreads(new List<Action> {_fileManager.WriteBytes}, new List<Action> { _fileManager.ForceStop, _fileManager.Dispose }, 1);

            threadWorker.StartThreads();

            _fileManager.ReadBytes();

            threadWorker.WaitAll();
            
            _fileManager.Dispose();
        }
        
        public abstract void ProcessData();

        public void ForceStop()
        {
            ForceStopped = true;
        }
    }
}