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
            operatingFolder = null;

            disk.AddNewFolder(rootFolder);

            UpdateCurrentDir();
            UpdateFolderCombobox();
            UpdateFolderText();
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

        private void NewFolder()
        {

        }

        private void DeleteFolder()
        {

        }

        private void EnterFolder()
        {

        }

        private void ReturnFolder()
        {

        }

        private void NewFile()
        {
            //if (win._newFileName == null) return;
        }

        private void DeleteFile()
        {

        }

        private void SaveFile()
        {

        }

        private void UpdateCurrentDir()
        {
            //当前目录显示
            if(currentDirectory!=null)
            {
                Stack<string> ancestor = new Stack<string>();
                ancestor.Push(currentDirectory.name);

                FCB fatherDic = currentDirectory.father;

                while (fatherDic != null)
                {
                    ancestor.Push(fatherDic.name);
                    fatherDic = fatherDic.father;
                }
                CurrentDirectory_Text.Text = "$\\";
                while (ancestor.Count != 0)
                    CurrentDirectory_Text.Text += ancestor.Pop() + "\\";
            }
        }

        //目录下拉表
        private void UpdateFolderCombobox()
        {
            FolderComboBox.Items.Clear();

            //全新的还没有添加子文件的文件夹
            if (currentDirectory.folderSon.Count() == 0) return;

            //编辑下拉选单
            for (int i = 0; i < currentDirectory.folderSon.Count(); i++)
            {
                FolderComboBox.Items.Add(currentDirectory.folderSon[i].name);
                if (currentDirectory.folderSon[i] == operatingFolder)
                    FolderComboBox.SelectedIndex = i;
            }
        }

        //当前文件夹信息
        private void UpdateFolderText()
        {
            FolderText.Text = "";

            //operatingFolder 为null时，显示currentDirectory的信息
            if (operatingFolder == null)
            {
                //编辑文本
                FolderText.Text += "------------\n\n";
                FolderText.Text += "文件夹名： " + currentDirectory.name + "\n\n";
                FolderText.Text += "文件夹大小： " + currentDirectory.size + "\n\n";
                FolderText.Text += "-------------\n\n";
            }
            else
            {
                //编辑文本
                FolderText.Text += "------------\n\n";
                FolderText.Text += "文件夹名： " + operatingFolder.name + "\n\n";
                FolderText.Text += "文件夹大小： " + operatingFolder.size + "\n\n";
                FolderText.Text += "-------------\n\n";
            }
            //文件夹包含的子文件夹和子文件信息
        }
        
        //文件下拉表
        //当前文件信息
        private void UpdateFileMessage()
        {

        }

        //更新祖先的size
        private void UpdateAncestorSize(FCB currentFCB, int sizeChanged)
        {
            FCB ancestor = currentFCB.father;

            while(ancestor!=null)
            {
                ancestor.size += sizeChanged;
                ancestor = ancestor.father;
            }
        }

        private FCB Recursion_Serach(FCB rootNode,string targetName)
        {
            if (rootNode.folderSon.Count() != 0)
            {
                for (int i = 0; i < rootNode.folderSon.Count(); i++)  
                {
                    return Recursion_Serach(rootNode.folderSon[i], targetName);
                }
            }

            if (rootNode.name == targetName) return rootNode;
            else return null;
        }

        private void Recursion_Delete(FCB rootNode)
        {
            //递归删除所有子文件夹
            if(rootNode.folderSon.Count()!=0)
            {
                for (int i = 0; i < rootNode.folderSon.Count(); i++)
                {
                    Recursion_Delete(rootNode.folderSon[i]);
                }
            }
            //释放内存
            //尝试直接释放头节点
            rootNode.father.folderSon.Remove(rootNode);
            disk.RemoveFolder(rootNode);

            rootNode = null;
            GC.Collect();
        }

        //遍历搜索FCB节点
        private FCB FindFCBName(string targetName)
        {
            return Recursion_Serach(rootFolder, targetName);
        }

        private void NewFolder_Button_Click(object sender, RoutedEventArgs e)
        {
            NewFolderWindow win = new NewFolderWindow();
            win.ShowDialog();
            if (win._newFolderName == null) return;

            string newFolderName = win._newFolderName;

            FCB newFolder = new FCB(Type.Folder, newFolderName, 1, ++nextPCBID);
            newFolder.father = currentDirectory;

            currentDirectory.folderSon.Add(newFolder);          //在父节点的文件夹子集中加入
            UpdateAncestorSize(newFolder, newFolder.size);      //祖先节点增重
            disk.AddNewFolder(newFolder);

            operatingFolder = newFolder;

            UpdateFolderCombobox();
            UpdateFolderText();
        }

        private void DeleteFolder_Button_Click(object sender, RoutedEventArgs e)
        {
            FCB fatherFolder = operatingFolder.father;

            //更新FCB树
            Recursion_Delete(operatingFolder);

            //更新祖先节点size
            UpdateAncestorSize(operatingFolder, 0 - operatingFolder.size);

            //决定下一个操作的文件夹
            if (fatherFolder.folderSon.Count() == 0)
                operatingFolder = fatherFolder;
            else
                operatingFolder = fatherFolder.folderSon[0];

            UpdateCurrentDir();
            UpdateFolderCombobox();
            UpdateFolderText();
        }

        private void EnterFolder_Button_Click(object sender, RoutedEventArgs e)
        {
            if (operatingFolder == null)
            {
                MessageBox.Show("请先创建子文件夹！");
                return;
            }

            currentDirectory = operatingFolder;

            if (operatingFolder.folderSon.Count() == 0)
                operatingFolder = null;
            else
                operatingFolder = operatingFolder.folderSon[0];

            UpdateCurrentDir();
            UpdateFolderCombobox();
            UpdateFolderText();
        }

        private void ReturnFolder_Button_Click(object sender, RoutedEventArgs e)
        {
            if(currentDirectory.father==null)
            {
                MessageBox.Show("目前已打开根目录！");
                return;
            }
            currentDirectory = currentDirectory.father;
            operatingFolder = currentDirectory.folderSon[0];

            UpdateCurrentDir();
            UpdateFolderCombobox();
            UpdateFolderText();
        }

        private void FolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //要实现切换combobox的时候转换operatingFolder并更新面板信息

            if (FolderComboBox.SelectedIndex >= 0 && FolderComboBox.SelectedIndex <= 3)
            {
                operatingFolder = operatingFolder.father.folderSon[FolderComboBox.SelectedIndex];
                UpdateFolderText();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }
    }
}
