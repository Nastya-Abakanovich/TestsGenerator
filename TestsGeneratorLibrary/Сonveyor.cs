using System.Threading.Tasks.Dataflow;

namespace TestsGeneratorLibrary
{
    public class Сonveyor
    {
        private readonly TestsGenerator _testsGenerator = new TestsGenerator();
        private readonly FileReaderWriter _fileReaderWriter = new FileReaderWriter();

        private string _outputPath;
        
        private TransformBlock<string, string> _fileReaderBlock;
        private TransformManyBlock<string, OutputFileInfo> _fileGeneratorBlock;
        private ActionBlock<OutputFileInfo> _fileWriterBlock;      

        public Сonveyor(string outputPath, 
            int maxDegreeOfParallelismReader, int maxDegreeOfParallelismGenerator, int maxDegreeOfParallelismWritre)
        {
            _outputPath = outputPath;

            _fileReaderBlock = new TransformBlock<string, string>(
               async filename => await _fileReaderWriter.Read(filename),
               new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelismReader });

            _fileGeneratorBlock = new TransformManyBlock<string, OutputFileInfo>(
               sourceCode => _testsGenerator.CreateTests(sourceCode),
               new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelismGenerator });

            _fileWriterBlock = new ActionBlock<OutputFileInfo>(
               async fileInfo => await _fileReaderWriter.Write(_outputPath + "\\" + fileInfo.Filename + ".cs", fileInfo.FileContent),
               new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelismWritre });

            var options = new DataflowLinkOptions() { PropagateCompletion = true };

            _fileReaderBlock.LinkTo(_fileGeneratorBlock, options);
            _fileGeneratorBlock.LinkTo(_fileWriterBlock, options);
        }

        public async Task Generate(List<string> filenames)
        {
            foreach (var filename in filenames)
            {
                _fileReaderBlock.Post(filename);
            }

            _fileReaderBlock.Complete();
            await _fileWriterBlock.Completion;
        }
    }
}
