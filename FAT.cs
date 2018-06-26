using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMangement
{
    public class Block
    {
        //一个string

        //一个指针——用于链式标记文件物理存储
        public int ID;

        public int type;            //1表示放4个PCB；2表示放长度为28的string+1个指针

        //type == 1
        public List<FCB> FCBList;

        //type == 2 
        public string data;
        public int nextBlock;

        public Block(int blockID)
        {
            ID = blockID;
            data = null;
            nextBlock = -1;
            type = -1;
        }
    }

    public class FAT
    {
        public const int BLOCK_CONTENT_LENGTH = 28;

        public List<Block> disk;

        public List<bool> bitmap;

        //private int bestBlockIDForNextFCB = -1;

        //private int bestBlockIDForNextFile = -1;

        public FAT(int blockNum)
        {
            disk = new List<Block>();
            bitmap = new List<bool>();

            for (int i = 1; i <= blockNum; i++)
            {
                Block newBlock = new Block(i);

                disk.Add(newBlock);
                bitmap.Add(false);
            }
        }

        private int FindFreeBlock(int type)
        {
            //type 为1表示为FCB 寻找空间
            if(type ==1)
            {
                for (int i = 0; i < disk.Count(); i++)
                    if (bitmap[i] == true && disk[i].type == 1 && disk[i].FCBList.Count() < 4)
                        return i + 1;
                for (int i = 0; i < disk.Count(); i++)
                    if (bitmap[i] == false)
                        return i + 1;
            }


            //type 为2表示为FileContent 寻找空间
            if (type == 2)
            {
                for (int i = 0; i < disk.Count(); i++)
                    if (bitmap[i] == false)
                        return i + 1;
            }

            return -1;
        }

        public void AddNewFCB(FCB newFolder)
        {
            int posID = FindFreeBlock(1);

            if (posID == -1)
            {
                System.Windows.MessageBox.Show("文件系统已满!");
                return;
            }

            bitmap[posID - 1] = true;                   //更新位图
            disk[posID - 1].type = 1;

            newFolder.blockPosID = posID;

            //新Block鉴定
            if(disk[posID-1].FCBList==null)
                disk[posID - 1].FCBList = new List<FCB>();
            
            disk[posID - 1].FCBList.Add(newFolder);     //更新disk

        }

        private int Min(int a, int b) { return a < b ? a : b; }

        public int AddNewFileContent(FCB newFile, string fileContent)
        {
            //return newFile所占数据块数
            int contentLength = fileContent.Count();
            int blockNum = contentLength / BLOCK_CONTENT_LENGTH + 1;

            int firstFreeBlock = FindFreeBlock(2);

            //此时虚拟磁盘已满
            if (firstFreeBlock == -1)
            {
                System.Windows.MessageBox.Show("文件系统已满!");
                return -1;
            }

            newFile.beginBlockID = firstFreeBlock;

            int currentBlockID = newFile.beginBlockID;
            int lastBlockID = -1;
            //int finalBlockID = -1;

            for (int i = 0; i < blockNum; i++)
            {
                bitmap[currentBlockID - 1] = true;
                disk[currentBlockID - 1].type = 2;

                disk[currentBlockID - 1].data = fileContent.Substring(0, Min(BLOCK_CONTENT_LENGTH, fileContent.Length));
                fileContent = fileContent.Remove(0, Min(BLOCK_CONTENT_LENGTH, fileContent.Length));

                //防止中断后已经更改的数据保留
                //使用中继变量：freeBlock
                int freeBlock = FindFreeBlock(2);
                if (freeBlock == -1)
                {
                    //磁盘满载判断，但目前没有释放前循环已占有的内存的操作
                    System.Windows.MessageBox.Show("文件系统已满!");
                    return -1;
                }
                //更新指针
                lastBlockID = currentBlockID;
                currentBlockID = freeBlock;

                disk[lastBlockID - 1].nextBlock = currentBlockID;
                //disk[currentBlockID - 1].nextBlock = bestBlockIDForNextFile;
            }

            newFile.endBlockID = lastBlockID;
            disk[lastBlockID - 1].nextBlock = -1;
            
            return blockNum;
        }

        public void RemoveFCB(FCB targetFolder)
        {
            if (targetFolder == null) return;
            disk[targetFolder.blockPosID - 1].FCBList.Remove(targetFolder);

            //如果此时该disk内PCBList已空
            //就完全初始化
            if (disk[targetFolder.blockPosID - 1].FCBList.Count() == 0) 
            {
                disk[targetFolder.blockPosID - 1].FCBList = null;
                disk[targetFolder.blockPosID - 1].type = -1;
                bitmap[targetFolder.blockPosID - 1] = false;
            }
        }

        public  void RemoveFileContent(FCB targetFile)
        {
            //disk[targetFile.blockPosID - 1].FCBList.Remove(targetFile);
            if (targetFile == null) return;

            int currentBlock = targetFile.beginBlockID;
            int nextBlock = disk[currentBlock - 1].nextBlock;

            while(true)
            {
                disk[currentBlock - 1].data = null;
                disk[currentBlock - 1].nextBlock = -1;
                disk[currentBlock - 1].type = -1;
                bitmap[currentBlock - 1] = false;

                if (disk[currentBlock - 1].nextBlock == -1) break;

                currentBlock = disk[currentBlock - 1].nextBlock;
                nextBlock = disk[currentBlock - 1].nextBlock;
            }
            targetFile.beginBlockID = -999;
            targetFile.endBlockID = -999;
        }

        public void EditFileContent(FCB targetFile,string newContent)
        {
            if (targetFile == null) return;

            RemoveFileContent(targetFile);
            AddNewFileContent(targetFile, newContent);
        }

        public string ExtractFileContent(FCB targetFile)
        {
            string wholeContent = "";

            int currentBlockID = targetFile.beginBlockID;

            while(true)
            {
                wholeContent += disk[currentBlockID - 1].data;

                if (disk[currentBlockID - 1].nextBlock == -1) break;
                else currentBlockID = disk[currentBlockID - 1].nextBlock;
            }
            return wholeContent;
        }

    }
}
