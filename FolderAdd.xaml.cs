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
    public partial class FolderAdd : Window
    {
        public string _newFolderName = null;

        public FolderAdd()
        {
            InitializeComponent();
        }

        private void Confrim_Button_Click(object sender, RoutedEventArgs e)
        {
            _newFolderName = NewFolderName_Text.Text;

            this.Close();
        }
    }
}
