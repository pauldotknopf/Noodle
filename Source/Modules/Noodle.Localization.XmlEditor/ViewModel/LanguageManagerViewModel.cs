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

namespace Noodle.Localization.XmlEditor.ViewModel
{
    /// <summary>
    /// The view model for the language manager
    /// </summary>
    public class LanguageManagerViewModel : ViewModelBase
    {
        #region Fields

        private readonly ObservableCollection<Pair<Language, ObservableCollection<LocaleStringResourceModel>>> _languages = new ObservableCollection<Pair<Language, ObservableCollection<LocaleStringResourceModel>>>();
        private readonly ObservableCollection<string> _possibleValues = new ObservableCollection<string>();

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageManagerViewModel"/> class.
        /// </summary>
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
                        new LocaleStringResourceModel("test 1", "value 1"),
                        new LocaleStringResourceModel("test 2", "value 2"),
                        new LocaleStringResourceModel(new LocaleStringResource{ResourceName = "test 3", ResourceValue="value 3"})
                    })
                });
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The languages currently being edited
        /// </summary>
        public ObservableCollection<Pair<Language, ObservableCollection<LocaleStringResourceModel>>> Languages
        {
            get { return _languages; }
        }

        /// <summary>
        /// A list of all the possibe values.
        /// This is used to spot missing values in other languages
        /// </summary>
        public ObservableCollection<string> PossibleValues
        {
            get { return _possibleValues; }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Import an xml file
        /// </summary>
        public RelayCommand ImportCommand { get; protected set; }

        /// <summary>
        /// Export an xml file
        /// </summary>
        public RelayCommand ExportCommand { get; protected set; }

        /// <summary>
        /// Exit the application
        /// </summary>
        public RelayCommand ExitCommand { get; protected set; }

        #endregion

        #region Business Logic

        /// <summary>
        /// Export an xml file
        /// </summary>
        private void Export()
        {
            if (_languages == null || _languages.Count == 0)
            {
                MessageBox.Show("You must first import an xml file to modify.", "Error");
                return;
            }

            string fileName = string.Empty;
            string backupFile = string.Empty;

            try
            {
                var sfd = new SaveFileDialog();
                if (sfd.ShowDialog().Value)
                {
                    fileName = sfd.FileName;
                    if (File.Exists(sfd.FileName))
                    {
                        backupFile = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + "-backup" + Path.GetExtension(fileName));
                        File.Move(fileName, backupFile);
                    }
                    using (var writer = File.Open(sfd.FileName, FileMode.OpenOrCreate, FileAccess.Write))
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
                if (!string.IsNullOrEmpty(backupFile) && File.Exists(backupFile) && !string.IsNullOrEmpty(fileName))
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    File.Move(backupFile, fileName);
                }
            }
        }

        /// <summary>
        /// Import an xml file
        /// </summary>
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

        /// <summary>
        /// Exit the application
        /// </summary>
        private void Exit()
        {
            Application.Current.Shutdown();
        }

        #endregion
    }
}
