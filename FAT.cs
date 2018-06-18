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
        }
    }

    class FAT
    {
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
            bestBlockIDForNextFile = 2;
        }

        public void AddNewFolder(FCB newFolder)
        {
            int posID = bestBlockIDForNextFCB;

            newFolder.blockPosition = posID;
            
            if(disk[posID-1].FCBList==null)             //FCBList还未初始化的情况
                disk[posID - 1].FCBList = new List<FCB>();

            disk[posID - 1].FCBList.Add(newFolder);     //更新disk

            bitmap[posID - 1] = true;                   //更新位图

            if (disk[posID - 1].FCBList.Count() == 4)
            {
                //该块已满
                //寻找下一个放置FCB的磁盘块
                for (int i = 0; i < disk.Count(); i++)
                {
                    //这里要满足FCBList不满而且该block没有且不会再下一步被用作file_data
                    if (disk[i].FCBList.Count() != 4 && bitmap[i]==false && i != bestBlockIDForNextFile - 1)
                        bestBlockIDForNextFCB = i + 1;
                }
            }
        }

        public void AddNewFile(FCB newFile)
        {

        }


    }
}
