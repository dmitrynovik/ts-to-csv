namespace Ts2CsvConverter.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args.Length > 0 ? args[0] : @".\";
            var converter = new TypescriptToCsvConverter();
            converter.TranslateDirectory(path);
        }
    }
}
