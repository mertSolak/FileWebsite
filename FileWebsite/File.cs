using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileWebsite
{
    public class File : Item
    {
        static int fileCounter;
        public File()
        {
            fileCounter++;
        }
        
        
    }
}