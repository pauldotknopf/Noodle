using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Noodle.Localization.Services;
using PowerArgs;

namespace Noodle.Localization.Audio
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var arguements = Args.Parse<Arguements>(args);

                var resources = new LanguageFileParser().DeserializeLanguagesFile(arguements.InputFile);

                foreach (var language in resources)
                {
                    var languageDirectory = new DirectoryInfo(Path.Combine(arguements.OutputDirectory, language.First.LanguageCulture));

                    if(!languageDirectory.Exists)
                        languageDirectory.Create();

                    if (arguements.CleanOutputDirectory)
                    {
                        foreach (var file in languageDirectory.GetFiles())
                            file.Delete();
                        foreach (var dir in languageDirectory.GetDirectories())
                            dir.Delete(true);
                    }

                    foreach (var resource in language.Second)
                    {
                        var destination = Path.Combine(languageDirectory.FullName, resource.ResourceName + ".mp3");
                        if(File.Exists(destination))
                            File.Delete(destination);
                        DownloadGoogleVoice(language.First.LanguageCulture, resource.ResourceValue, destination);
                    }
                }

                Console.WriteLine("The audio has been generate for {0} to the directory {1}", arguements.InputFile, arguements.OutputDirectory);
            }
            catch (ArgException ex)
            {
                Console.WriteLine(ArgUsage.GetUsage<Arguements>());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private static void DownloadGoogleVoice(string culture, string value, string destination)
        {
            using (var client = new WebClient())
                client.DownloadFile(string.Format("http://translate.google.com/translate_tts?ie=UTF-8&q={0}&tl={1}&total=1&idx=0&textlen={2}", value, culture, value.Length), destination);
        }
    }
}
