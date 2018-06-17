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
using System.IO;

namespace FileMangement
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //主要维护的数据结构
        //Disk<Block>
        //FileTree<FCB>

        private void Init()
        {
            WelcomeWindow win1 = new WelcomeWindow();
            win1.ShowDialog();

        }

        /*private void FileRWtest()
        {
            int[] abc = new int[4];
            for (int i = 0; i < 4; i++)
                abc[i] = i;

            StreamWriter sw = new StreamWriter("test.dat");

            for (int i = 0; i < 4; i++)
            {
                sw.Write(i);
                sw.Write("\n");
            }
            sw.Dispose();

        }*/

        public MainWindow()
        {
            InitializeComponent();

            Init();
            
        }
    }
}
