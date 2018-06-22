using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMangement
{
    class Block
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

    class FAT
    {
        public const int BLOCK_CONTENT_LENGTH = 28;

        public List<Block> disk;

        private List<bool> bitmap;

        private int bestBlockIDForNextFCB = -1;

        private int bestBlockIDForNextFile = -1;

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

            bestBlockIDForNextFCB = 1;
            disk[0].type = 1;
            disk[0].FCBList = new List<FCB>();

            bestBlockIDForNextFile = 2;
            disk[1].type = 2;
        }

        public void AddNewFCB(FCB newFolder)
        {
            int posID = bestBlockIDForNextFCB;

            newFolder.blockPosID = posID;
            
            //if(disk[posID-1].FCBList==null)             //FCBList还未初始化的情况
              //  disk[posID - 1].FCBList = new List<FCB>();

            disk[posID - 1].FCBList.Add(newFolder);     //更新disk

            bitmap[posID - 1] = true;                   //更新位图

            if (disk[posID - 1].FCBList.Count() == 4)
            {
                //该块已满
                //寻找下一个放置FCB的磁盘块
                for (int i = 0; i < disk.Count(); i++)
                {
                    //这里要满足该block没有且不会在下一步被用作file_data
                    if (disk[i].type == -1 && bitmap[i] == false)
                    {
                        bestBlockIDForNextFCB = i + 1;
                        disk[i].type = 1;
                        disk[i].FCBList = new List<FCB>();
                    }
                }
            }
        }

        private int Min(int a, int b) { return a < b ? a : b; }

        public int AddNewFileContent(FCB newFile, string fileContent)
        {
            //return newFile所占数据块数
            int contentLength = fileContent.Count();
            int blockNum = contentLength / BLOCK_CONTENT_LENGTH + 1;

            newFile.beginBlockID = bestBlockIDForNextFile;

            int currentBlockID = -1;

            for (int i = 0; i < blockNum; i++)
            {
                currentBlockID = bestBlockIDForNextFile;

                //if (disk[currentBlockID - 1].data == null)
                //  disk[currentBlockID - 1].data = new string[20];
                disk[currentBlockID - 1].data = fileContent.Substring(0, Min(BLOCK_CONTENT_LENGTH, fileContent.Length));
                fileContent.Remove(0, Min(BLOCK_CONTENT_LENGTH, fileContent.Length));

                bitmap[currentBlockID - 1] = true;

                //更新bestBlockIDForNextFile
                for (int j = 0; j < disk.Count(); j++)
                {
                    if (disk[j].type == -1 && bitmap[j] == false)
                    {
                        bestBlockIDForNextFile = j + 1;
                        disk[j].type = 2;
                        break;
                    }
                }
                //更新指针
                disk[currentBlockID - 1].nextBlock = bestBlockIDForNextFile;
            }

            newFile.endBlockID = currentBlockID;
            disk[currentBlockID - 1].nextBlock = -1;
            
            return blockNum;
        }

        public void RemoveFolder(FCB targetFolder)
        {
            disk[targetFolder.blockPosID - 1].FCBList.Remove(targetFolder);

            //如果此时该disk内PCBList已空
            //就完全初始化
            if (disk[targetFolder.blockPosID - 1].FCBList.Count() == 0) 
            {
                disk[targetFolder.blockPosID - 1].FCBList = null;
                disk[targetFolder.blockPosID - 1].type = -1;
            }
        }

        public  void RemoverFile(FCB targetFile)
        {

        }

        public string ExtractFileContent(FCB targetFile)
        {
            string wholeContent = "";

            int currentBlockID = targetFile.beginBlockID;

            while(true)
            {
                wholeContent += disk[currentBlockID - 1].data;

                if (disk[currentBlockID - 1].nextBlock == -1) break;
                else currentBlockID = disk[currentBlockID - 1].nextBlock - 1;
            }

            return wholeContent;
        }

    }
}
