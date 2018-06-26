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
    public partial class NewSystemWindow : Window
    {
        public int _blockNum = -1;

        public NewSystemWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BlockNumText.Text == "")
            {
                MessageBox.Show("请先输入磁盘块数");
                return;
            }

            _blockNum = Convert.ToInt32(BlockNumText.Text);

            if (_blockNum > 0)
                this.Close();
            else
                MessageBox.Show("请输入有效数字！");
        }
    }
}
