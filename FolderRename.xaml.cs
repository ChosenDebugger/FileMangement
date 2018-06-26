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
    /// FolderRename.xaml 的交互逻辑
    /// </summary>
    public partial class FolderRename : Window
    {
        public string _FolderNewName = null;

        public FolderRename()
        {
            InitializeComponent();
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            if (FolderNewName_Text.Text == null)
            {
                MessageBox.Show("请输入有效名称");
                return;
            }

            _FolderNewName = FolderNewName_Text.Text;
            
            this.Close();
        }
    }
}
