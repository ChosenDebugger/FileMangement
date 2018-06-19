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
using System.IO;

namespace FileMangement
{
    /// <summary>
    /// WelcomeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        public int _systemModel = -1;                //1表示新建系统；2表示加载系统

        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            //在MainWindow中打开Options窗体设置初始值
            _systemModel = 1;

            this.Hide();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            //在Mainwindow中根据文件信息复刻上一版本的文件系统
            _systemModel = 2;

            //打开浏览窗口，加载文件

            this.Hide();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }
    }
}
