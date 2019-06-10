using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Fox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CreateApplicationDataFolders();

            if (e.Args.Length != 0)
            {
                var file = e.Args[0];

                if (e.Args.Length > 1)
                {
                    file = string.Join("", e.Args);

                }

                //Directory.SetCurrentDirectory($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}");

                var mainWindow = new MainWindow(file);
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("The Fox tagger can only be opened with an image file");
            }
        }

        private void CreateApplicationDataFolders()
        {
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Hivemind Software\\Fox"));
        }
    }
}
