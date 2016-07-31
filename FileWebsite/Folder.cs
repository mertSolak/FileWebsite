using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileWebsite
{
    public class Folder : Item
    {
        public static int folderCounter;
        public Folder()
        {
            folderCounter++;
        }
        public List<Folder> folders = new List<Folder>();
        public List<File> files = new List<File>();

        public Folder(int id, Item item)
        {
            Name = item.Name;
            Id = item.Id;
            ParentId = item.ParentId;
            Size = item.Size;
            Status = item.Status;
            Time = item.Time;
            SubItems = item.SubItems;
            ParentItems = item.ParentItems;
            Extension = item.Extension;
            UserId = item.UserId;
            Type = item.Type;
            User = item.User;
        }


        public void AddItem(List<Item> items)
        {
            if (items == null)
                return;

            foreach (var item in items)
            {
                Item i;
                if (item.Type == 0)
                {
                    i = new Folder();
                }
                else {
                    i = new File();
                }

                i.Name = item.Name;
                i.Id = item.Id;
                i.ParentId = item.ParentId;
                i.Size = item.Size;
                i.Status = item.Status;
                i.Time = item.Time;
                i.SubItems = item.SubItems;
                i.ParentItems = item.ParentItems;
                i.Extension = item.Extension;
                i.UserId = item.UserId;
                i.Type = item.Type;
                i.User = item.User;

                if (item.Type == 0)
                {
                    folders.Add((Folder)i);
                }
                else
                {
                    files.Add((File)i);
                }
            }
        }
    }
}