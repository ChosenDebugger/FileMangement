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
    public partial class FileAdd : Window
    {
        public string _newFileName = null;
        public string _newFileContent = null;

        public FileAdd()
        {
            InitializeComponent();
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            _newFileName = NewFileName_Text.Text;
            _newFileContent = NewFileContent_Text.Text;

            this.Close();
        }
    }
}
