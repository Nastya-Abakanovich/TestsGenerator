using TestsGeneratorLibrary;

namespace LibraryTests
{
    public class FileReaderWriterTests
    {
        public async Task NonExistentFileToRead()
        {
            FileReaderWriter fileReaderWriter = new FileReaderWriter();
            await fileReaderWriter.Read("..\\..\\..\\..\\Example\\TestClasses\\1.cs");
        }

        [Test]
        public void Read_NonExistentFileToRead_ReturnArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await NonExistentFileToRead());
        }

        [Test]
        public async Task Read_ExistentFileToRead_ReturnTextFromFile()
        {
            FileReaderWriter fileReaderWriter = new FileReaderWriter();
            string result = await fileReaderWriter.Read("..\\..\\..\\Input.txt");

            Assert.AreEqual("Input text.", result, "The text from the file was read incorrectly.");
        }

        public async Task NonExistentDirectoryToWrite()
        {
            FileReaderWriter fileReaderWriter = new FileReaderWriter();
            await fileReaderWriter.Write("..\\..\\..\\..\\Example\\Tests\\filename.cs", "");
        }

        [Test]
        public void Write_NonExistentDirectoryToWrite_ReturnArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await NonExistentDirectoryToWrite());
        }

        [Test]
        public async Task Write_ExistentDirectoryToWrite_WriteTextToFile()
        {
            FileReaderWriter fileReaderWriter = new FileReaderWriter();
            await fileReaderWriter.Write("..\\..\\..\\Output.txt", "Output text.");
            string result = await fileReaderWriter.Read("..\\..\\..\\Output.txt");

            Assert.AreEqual("Output text.", result, "The text in the file is not written correctly.");
        }

    }
}