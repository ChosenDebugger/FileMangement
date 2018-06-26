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
    public partial class FileShow : Window
    {
        private FAT disk = null;

        private FCB currentFile = null;

        public FileShow(FAT _disk, FCB _currentFile)
        {
            InitializeComponent();

            disk = _disk;
            currentFile = _currentFile;

            FileName_Text.Text = currentFile.name;
            FileSize_Text.Text = Convert.ToString(currentFile.size);

            FileContent_Text.Text = disk.ExtractFileContent(currentFile);
        }

        private void FileEdit_Button_Click(object sender, RoutedEventArgs e)
        {
            FileEdit fileEditWindow = new FileEdit();
            fileEditWindow.ShowDialog();

            string newFileContent = fileEditWindow._newFileContent;

            if (newFileContent == null) return;
            currentFile.size = (newFileContent.Count() / 28 + 1) * 4;
            disk.EditFileContent(currentFile, newFileContent);

            FileSize_Text.Text = Convert.ToString(currentFile.size);
            FileContent_Text.Text = disk.ExtractFileContent(currentFile);
        }
    }
}
