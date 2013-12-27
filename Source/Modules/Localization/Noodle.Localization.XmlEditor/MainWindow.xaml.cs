using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
using Telerik.Windows.Controls;

namespace Noodle.Localization.XmlEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();
        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var tabControl = sender as RadTabControl;
            if(tabControl == null) return;
            foreach(var item in tabControl.Items)
            {
                var tab = tabControl.ItemContainerGenerator.ContainerFromIndex(tabControl.Items.IndexOf(item)) as RadTabItem;
                var children = GetLogicalChildCollection<RadGridView>(tab);
                if(tab == null) return;
            }
        }

        private List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
        {
            var logicalCollection = new List<T>();
            GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
            return logicalCollection;
        }

        private void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            var children = LogicalTreeHelper.GetChildren(parent);
            foreach (var child in children)
            {
                if (!(child is DependencyObject)) continue;
                var depChild = child as DependencyObject;
                if (child is T)
                    logicalCollection.Add(child as T);
                GetLogicalChildCollection(depChild, logicalCollection);
            }
        }
    }
}
