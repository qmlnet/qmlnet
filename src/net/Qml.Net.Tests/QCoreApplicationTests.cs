using System;
using System.IO;
using Xunit;
using FluentAssertions;

namespace Qml.Net.Tests
{
    public class QCoreApplicationTests
    {
#if NETCOREAPP3_1
        [Fact]
        public void CanInstallTranslator()
        {
            string Translator(ReadOnlySpan<char> context, ReadOnlySpan<char> sourceText, ReadOnlySpan<char> disambiguation, int n)
            {
                return $"translated:{new string(context)}:{new string(sourceText)}:{new string(disambiguation)}:{n}";
            }

            using (var app = new QCoreApplication())
            {
                app.InstallTranslator(Translator);

                var result = app.Translate("ctx", "src", "disambig", 2);
                result.Should().Be("translated:ctx:src:disambig:2");
            }
        }

        [Fact]
        public void CanRemoveTranslator()
        {
            string Translator(ReadOnlySpan<char> context, ReadOnlySpan<char> sourceText, ReadOnlySpan<char> disambiguation, int n)
            {
                return "should not be called";
            }

            using (var app = new QCoreApplication())
            {
                Translator translatorDel = Translator;
                app.InstallTranslator(translatorDel);
                app.RemoveTranslator(translatorDel);

                var result = app.Translate("ctx", "src");
                result.Should().Be("src");
            }
        }

        [Fact]
        public void CanLoadTranslationFromMemory()
        {
            var resourcesFolder = Path.Join(Path.GetDirectoryName(typeof(QCoreApplicationTests).Assembly.Location),
                "Resources");
            var qmFile = Path.Join(resourcesFolder, "example.de.qm");
            var qmData = File.ReadAllBytes(qmFile);

            using (var app = new QCoreApplication())
            {
                // With no translations installed, the result should be the source text
                var untranslated = app.Translate("QPushButton", "Hello world!");
                untranslated.Should().Be("Hello world!");

                app.LoadTranslationData(qmData);

                // After installing our translations, it should be the text from example.de.ts
                var translated = app.Translate("QPushButton", "Hello world!");
                translated.Should().Be("Hallo from Deutschland!");
            }
        }
#endif
    }
}
