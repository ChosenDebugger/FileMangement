using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMangement
{
    enum Type
    {
        NotDefined = 0,
        Folder = 1,
        File = 2,
    }

    class FCB
    {
        public Type type;

        public int ID = -1;
        public int size = -1;
        public int blockPosID = -1;
        public string name = null;

        public FCB father = null;

        //type == folder
        public List<FCB> fileSon = null;
        public List<FCB> folderSon = null;

        //type == file
        public int beginBlockID = -999;
        public int endBlockID = -999;

        
        public FCB() { type = Type.NotDefined; }

        public FCB(Type _type, string _name, int _id)
        {
            ID = _id;
            name = _name;
            type = _type;

            if (type == Type.Folder)
            {
                fileSon = new List<FCB>();
                folderSon = new List<FCB>();
            }

            if (type == Type.File)
            {
                beginBlockID = -1;
                endBlockID = -1;
            }
        }

        public FCB(Type _type, string _name, int _size, int _id)
        {
            ID = _id;
            type = _type;
            name = _name;
            size = _size;
            
            if (type == Type.Folder)
            {
                fileSon = new List<FCB>();
                folderSon = new List<FCB>();
            }

            if (type == Type.File)
            {
                beginBlockID = -1;
                endBlockID = -1;
            }
        }
    }
}
