using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Xml;
using Excel;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using MongoDB.Driver.Linq;
using Noodle.Localization.Services;
using Noodle.Localization.XmlEditor.Bing;
using Telerik.Windows.Controls;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

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
                ImportExcelCommand = new RelayCommand(ImportExcel);
                ExportExcelCommand = new RelayCommand(ExportExcel);
                ExitCommand = new RelayCommand(Exit);
                AddResourceCommand = new RelayCommand(AddResource, () => _languages.Count > 0);
                AddLanguageCommand = new RelayCommand(AddLanguage);
                TranslateMissingValuesCommand = new RelayCommand(TranslateMissingValues);
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

        /// <summary>
        /// Translate missing values automatically
        /// </summary>
        private void TranslateMissingValues()
        {
            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            //Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
            var admAuth = new AdmAuthentication("Noodle", "u04jN8af+vigmdgD5ncB0KRmd3ejMyKRx+T9cFiLEzk=");
            var accessToken = admAuth.GetAccessToken().access_token;


            var source = _languages.First();
            foreach (var language in _languages)
            {
                foreach (var resource in language.Second)
                {
                    if (resource.IsMissing && string.IsNullOrEmpty(resource.ResourceValue))
                    {
                        var sourceValue = source.Second.Single(x => x.ResourceName == resource.ResourceName);
                        resource.ResourceValue = Translate(accessToken, sourceValue.ResourceValue, source.First.LanguageCulture, language.First.LanguageCulture);
                    }
                }
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
        /// Import excel file
        /// </summary>
        public RelayCommand ImportExcelCommand { get; protected set; }

        /// <summary>
        /// Export excel file
        /// </summary>
        public RelayCommand ExportExcelCommand { get; protected set; }

        /// <summary>
        /// Exit the application
        /// </summary>
        public RelayCommand ExitCommand { get; protected set; }

        /// <summary>
        /// Add a resource
        /// </summary>
        public RelayCommand AddResourceCommand { get; protected set; }

        /// <summary>
        /// Add a language
        /// </summary>
        public RelayCommand AddLanguageCommand { get; protected set; }

        /// <summary>
        /// Auto translate missing values
        /// </summary>
        public RelayCommand TranslateMissingValuesCommand { get; protected set; }

        #endregion

        #region Business Logic

        private void ImportExcel()
        {
            try
            {
                var filePath = string.Format(@"C:\Users\Paul\Desktop\translations.xlsx");

                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                using (var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                {
                    excelReader.IsFirstRowAsColumnNames = true;
                    var dataset = excelReader.AsDataSet();

                    if (dataset.Tables.Count != 1)
                        throw new Exception("You must have exactly one sheet.");

                    var table = dataset.Tables[0];

                    var columns = table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

                    if (columns.Count < 2)
                        throw new Exception(
                            "You must have at least two columns. First being the 'Name', and the second being a translated language.");

                    if (columns[0] != "Name")
                        throw new Exception("The first column must be 'Name'.");

                    foreach (var translatedCultureCode in columns.Skip(1))
                    {
                        try
                        {
                            CultureInfo.CreateSpecificCulture(translatedCultureCode);
                        }
                        catch
                        {
                            throw new Exception("The translated culture '" + translatedCultureCode + "' is not valid.");
                        }
                    }

                    foreach (DataRow entry in table.Rows)
                    {
                        var name = entry[0].ToString();
                        for (var x = 1; x < columns.Count; x++)
                        {
                            var languageCulture = columns[x];
                            var value = entry[x].ToString();

                            var languageViewModel = Languages.Single(language => language.First.LanguageCulture == languageCulture);
                            var resourceViewModel = languageViewModel.Second.Single(resource => resource.ResourceName == name);

                            resourceViewModel.ResourceValue = value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExportExcel()
        {
        }

        private string Translate(string accessToken, string text, string from, string to)
        {
            var client = new LanguageServiceClient();
            var httpRequestProperty = new HttpRequestMessageProperty();
            httpRequestProperty.Method = "POST";
            httpRequestProperty.Headers.Add("Authorization", "Bearer " + accessToken);

            using (var scope = new OperationContextScope(client.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                return client.Translate("", text, from, to, "text/plain", "");
            }
        }

        /// <summary>
        /// Add a language
        /// </summary>
        private void AddLanguage()
        {

        }

        /// <summary>
        /// Add a resource
        /// </summary>
        private void AddResource()
        {
            var dialogparameters = new DialogParameters()
            {
                Content = "Resource name:"
            };
            RadWindow.Prompt(dialogparameters, (sender, e) =>
                                                    {
                                                        var result = e.PromptResult;
                                                    });
        }

        /// <summary>
        /// Export an xml file
        /// </summary>
        private void Export()
        {
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
                    var deserializedLanguages = new LanguageFileParser().DeserializeLanguagesFile(ofd.FileName);

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
