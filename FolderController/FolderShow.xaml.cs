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
    public partial class FolderShow : Window
    {
        private int blockNum = -1;                              //存储器总块数

        private FAT disk;                                       //虚拟磁盘

        private int FCBID = 0;                                  //下一个生成的PCB的ID
        
        private FCB rootFolder;                                 //根目录节点

        private FCB currentDirectory = null;                    //当前所在目录地址

        //private FCB operatingFolder = null;                     //当前正在操作的文件夹

        private FCB currentFile = null;                       //当前正在操作的文件
        

        public FolderShow(int _blockNum)
        {
            InitializeComponent();

            blockNum = _blockNum;

            Init();
        }

        //利用重载实现Load
        public FolderShow(int _blockNum, string disk)
        {

        }

        private void Init()
        {
            rootFolder = new FCB(Type.Folder, "rootFolder", 1, ++FCBID);

            currentDirectory = rootFolder;


        }

        private void UpdateFCBList()
        {
            FCBList.Items.Clear();

            for (int i = 0; i < currentDirectory.folderSon.Count(); i++)
                FCBList.Items.Add(currentDirectory.folderSon[i].name);

            for (int i = 0; i < currentDirectory.fileSon.Count(); i++)
                FCBList.Items.Add(currentDirectory.fileSon[i].name);
        }


        private void NewFolder_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderAdd folderAddWindow = new FolderAdd();
            folderAddWindow.Show();

            if (folderAddWindow._newFolderName == null) return;

            string newFolderName = folderAddWindow._newFolderName;
            FCB newFolder = new FCB(Type.Folder, newFolderName, 1, ++FCBID);

            newFolder.father = currentDirectory;
            currentDirectory.folderSon.Add(newFolder);
            disk.AddNewFCB(newFolder);

            UpdateFCBList();
        }

        private void ReturnkFolder_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FCBList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
