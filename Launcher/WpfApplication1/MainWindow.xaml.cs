using System;
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
            InitializeComponent();
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
        }

        private void ExeFinderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            ExeFinderButton.Content = dialog.SelectedPath;
        }
    }
}
