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
    /// <summary>
    /// NewFolderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewFolderWindow : Window
    {
        public string _newFolderName;

        public NewFolderWindow()
        {
            InitializeComponent();
        }

        private void NewFolderName_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            _newFolderName = NewFolderName_Text.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
