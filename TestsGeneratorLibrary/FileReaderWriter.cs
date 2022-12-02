namespace TestsGeneratorLibrary
{
    public class FileReaderWriter
    {
        public async Task<string> Read(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new ArgumentException("File doesn't exists.");
            }

            String result = "";
            using (var reader = File.OpenText(filename))
            {
                result = await reader.ReadToEndAsync();                
            }
                
            return result;
        }

        public async Task Write(string filePath, string fileContent)
        {
            if (!Directory.Exists(filePath))
            {
                throw new ArgumentException("Directory doesn't exists.");
            }

            await using var writer = new StreamWriter(filePath);
            await writer.WriteAsync(fileContent);
        }

    }
}
