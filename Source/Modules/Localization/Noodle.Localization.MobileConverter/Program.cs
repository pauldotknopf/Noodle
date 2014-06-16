
using System.Linq;
using PowerArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Noodle.Localization.MobileConverter
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parsed = Args.Parse<MobileConverterArgs>(args);
                Console.WriteLine("You entered the operating system of '{0}' with the path to the languages xml to be converted at '{1}'. The output path you entered was '{2}'", parsed.OperatingSystemName, parsed.InputPath, parsed.OutputDirectory);
                parsed.OperatingSystemName = parsed.OperatingSystemName.ToLower().Trim();
                if (parsed.OperatingSystemName.Contains("ios"))
                {
                    ConvertToiOS(parsed.InputPath, parsed.OutputDirectory);
                }
                else if (parsed.OperatingSystemName.Contains("android"))
                {
                    ConvertToAndroid(parsed.InputPath, parsed.OutputDirectory);
                }
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            catch (ArgException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ArgUsage.GetUsage<MobileConverterArgs>());
            }
        }

        // ReSharper disable once InconsistentNaming
        private static void ConvertToiOS(string inputPath, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            var languagesList = Helpers.DeserializeLanguagesFile(inputPath);
            var doneCultureCodes = new List<string>(languagesList.Count);
            foreach (var languagePair in languagesList)
            {
                var language = languagePair.First;
                var stringLocaleResourceList = languagePair.Second;
                Console.WriteLine("Doing " + language.Name);
                var cultureCode = language.LanguageCulture.Substring(0, 2);
                if (doneCultureCodes.Contains(cultureCode))
                {
                    Console.WriteLine("Skipping " + cultureCode + " due to already doing that language. Specific regions are not supported by this tool.");
                    continue;
                }
                doneCultureCodes.Add(cultureCode);

                var builder = new StringBuilder();
                foreach (var resource in stringLocaleResourceList)
                {
                    resource.ResourceValue = resource.ResourceValue.Replace("{", "\\{")
                        .Replace("}", "\\}")
                        .Replace("\"", "\\\"");
                    builder.AppendLine("\"" + resource.ResourceName + "\"" + " = " + "\"" + resource.ResourceValue + "\";");
                }

                //Do mappings
                var mappingsList = Helpers.DeserializeLanguageFileMappings(inputPath);
                foreach (var mapping in mappingsList)
                {
                    var mapping1 = mapping;
                    foreach (var resource in stringLocaleResourceList.Where(resource => resource.ResourceName == mapping1.MappingTo))
                    {
                        resource.ResourceValue = resource.ResourceValue.Replace("{", "\\{")
                       .Replace("}", "\\}")
                       .Replace("\"", "\\\"");
                        builder.AppendLine("\"" + mapping.MappingFrom + "\"" + " = " + "\"" + resource.ResourceValue + "\";");
                        // Only want one mapping...break incase there are doubles. Don't want to have doubles in output.
                        break;
                    }
                }

                Directory.CreateDirectory(outputDirectory + "/" + cultureCode + ".lproj");
                File.WriteAllText(outputDirectory + "/" + cultureCode + ".lproj/" + "Localizable.strings", builder.ToString());
            }
        }

        private static void ConvertToAndroid(string inputPath, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            var languagesList = Helpers.DeserializeLanguagesFile(inputPath);
            var doneCultureCodes = new List<string>(languagesList.Count);
            foreach (var languagePair in languagesList)
            {
                var language = languagePair.First;
                var stringLocaleResourceList = languagePair.Second;
                Console.WriteLine("Doing " + language.Name);
                var cultureCode = language.LanguageCulture.Substring(0, 2);
                if (doneCultureCodes.Contains(cultureCode))
                {
                    Console.WriteLine("Skipping " + cultureCode + " due to already doing that language. Specific regions are not supported by this tool.");
                    continue;
                }
                doneCultureCodes.Add(cultureCode);
                var builder = new StringBuilder();
                builder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                builder.AppendLine("<resources>");

                foreach (var resource in stringLocaleResourceList)
                {
                    resource.ResourceValue = resource.ResourceValue.Replace("{", "\\{")
                        .Replace("}", "\\}")
                        .Replace("\"", "\\\"").Replace("\'", "\\'");
                    builder.AppendLine("<string name=\"" + resource.ResourceName + "\">" + resource.ResourceValue + "</string>");
                }

                //Do mappings
                var mappingsList = Helpers.DeserializeLanguageFileMappings(inputPath);
                foreach (var mapping in mappingsList)
                {
                    var mapping1 = mapping;
                    foreach (var resource in stringLocaleResourceList.Where(resource => resource.ResourceName == mapping1.MappingTo))
                    {
                        resource.ResourceValue = resource.ResourceValue.Replace("{", "\\{")
                       .Replace("}", "\\}")
                       .Replace("\"", "\\\"").Replace("\'", "\\'");
                        builder.AppendLine("<string name=\"" + mapping.MappingFrom + "\">" + resource.ResourceValue + "</string>");
                        // Only want one mapping...break incase there are doubles. Don't want to have doubles in output.
                        break;
                    }
                }

                builder.AppendLine("</resources>");

                Directory.CreateDirectory(outputDirectory + "/values-" + cultureCode);
                File.WriteAllText(outputDirectory + "/values-" + cultureCode + "/Strings.xml", builder.ToString());
            }
        }

    }

    // A class that describes the command line arguments for this program
    public class MobileConverterArgs
    {
        [ArgRequired(PromptIfMissing = true)]
        public string InputPath { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string OutputDirectory { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string OperatingSystemName { get; set; }
    }
}
