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
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow win = new OptionsWindow();
            this.Close();

            win.ShowDialog();

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            /*
            StreamReader sr = new StreamReader("test.dat");
            string abc = sr.ReadToEnd();
            sr.Dispose();
            Console.WriteLine(abc);
            */

        }
    }
}
