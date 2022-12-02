namespace TestsGeneratorLibrary
{
    public class OutputFileInfo
    {
        public string Filename { get; set; }
        public string FileContent { get; set; }

        public OutputFileInfo()
        {
            Filename = "";
            FileContent = "";
        }

        public OutputFileInfo(string filename, string fileContent)
        {
            Filename = filename;
            FileContent = fileContent;
        }
    }
}
