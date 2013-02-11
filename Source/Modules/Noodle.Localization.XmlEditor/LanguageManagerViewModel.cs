using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Noodle.Localization.Services;

namespace Noodle.Localization.XmlEditor
{
    public class LanguageManagerViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Pair<Language, ObservableCollection<LocaleStringResourceModel>>> _languages = new ObservableCollection<Pair<Language, ObservableCollection<LocaleStringResourceModel>>>();
        private ObservableCollection<string> _possibleValues = new ObservableCollection<string>();

        public LanguageManagerViewModel()
        {
            if (!IsInDesignMode)
            {
                ImportCommand = new RelayCommand(Import);
                ExportCommand = new RelayCommand(Export);
                ExitCommand = new RelayCommand(Exit);
            }
            else
            {
                _languages.Add(new Pair<Language, ObservableCollection<LocaleStringResourceModel>>
                {
                    First = new Language { Name = "English" },
                    Second = new ObservableCollection<LocaleStringResourceModel>(new List<LocaleStringResourceModel>
                    {
                        new LocaleStringResourceModel
                        {
                            ResourceName = "test 1",
                            ResourceValue = "value 1"
                        }, new LocaleStringResourceModel
                        {
                            ResourceName = "test 2",
                            ResourceValue = "value 2"
                        }
                    })
                });
            }
        }

        public ObservableCollection<Pair<Language, ObservableCollection<LocaleStringResourceModel>>> Languages
        {
            get { return _languages; }
        }

        public ObservableCollection<string> PossibleValues
        {
            get { return _possibleValues; }
        }

        public RelayCommand ImportCommand { get; protected set; }

        public RelayCommand ExportCommand { get; protected set; }

        public RelayCommand ExitCommand { get; protected set; }

        private void Export()
        {
            if (_languages == null || _languages.Count == 0)
            {
                MessageBox.Show("You must first import an xml file to modify.", "Error");
                return;
            }

            try
            {
                var sfdXml = new SaveFileDialog();
                var result = sfdXml.ShowDialog();

                if (result.Value)
                {
                    if (File.Exists(sfdXml.FileName))
                        File.Delete(sfdXml.FileName);

                    using (var writer = File.Open(sfdXml.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (var xmlWriter = XmlWriter.Create(writer))
                        {
                            xmlWriter.WriteStartElement("Languages");
                            foreach (var language in _languages)
                            {
                                xmlWriter.WriteStartElement("Language");
                                xmlWriter.WriteAttributeString("Name", language.First.Name);
                                xmlWriter.WriteAttributeString("CultureCode", language.First.LanguageCulture);
                                foreach (var resource in language.Second.Where(x => !x.IsMissing || (x.IsMissing && !string.IsNullOrEmpty(x.ResourceValue))))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Import()
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.FileOk += (sender, args) => args.Cancel = string.IsNullOrEmpty(ofd.FileName) || !Path.GetExtension(ofd.FileName.ToLower()).Equals(".xml");
                var result = ofd.ShowDialog();
                if (result.Value)
                {
                    var deserializedLanguages = new LanguageInstaller(null, null, null).DeserializeLanguagesFile(ofd.FileName);

                    _languages.Clear();
                    _possibleValues.Clear();
                    foreach (var possibleValue in deserializedLanguages.SelectMany(x => x.Second).Select(x => x.ResourceName).Distinct().ToList())
                    {
                        _possibleValues.Add(possibleValue);
                    }

                    foreach (var pair in deserializedLanguages)
                    {
                        var language = new Pair<Language, ObservableCollection<LocaleStringResourceModel>>();
                        language.First = pair.First;
                        language.Second = new ObservableCollection<LocaleStringResourceModel>();
                        foreach (var possibleValue in _possibleValues)
                        {
                            var resourceModel = new LocaleStringResourceModel(pair.Second.FirstOrDefault(x => x.ResourceName == possibleValue));
                            if (resourceModel.IsMissing)
                                resourceModel.ResourceName = possibleValue;
                            language.Second.Add(resourceModel);
                        }
                        _languages.Add(language);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
