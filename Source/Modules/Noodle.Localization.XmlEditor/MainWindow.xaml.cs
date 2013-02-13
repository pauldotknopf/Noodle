using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Noodle.Localization.XmlEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            LanguagesTab.PreviewMouseDown += LanguagesTabOnPreviewMouseDown;
            Focusable = true;
        }

        private void LanguagesTabOnPreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // this fixes an issue in windows xp 
            // see http://stackoverflow.com/questions/5223998/wpf-datagrid-itemssource-binding-issue
            Focus();
            LanguagesTab.Focus();
        }

        private void DataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            TextBox tb = e.EditingElement as TextBox;
            if (tb != null)
            {
                tb.Focus();
                //you can set caret position and ...
            }
        }
    }
}
