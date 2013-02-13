using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace Noodle.Localization.XmlEditor
{
    public class CustomDataGrid : DataGrid
    {
        public CustomDataGrid()
        {

            var ctxMenu = new ContextMenu();

            var menu1 = new MenuItem { Header = "Cut", Command = ApplicationCommands.Cut, CommandTarget = this };

            var menu2 = new MenuItem { Header = "Copy", Command = ApplicationCommands.Copy, CommandTarget = this };

            var menu3 = new MenuItem { Header = "Paste", Command = ApplicationCommands.Paste, CommandTarget = this };



            ctxMenu.Items.Add(menu2);

            ctxMenu.Items.Add(menu3);

            ctxMenu.Items.Add(menu1);



            this.ContextMenu = ctxMenu;



            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, new ExecutedRoutedEventHandler(MyExecutedRoutedEventHandler), new CanExecuteRoutedEventHandler(CanExecuteRoutedEventHandler)));

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(MyExecutedRoutedEventHandler), new CanExecuteRoutedEventHandler(CanExecuteRoutedEventHandler)));

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, new ExecutedRoutedEventHandler(MyExecutedRoutedEventHandler), new CanExecuteRoutedEventHandler(CanExecuteRoutedEventHandler)));

        }



        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {

            base.OnMouseRightButtonUp(e);



            if (this.ContextMenu != null)
            {

                this.ContextMenu.IsOpen = true;

                e.Handled = true;

            }

        }



        private void MyExecutedRoutedEventHandler(object sender, ExecutedRoutedEventArgs e)
        {

            var i = e.Parameter;

        }



        private void CanExecuteRoutedEventHandler(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = true;

        }

    }
}
