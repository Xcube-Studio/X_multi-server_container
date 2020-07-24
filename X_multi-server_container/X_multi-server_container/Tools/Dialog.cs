using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace X_multi_server_container.Tools
{
    public static class DialogAPI
    {
        public static void MessageBoxShow(string title, string content)
        {
            using (TaskDialog dialog = new TaskDialog())
            {
                dialog.WindowTitle = title;
                dialog.MainInstruction = title;
                dialog.Content = content;
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Close));
                dialog.ShowDialog(Application.Current.MainWindow);
            }
        }

    }
}
