using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper.Configuration;

namespace Ts2CsvConverter
{
    public class TypescriptToCsvConverter
    {
        private readonly Encoding _encoding;

        public class ConverterException : Exception
        {
            public ConverterException(string message) : base(message) {  }
        }

        public TypescriptToCsvConverter(Encoding encoding = null)
        {
            _encoding = encoding ?? Encoding.UTF8;
        }

        private const string Expr = @"(?<='|\"")[\w\s\-\'\""\>\<\/,\.\{\}]*(?=':|\"":|'$|\""$)";

        public (string, string) GetTranslation(string line)
        {
            var delimiter = Regex.Match(line, @"'\s*:|""\s*:");
            if (delimiter.Success)
            {
                int pos = line.IndexOf(delimiter.Value, StringComparison.InvariantCultureIgnoreCase);
                if (pos > 0)
                {
                    var key = line.Substring(0, pos).Trim(' ', '\'', '\"', ':');
                    var value = line.Substring(pos + delimiter.Value.Length, line.Length - pos - delimiter.Value.Length).Trim(' ', '\'', '\"', ':');
                    return (key, value);
                }
            }
            return (null, null);
        }

        public (string, string) GetRegexTranslation(string line)
        {
            var matches = Regex.Matches(line, Expr);
            switch (matches.Count)
            {
                case 0:
                case 1:
                    return (null, null);
                case 2:
                    return (matches[0].Value, matches[1].Value);
                default:
                    throw new ConverterException($"Unexpected number of matches: {matches.Count}");
            }
        }

        public IDictionary<string, string> GetTranslations(Stream stream)
        {
            using (var reader = new StreamReader(stream, _encoding))
            {
                var result = new Dictionary<string, string>();
                string line;
                while ((line = reader.ReadLine()?.Trim(' ', ',')) != null)
                {
                    if (line.StartsWith("//"))
                        continue;

                    var translation = GetTranslation(line);
                    if (translation.Item1 != null)
                        result[translation.Item1] = translation.Item2;
                }
                return result;
            }
        }

        public IDictionary<string, string> GetTranslationsFromFile(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return GetTranslations(stream);
            }
        }

        public IDictionary<string, IDictionary<string, string>> GetTranslationsFromDirectory(string path) => 
            Directory.GetFiles(path, "*.ts").ToDictionary(Path.GetFileNameWithoutExtension, GetTranslationsFromFile);

        public void TranslateDirectory(string path)
        {
            foreach (var kvp in GetTranslationsFromDirectory(path))
            {
                var fileName = kvp.Key;
                var translations = kvp.Value;
                var output = Path.Combine(path, $"{fileName}.csv");

                using (var file = File.OpenWrite(output))
                {
                    using (var writer = new StreamWriter(file, _encoding))
                    {
                        using (var csvWriter = new CsvHelper.CsvWriter(writer, new Configuration { HasHeaderRecord = false, QuoteAllFields = true }))
                        {
                            foreach (var translation in translations)
                            {
                                csvWriter.WriteRecord(translation);
                                csvWriter.NextRecord();
                            }
                        }
                    }
                }
            }
        }
    }
}
