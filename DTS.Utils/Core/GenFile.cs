namespace DTS.Utils.Core
{
    class GenFile
    {
        public GenFile(string filePath, string text)
        {
            FilePath = filePath;
            Text = text;
        }

        public string FilePath { get; set; }
        public string Text { get; set; }
    }
}