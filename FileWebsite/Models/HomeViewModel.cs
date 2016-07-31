using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileWebsite.Models
{
    public class HomeViewModel
    {       
        public List<Item> AllItems { get; set; }
        public List<Item> Folders { get; set; }
        public List<Item> Files { get; set; }
        public string directory { get; set; }
        public int parentId { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string created { get; set; }
        public string size { get; set; }
        public string folderName { get; set; }
        public string iconName { get; set; }
        public string newName { get; set; }
        public List<Item> parentFolders { get; set; }
        public HomeViewModel(string defdir) {
            directory = defdir;
        }
        public HomeViewModel() { }
    }
}