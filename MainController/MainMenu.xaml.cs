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
            LoadSystem preWindow = new LoadSystem();

            preWindow.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }

    }
}
