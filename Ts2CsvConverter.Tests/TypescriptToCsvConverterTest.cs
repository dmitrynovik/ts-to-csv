using Xunit;

namespace Ts2CsvConverter
{
    public class TypescriptToCsvConverterTest
    {
        [Fact]
        public void UnicodeWordWithApostrophy()
        {
            const string text = @"'AddingSpecialBags-Item-CRST': 'Siège's'";
            var tt = new TypescriptToCsvConverter().GetTranslation(text);
            Assert.Equal("AddingSpecialBags-Item-CRST", tt.Item1);
            Assert.Equal("Siège's", tt.Item2);
        }

        [Fact]
        public void UnicodeWord()
        {
            const string text = @"'AddingSpecialBags-Item-CRST': 'Siège pour bébé ou siège de voiture'";
            var tt = new TypescriptToCsvConverter().GetTranslation(text);
            Assert.Equal("AddingSpecialBags-Item-CRST", tt.Item1);
            Assert.Equal("Siège pour bébé ou siège de voiture", tt.Item2);
        }

        [Fact]
        public void UnicodeWordWithFormatting()
        {
            const string text = @"'AddingSpecialBags-Item-CRST': 'Siège pour bébé ou siège de voiture {PAX}'";
            var tt = new TypescriptToCsvConverter().GetTranslation(text);
            Assert.Equal("AddingSpecialBags-Item-CRST", tt.Item1);
            Assert.Equal("Siège pour bébé ou siège de voiture {PAX}", tt.Item2);
        }

        [Fact]
        public void UnicodeWordWithSpecialSymbols()
        {
            const string text = @"'AddingSpecialBags-Item-CRST': 'Siège pour bébé ou /siège-de-voiture.../'";
            var tt = new TypescriptToCsvConverter().GetTranslation(text);
            Assert.Equal("AddingSpecialBags-Item-CRST", tt.Item1);
            Assert.Equal("Siège pour bébé ou /siège-de-voiture.../", tt.Item2);
        }

        [Fact]
        public void UnicodeWordWithHtml()
        {
            const string text = @"'AddingSpecialBags-Item-CRST': 'Siège pour bébé ou siège de voiture<br/>'";
            var tt = new TypescriptToCsvConverter().GetTranslation(text);
            Assert.Equal("AddingSpecialBags-Item-CRST", tt.Item1);
            Assert.Equal("Siège pour bébé ou siège de voiture<br/>", tt.Item2);
        }

        [Fact]
        public void EmptyTranslation()
        {
            const string text = @"'Empty-Empty': ''";
            var tt = new TypescriptToCsvConverter().GetTranslation(text);
            Assert.Equal("Empty-Empty", tt.Item1);
            Assert.Equal("", tt.Item2);
        }
    }
}
