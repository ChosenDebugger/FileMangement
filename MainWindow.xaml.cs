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
    public partial class MainWindow : Window
    {
        public int blockNum = -1;                               //当前文件系统的数据块总数

        private int nextPCBID = 0;                              //下一个生成的PCB的ID

        private FAT disk;                                       //存储对应数目数据块的disk区域

        //private List<bool> bitmap;                            //管理空闲区域的位图

        private FCB rootFolder;                                 //根目录节点

        private FCB currentDirectory = null;                    //当前所在目录地址

        private FCB operatingFolder = null;                     //当前正在操作的文件夹

        private FCB operatingFile = null;                       //当前正在操作的文件

        //主要维护的数据结构
        //Disk<Block>
        //FileTree<FCB>

        private void Init()
        {
            WelcomeWindow welcomeWin = new WelcomeWindow();
            welcomeWin.ShowDialog();

            int systemModel = welcomeWin._systemModel;

            if (systemModel == 1)
                NewSystem();
            if (systemModel == 2)
                LoadSystem();

            //配置数据初始化
            rootFolder = new FCB(Type.Folder, "root", 1, ++nextPCBID);

            currentDirectory = rootFolder;
            disk.AddNewFolder(rootFolder);

            UpdateCurrentDir();
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

        
        private void NewSystem()
        {
            OptionsWindow win = new OptionsWindow();
            win.ShowDialog();
            blockNum = win._blockNum;

            disk = new FAT(blockNum);                       
            
            currentDirectory = rootFolder;

        }

        private void LoadSystem()
        {

        }

        public void NewFolder()
        {
            NewFolderWindow win = new NewFolderWindow();
            win.ShowDialog();
            string newFolderName = win._newFolderName;

            FCB newFolder = new FCB(Type.Folder, newFolderName, ++nextPCBID);
            newFolder.father = currentDirectory;

            currentDirectory.folderSon.Add(newFolder);          //在父节点的文件夹子集中加入
            UpdateAncestorSize(newFolder, newFolder.size);      //祖先节点增重
            disk.AddNewFolder(newFolder);

            FolderComboBox.Items.Add(newFolder.name);
            operatingFolder = newFolder;
            
        }


        private void NewFolder_Button_Click(object sender, RoutedEventArgs e)
        {
            NewFolder();

            UpdateFolderMessage();
        }

        private void UpdateCurrentDir()
        {
            //当前目录显示
            if(currentDirectory!=null)
            {
                Queue<string> ancestor = new Queue<string>();
                ancestor.Enqueue(currentDirectory.name);

                FCB fatherDic = currentDirectory.father;

                while (fatherDic != null)
                    ancestor.Enqueue(fatherDic.name);

                CurrentDirectory_Text.Text = "$\\";
                while (ancestor.Count != 0)
                    CurrentDirectory_Text.Text += ancestor.Dequeue() + "\\";
            }
        }

        //目录下拉表
        //当前文件夹信息
        private void UpdateFolderMessage()
        {
            //编辑选框栏
            FolderComboBox.Text = operatingFolder.name;
            
            //先清空当前框内文本
            FolderText.Text = "";
            //编辑文本
            FolderText.Text += "------------\n\n";
            FolderText.Text += "文件夹大小： " + Convert.ToInt16(operatingFolder.size) + "\n\n";
            FolderText.Text += "-------------\n\n";

        }
        
        //文件下拉表
        //当前文件信息
        private void UpdateFileMessage()
        {

        }

        private void UpdateAncestorSize(FCB currentFCB, int sizeChanged)
        {
            FCB ancestor = currentFCB.father;

            while(ancestor!=null)
            {
                ancestor.size += sizeChanged;
                ancestor = ancestor.father;
            }
        }
    }
}
