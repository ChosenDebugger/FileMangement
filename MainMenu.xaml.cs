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
using System.Windows.Shapes;
using System.Windows.Forms;

namespace FileMangement
{
    public partial class MainMenu : Window
    {
        private int blockNum = -1;

        public MainMenu()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            NewSystemWindow preWindow = new NewSystemWindow();
            preWindow.ShowDialog();

            blockNum = preWindow._blockNum;

            if (blockNum != -1)
            {
                FolderShow mainWindow = new FolderShow(blockNum);
                mainWindow.Show();
                this.Hide();
            }
                
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string _systemPath = "";
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.RootFolder = Environment.SpecialFolder.Desktop;

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                _systemPath = folderBrowser.SelectedPath;

            FolderShow mainWindow = new FolderShow(_systemPath);
            mainWindow.Show();
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }

    }
}
