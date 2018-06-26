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
    public partial class FileEdit : Window
    {
        public string _newFileContent = null;

        public FileEdit()
        {
            InitializeComponent();
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            if (NewFileContent_Text.Text == null)
            {
                MessageBox.Show("请输入有效字符！");
            }

            _newFileContent = NewFileContent_Text.Text;
            this.Close();
        }
    }
}
