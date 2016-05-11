namespace DTS.Utils.Core
{
    public class GenFile
    {
        public GenFile(string relativeFilePath, string text)
        {
            RelativeFilePath = relativeFilePath;
            Text = text;
        }

        public string RelativeFilePath { get; set; }
        public string Text { get; set; }
    }
}