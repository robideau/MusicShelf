using System;
using System.IO; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string directory = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\DirConfig.txt";
            string topdirectory = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            InitializeComponent();
            if (File.Exists(directory)) {
                System.IO.StreamReader configReader = new System.IO.StreamReader(directory);
                FileBrowseButton.Content = configReader.ReadLine();
                ExeFinderButton.Content = topdirectory;
                configReader.Close();
            }
            else {
                System.Windows.MessageBox.Show("DirConfig could not be found.");
            }
        }

        private void fileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            FileBrowseButton.Content = dialog.SelectedPath;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Process.Start(@"F:\Projects\Unity\MusicShelf\MusicShelf.exe");
                Process.Start(@""+ExeFinderButton.Content+"\\MusicShelf.exe");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("An error occured. Please make sure that the MusicShelf .exe is present and healthy.", "Error");
            }
            try
            {
                string dirPath = @""+ExeFinderButton.Content+"\\DirConfig.txt";
                if (File.Exists(dirPath))
                {
                    string line1 = "Music library directory: " + FileBrowseButton.Content + "\r\n";
                    string line2 = "MusicShelf.exe location: " + ExeFinderButton.Content;
                    System.IO.StreamWriter configFile = new System.IO.StreamWriter(dirPath);
                    configFile.Write(line1);
                    configFile.Write(line2);
                    configFile.Close();
                    /*Console.Write("file exists.");
                    TextWriter tw = new StreamWriter(dirPath, true);
                    tw.WriteLine("Music library directory: ");
                    tw.WriteLine(FileBrowseButton.Content);
                    tw.Close();*/
                }
                else
                {
                    System.Windows.MessageBox.Show("DirConfig.txt could not be found.");
                }
            }
            catch (IOException ex)
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("An error occured writing to config file.");
            }
        }

        private void ExeFinderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            ExeFinderButton.Content = dialog.SelectedPath;
        }
    }
}
