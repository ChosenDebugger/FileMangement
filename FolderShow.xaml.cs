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
using System.Windows.Forms;


namespace FileMangement
{
    public partial class FolderShow : Window
    {
        public const int BLOCK_CONTENT_LENGTH = 12;             //块的数据容量度量

        private int blockNum = -1;                              //存储器总块数

        private FAT disk;                                       //虚拟磁盘

        private int FCBID = -1;                                 //下一个生成的PCB的ID

        private FCB rootFolder;                                 //根目录节点

        private FCB currentDirectory = null;                    //当前所在目录地址

        //private ContextMenu rightClickMenu;                   //右键菜单

        //private FCB operatingFolder = null;                   //当前正在操作的文件夹

        //private FCB currentFile = null;                       //当前正在操作的文件

        public FolderShow(int _blockNum)
        {
            InitializeComponent();

            blockNum = _blockNum;

            FCBID = 0;
            disk = new FAT(blockNum);

            rootFolder = new FCB(Type.Folder, "root", 1, ++FCBID);
            disk.AddNewFCB(rootFolder);

            Init();
        }

        //利用重载实现Load
        public FolderShow(string systemPath)
        {
            InitializeComponent();

            StreamReader reader = new StreamReader(systemPath + "\\BlockNum.dat");
            blockNum = Convert.ToInt16(reader.ReadLine());

            FCBID = 0;
            disk = new FAT(blockNum);

            SystemRecover(systemPath);

            Init();
        }

        private void Init()
        {
            currentDirectory = rootFolder;

            FCBList.ContextMenu = new System.Windows.Controls.ContextMenu();

            System.Windows.Controls.MenuItem menuItem_Rename =
                new System.Windows.Controls.MenuItem { Header = "重命名" };

            menuItem_Rename.Click += new RoutedEventHandler(Rename_Click);

            System.Windows.Controls.MenuItem menuItem_Delete =
                new System.Windows.Controls.MenuItem { Header = "删除" };

            menuItem_Delete.Click += new RoutedEventHandler(Delete_Click);

            FCBList.ContextMenu.Items.Add(menuItem_Rename);
            FCBList.ContextMenu.Items.Add(menuItem_Delete);

            UpdateCurrentDir();
            UpdateFCBList();
        }

        private void RecoverFile(FCB rootNode, int BlockIndex, string systemPath)
        {
            disk.disk[BlockIndex - 1].data = "";
            disk.bitmap[BlockIndex - 1] = true;

            StreamReader reader = new StreamReader(systemPath + "\\DataBlock_" + Convert.ToString(BlockIndex) + ".dat");

            disk.disk[BlockIndex - 1].type = Convert.ToInt16(reader.ReadLine());
            disk.disk[BlockIndex - 1].data += reader.ReadLine();
            disk.disk[BlockIndex - 1].nextBlock = Convert.ToInt16(reader.ReadLine());

            if (disk.disk[BlockIndex - 1].nextBlock != -1)
                RecoverFile(rootNode, disk.disk[BlockIndex - 1].nextBlock, systemPath);
        }

        private void RecoverTree(FCB rootNode, int FCBID, FCB father, string systemPath)
        {
            string fullSystemPath = systemPath + "\\FCBBlock.dat";

            rootNode.father = father;
            rootNode.fileSon = new List<FCB>();
            rootNode.folderSon = new List<FCB>();

            StreamReader reader = new StreamReader(fullSystemPath);
            string singleLine = reader.ReadLine();

            while (Convert.ToInt16(singleLine.Substring(0, singleLine.IndexOf(" ") + 1)) != FCBID)
            {
                singleLine = reader.ReadLine();
            }

            rootNode.ID = Convert.ToInt16(singleLine.Substring(0, singleLine.IndexOf(" ")));
            singleLine = singleLine.Remove(0, singleLine.IndexOf(" ") + 1);

            rootNode.type = (Type)Convert.ToInt16(singleLine.Substring(0, singleLine.IndexOf(" ")));          ///////////////////problem
            singleLine = singleLine.Remove(0, singleLine.IndexOf(" ") + 1);

            rootNode.name = singleLine.Substring(0, singleLine.IndexOf(" "));
            singleLine = singleLine.Remove(0, singleLine.IndexOf(" ") + 1);

            rootNode.size = Convert.ToInt16(singleLine.Substring(0, singleLine.IndexOf(" ")));
            singleLine = singleLine.Remove(0, singleLine.IndexOf(" ") + 1);

            rootNode.blockPosID = Convert.ToInt16(singleLine.Substring(0, singleLine.IndexOf(" ")));
            singleLine = singleLine.Remove(0, singleLine.IndexOf(" ") + 1);

            //更新disk 的type
            disk.disk[rootNode.blockPosID - 1].type = 1;
            //更新disk 内Block的FCBList
            if (disk.disk[rootNode.blockPosID - 1].FCBList == null)
                disk.disk[rootNode.blockPosID - 1].FCBList = new List<FCB>();
            disk.disk[rootNode.blockPosID - 1].FCBList.Add(rootNode);
            //更新位图
            disk.bitmap[rootNode.blockPosID - 1] = true;

            if (rootNode.type == Type.File)
            {
                //父节点添加该节点指针
                father.fileSon.Add(rootNode);

                rootNode.beginBlockID = Convert.ToInt16(singleLine.Substring(0, singleLine.IndexOf(" ")));
                singleLine = singleLine.Remove(0, singleLine.IndexOf(" ") + 1);

                rootNode.endBlockID = Convert.ToInt16(singleLine.Substring(0, singleLine.IndexOf(" ")));
                singleLine = singleLine.Remove(0, singleLine.IndexOf(" ") + 1);

                RecoverFile(rootNode, rootNode.beginBlockID, systemPath);
            }
            else if (rootNode.type == Type.Folder)
            {
                //父节点添加该节点指针
                if (father != null) { father.folderSon.Add(rootNode); }

                //提取子文件夹FCB 的索引
                List<int> folderSonIndex = new List<int>();

                singleLine = singleLine.Remove(0, singleLine.IndexOf("(") + 1);
                while (!singleLine.Substring(0, 1).Equals(")"))
                {
                    folderSonIndex.Add(Convert.ToInt16(singleLine.Substring(0, singleLine.IndexOf(" "))));
                    singleLine = singleLine.Remove(0, singleLine.IndexOf(" ") + 1);
                }
                singleLine = singleLine.Remove(0, 1);
                //递归添加子节点
                for (int i = 0; i < folderSonIndex.Count(); i++)
                {
                    FCB newFolder = new FCB(Type.Folder, null, 1, folderSonIndex[i]);
                    RecoverTree(newFolder, folderSonIndex[i], rootNode, systemPath);
                }

                //提取子文件FCB 的索引
                List<int> fileSonIndex = new List<int>();

                singleLine = singleLine.Remove(0, singleLine.IndexOf("(") + 1);
                while (!singleLine.Substring(0, 1).Equals(")"))
                {
                    fileSonIndex.Add(Convert.ToInt16(singleLine.Substring(0, singleLine.IndexOf(" "))));
                    singleLine = singleLine.Remove(0, singleLine.IndexOf(" ") + 1);
                }
                singleLine = singleLine.Remove(0, 1);

                //根据Index 恢复该FCB的文件子集
                for (int i = 0; i < fileSonIndex.Count(); i++)
                {
                    FCB newFile = new FCB(Type.File, null, 1, fileSonIndex[i]);
                    RecoverTree(newFile, fileSonIndex[i], rootNode, systemPath);
                }
            }
            return;
        }

        //根据路径名找到FCBBLock.dat 文件
        //根据文件复原加载系统的FCB tree
        private void SystemRecover(string systemPath)
        {
            rootFolder = new FCB(Type.Folder, "root", ++FCBID);
            //basic Information Recover
            RecoverTree(rootFolder, 1, null, systemPath);
        }

        private void UpdateCurrentDir()
        {
            if (currentDirectory == null) return;

            FCBInformation.Clear();

            //编辑文本
            FCBInformation.Text += "------------\n";
            FCBInformation.Text += "文件夹名： " + currentDirectory.name + "\n\n";
            FCBInformation.Text += "文件夹大小： " + currentDirectory.size + "\n";
            FCBInformation.Text += "-------------\n";

            //编辑List
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

        private void UpdateFCBList()
        {
            FCBList.Items.Clear();

            for (int i = 0; i < currentDirectory.folderSon.Count(); i++)
            {
                FCBList.Items.Add(currentDirectory.folderSon[i].name);
            }
            for (int i = 0; i < currentDirectory.fileSon.Count(); i++)
            {
                FCBList.Items.Add(currentDirectory.fileSon[i].name);
            }
        }

        //更新祖先的size
        private void UpdateAncestorSize(FCB currentFCB, int sizeChanged)
        {
            FCB ancestor = currentFCB.father;

            while (ancestor != null)
            {
                ancestor.size += sizeChanged;
                ancestor = ancestor.father;
            }
        }

        private FCB Recursion_Serach_Folder(FCB rootNode, string targetName)
        {
            if (rootNode.name == targetName) return rootNode;

            for (int i = 0; i < rootNode.folderSon.Count(); i++)
            {
                FCB currentNode = Recursion_Serach_Folder(rootNode.folderSon[i], targetName);

                if (currentNode == null) continue;
                if (currentNode.name == targetName) return currentNode;
            }

            return null;
        }

        private FCB Recursion_Serach_Folder(FCB rootNode, int targetID)
        {
            if (rootNode.ID == targetID) return rootNode;

            for (int i = 0; i < rootNode.folderSon.Count(); i++)
            {
                FCB currentNode = Recursion_Serach_Folder(rootNode.folderSon[i], targetID);

                if (currentNode == null) continue;
                if (currentNode.ID == targetID) return currentNode;
            }

            return null;
        }

        private FCB Recursion_Search_File(FCB rootNode, string targetName)
        {
            for (int i = 0; i < rootNode.fileSon.Count(); i++)
            {
                FCB currentNode = rootNode.fileSon[i];

                if (currentNode.name == targetName)
                    return currentNode;
            }

            for (int i = 0; i < rootNode.folderSon.Count(); i++)
            {
                FCB currentNode = Recursion_Search_File(rootNode.folderSon[i], targetName);

                if (currentNode == null) continue;
                if (currentNode.name == targetName) return currentNode;
            }
            return null;
        }

        private FCB Recursion_Search_File(FCB rootNode, int targetID)
        {
            for (int i = 0; i < rootNode.fileSon.Count(); i++)
            {
                FCB currentNode = rootNode.fileSon[i];

                if (currentNode.ID == targetID)
                    return currentNode;
            }

            for (int i = 0; i < rootNode.folderSon.Count(); i++)
            {
                FCB currentNode = Recursion_Search_File(rootNode.folderSon[i], targetID);

                if (currentNode == null) continue;
                if (currentNode.ID == targetID) return currentNode;
            }
            return null;
        }

        private void Recursion_Delete(FCB rootNode)
        {
            //递归删除所有子文件夹

            for (int i = 0; i < rootNode.folderSon.Count(); i++)
            {
                Recursion_Delete(rootNode.folderSon[i]);
            }

            for (int i = 0; i < rootNode.fileSon.Count(); i++)
            {
                rootNode.fileSon.Remove(rootNode);
                disk.RemoveFileContent(rootNode.fileSon[i]);
            }
            
            //释放内存
            //尝试直接释放头节点
            if (rootNode.father != null)
                rootNode.father.folderSon.Remove(rootNode);

            disk.RemoveFCB(rootNode);

            rootNode = null;
            GC.Collect();
        }

        //依据name遍历搜索FCB
        private FCB FindFCBName(string targetName)
        {
            FCB result;
            result = Recursion_Serach_Folder(rootFolder, targetName);

            if (result == null)
                result = Recursion_Search_File(rootFolder, targetName);

            return result;
        }

        //依据ID遍历搜索FCB
        private FCB FindFCBID(int targetID)
        {
            FCB result;
            result = Recursion_Serach_Folder(rootFolder, targetID);

            if (result == null)
                result = Recursion_Search_File(rootFolder, targetID);

            return result;
        }

        private void NewFolder_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderAdd folderAddWindow = new FolderAdd();
            folderAddWindow.ShowDialog();

            string name = folderAddWindow._newFolderName;
            
            //窗口直接关闭的情况
            if (name == null) return;

            //命名含有非法字符检测
            if (name.Contains(" ") || name == "")
            {
                System.Windows.MessageBox.Show("含有非法字符");
                return;
            }

            //重名检测
            if(FindFCBName(name)!=null)
            {
                System.Windows.MessageBox.Show("“" + name + "”已存在");
                return;
            }

            string newFolderName = folderAddWindow._newFolderName;
            FCB newFolder = new FCB(Type.Folder, newFolderName, 1, ++FCBID);

            newFolder.father = currentDirectory;
            currentDirectory.folderSon.Add(newFolder);
            UpdateAncestorSize(newFolder, newFolder.size);

            disk.AddNewFCB(newFolder);

            UpdateCurrentDir();
            UpdateFCBList();
        }

        private void NewFile_Button_Click(object sender, RoutedEventArgs e)
        {
            FileAdd fileAddWindow = new FileAdd();
            fileAddWindow.ShowDialog();

            string name = fileAddWindow._newFileName;

            //窗体直接关闭
            if (name == null) return;

            //命名含有非法字符检测
            if (name.Contains(" ") || name == "") 
            {
                System.Windows.MessageBox.Show("含有非法字符");
                return;
            }

            //重名检测
            if (FindFCBName(name) != null)
            {
                System.Windows.MessageBox.Show("“" + name + "”已存在");
                return;
            }

            string newFileName = fileAddWindow._newFileName;
            string newFileContent = fileAddWindow._newFileContent;
            int size = newFileContent.Count() / BLOCK_CONTENT_LENGTH + 1;

            FCB newFile = new FCB(Type.File, newFileName, size, ++FCBID);

            newFile.father = currentDirectory;
            currentDirectory.fileSon.Add(newFile);
            UpdateAncestorSize(newFile, size);

            disk.AddNewFCB(newFile);
            disk.AddNewFileContent(newFile, newFileContent);

            UpdateCurrentDir();
            UpdateFCBList();
        }

        private void ReturnkFolder_Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentDirectory == rootFolder)
            {
                System.Windows.MessageBox.Show("无上级目录");
                return;
            }

            currentDirectory = currentDirectory.father;

            UpdateCurrentDir();
            UpdateFCBList();
        }

        private void FCBList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //无选中项目
            if (FCBList.SelectedItem == null) return;

            string targetName = FCBList.SelectedItem.ToString();

            for (int i = 0; i < currentDirectory.folderSon.Count(); i++)
            {
                if (currentDirectory.folderSon[i].name == targetName)
                {
                    currentDirectory = currentDirectory.folderSon[i];
                    UpdateCurrentDir();
                    UpdateFCBList();

                    break;
                }
            }

            for (int i = 0; i < currentDirectory.fileSon.Count(); i++)
            {
                if (currentDirectory.fileSon[i].name == targetName)
                {
                    int beforeSize = currentDirectory.fileSon[i].size;

                    FileShow fileShow = new FileShow(disk, currentDirectory.fileSon[i]);
                    fileShow.ShowDialog();

                    int afterSize = currentDirectory.fileSon[i].size;

                    UpdateAncestorSize(currentDirectory.fileSon[i], afterSize - beforeSize);

                    UpdateCurrentDir();
                    break;
                }
            }
        }

        private void Rename_Click(object sender, EventArgs e)
        {
            FCB currentFCB = FindFCBName(FCBList.SelectedItem.ToString());

            FolderRename folderRenameWindow = new FolderRename();
            folderRenameWindow.ShowDialog();

            if (folderRenameWindow._FolderNewName == null) return;

            string newName = folderRenameWindow._FolderNewName;
            if(FindFCBName(newName)!=null)
            {
                System.Windows.MessageBox.Show("已存在“" + newName + "”");
                return;
            }

            currentFCB.name = newName;

            UpdateFCBList();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            FCB currentFCB = FindFCBName(FCBList.SelectedItem.ToString());

            if (currentFCB == null) return;

            else if (currentFCB.type == Type.File)
            {
                UpdateAncestorSize(currentFCB, -(currentFCB.size));
                disk.RemoveFileContent(currentFCB);
                currentFCB.father.fileSon.Remove(currentFCB);
            }

            else if (currentFCB.type == Type.Folder)
            {
                UpdateAncestorSize(currentFCB, -(currentFCB.size));
                Recursion_Delete(currentFCB);
            }

            disk.RemoveFCB(currentFCB);

            UpdateCurrentDir();
            UpdateFCBList();
        }

        private void Format_Button_Click(object sender, RoutedEventArgs e)
        {
            Recursion_Delete(rootFolder);
            disk = null;
            GC.Collect();

            FCBID = 0;
            disk = new FAT(blockNum);

            rootFolder = new FCB(Type.Folder, "root", 1, ++FCBID);
            disk.AddNewFCB(rootFolder);

            Init();
        }

        private void SystemFolderClear(string systemPath)
        {
            DirectoryInfo dir = new DirectoryInfo(systemPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录

            foreach (FileSystemInfo i in fileinfo)
                File.Delete(i.FullName);                            //删除指定文件
        }
    
        private void SaveAndExit_Button_Click(object sender, RoutedEventArgs e)
        {
            string systemPath = "";
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.RootFolder = Environment.SpecialFolder.Desktop;

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                systemPath = folderBrowser.SelectedPath;
            else if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            //清空当前文件夹
            SystemFolderClear(systemPath);

            //新建文件流指针
            StreamWriter writer;

            writer = new StreamWriter(systemPath + "\\BlockNum.dat", false);
            writer.WriteLine(Convert.ToString(blockNum), false);
            writer.Close(); 

            for (int i = 0; i < blockNum; i++)
            {
                Block currentBlock = disk.disk[i];

                //因为type为-1的Block没有存储任何信息可以直接还原
                //所以不对其做存储操作
                if (currentBlock.type == -1) { }
                
                //将所有type为1的Block存储在单个文件内
                //以保证目录树的完整还原
                else if (currentBlock.type == 1) 
                {
                    writer = new StreamWriter(systemPath + "\\FCBBlock.dat", true);

                    for (int j = 0; j < currentBlock.FCBList.Count(); j++)
                    {
                        FCB currentFCB = disk.disk[i].FCBList[j];
                        
                        //文件型FCB 
                        if (currentFCB.type == Type.File)
                        {
                            writer.WriteLine(currentFCB.ID + " " + 2 + " " + currentFCB.name + " " + currentFCB.size + " " +
                                currentFCB.blockPosID + " " + currentFCB.beginBlockID + " " + currentFCB.endBlockID + " ");
                        }
                        //文件夹型FCB 
                        else if (currentFCB.type == Type.Folder)
                        {
                            writer.Write(currentFCB.ID + " " + 1 + " " + currentFCB.name + " " +
                                currentFCB.size + " " + currentFCB.blockPosID + " ");
                            
                            writer.Write("(");
                            for (int k = 0; k < currentFCB.folderSon.Count(); k++)
                                writer.Write(currentFCB.folderSon[k].ID + " ");
                            writer.Write(")");

                            writer.Write("(");
                            for (int k = 0; k < currentFCB.fileSon.Count(); k++)
                                writer.Write(currentFCB.fileSon[k].ID + " ");
                            writer.Write(") ");

                            writer.WriteLine("");
                        }
                    }
                    writer.Close();
                }

                //将所有type为2的Block存储在不同文件中
                else if (currentBlock.type == 2) 
                {
                    writer = new StreamWriter(systemPath + "\\DataBlock_" + Convert.ToString(currentBlock.ID) + ".dat", false);

                    writer.WriteLine(currentBlock.type);
                    //writer.WriteLine(currentBlock.ID); //不需要
                    writer.WriteLine(currentBlock.data);
                    writer.WriteLine(currentBlock.nextBlock);

                    writer.Close();
                }

            }
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }
    }
}
