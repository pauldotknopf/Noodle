
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Noodle.Localization.LanguageFileMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var sb = new StringBuilder();

                //long count = 0;

                var languagesListEvo = Helpers.DeserializeLanguagesFile("C:\\MedXChangeLanguages-Evo.xml");
                var languagesListEndo = Helpers.DeserializeLanguagesFile("C:\\MedXChangeLanguages-Endo.xml");

                if (languagesListEvo.Count != languagesListEndo.Count)
                    throw new Exception("Different number of languages!");

                foreach (var languagePairEvo in languagesListEvo)
                {
                    var evoLanguage = languagePairEvo.First;
                    var stringLocaleResourceListEvo = languagePairEvo.Second;

                    var endoLanguage = languagesListEndo.First(x => x.First.Name.Equals(evoLanguage.Name));
                    var stringLocaleResourceListEndo = endoLanguage.Second;

                    foreach (var endoPair in stringLocaleResourceListEndo)
                    {
                        if (!stringLocaleResourceListEvo.Exists(x => x.ResourceName.Equals(endoPair.ResourceName)))
                        {
                            stringLocaleResourceListEvo.Add(endoPair);
                        }
                    }
                }

                using (var writer = File.Open("C:\\MedXChangeLanguages-Merged-Final.xml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (var xmlWriter = XmlWriter.Create(writer))
                    {
                        xmlWriter.WriteStartElement("Languages");
                        foreach (var language in languagesListEvo)
                        {
                            xmlWriter.WriteStartElement("Language");
                            xmlWriter.WriteAttributeString("Name", language.First.Name);
                            xmlWriter.WriteAttributeString("CultureCode", language.First.LanguageCulture);
                            foreach (var resource in language.Second.Where(x =>  !string.IsNullOrEmpty(x.ResourceValue)))
                            {
                                xmlWriter.WriteStartElement("LocaleResource");
                                xmlWriter.WriteAttributeString("Name", resource.ResourceName);
                                xmlWriter.WriteStartElement("Value");
                                xmlWriter.WriteRaw(resource.ResourceValue);
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteEndElement();
                            }
                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
