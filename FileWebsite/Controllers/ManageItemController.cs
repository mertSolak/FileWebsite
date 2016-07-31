using FileWebsite.Models;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FileWebsite.Controllers
{
    public class ManageItemController : Controller
    {
        const string fileSystemBase = "D:\\Container\\";

        public string GetExtension(string name)
        {
            return Path.GetExtension(name.Replace(".lnk", "")).Trim();
        }

        public string GetName(string name)
        {
            return Path.GetFileNameWithoutExtension(name.Replace(".lnk", "")).Trim();
        }

        public bool RemoveItem(HomeViewModel model)
        {
            model.id = Convert.ToInt32(GetIdOfElement(model));
            return RemoveItemFromDatabase(model.id);
        }

        public bool RemoveItemFromDatabase(int directoryId)
        {
            try
            {
                using (var mydb = new Entity())
                {
                    Item item = mydb.Items.FirstOrDefault(m => m.Id == directoryId && m.Status == 1);
                    item.Status = 0;
                    if (item.Type == 1)
                    {
                        mydb.SaveChanges();
                        return true;
                    }
                    else
                    {
                        Folder folder = FindAllFilesUnderThatFolder(directoryId, mydb);

                        if (folder.folders.Count != 0)
                            for (int i = 0; i < folder.folders.Count; i++)
                            {
                                RemoveItemFromDatabase(folder.folders[i].Id);
                            }

                        if (folder.files.Count != 0)
                            for (int i = 0; i < folder.files.Count; i++)
                            {
                                RemoveItemFromDatabase(folder.files[i].Id);
                            }
                        mydb.SaveChanges();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public string RenameItem(HomeViewModel model)
        {
            try
            {
                using (var mydb = new Entity())
                {
                    model.id = Convert.ToInt32(GetIdOfElement(model));
                    Item item = mydb.Items.Find(model.id);
                    item.Name = model.newName.Trim();
                    mydb.SaveChanges();
                    return item.Name + item.Extension;
                }
            }
            catch
            {
                return "undefined";
            }
        }

        public FileResult Download(int directoryId = -1)
        {
            string fileName, path;
            using (var db = new Entity())
            {
                Item item = db.Items.Find(directoryId);
                fileName = item.Name.Trim() + item.Extension.Trim();
                if (item.Type == 1)
                {
                    path = PathGenerator(directoryId);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {
                    var accFiles = new Dictionary<int, string>();
                    var folder = new Folder();
                    folder = FindAllFilesUnderThatFolder(directoryId, db);
                    return Zip(fileName, findSourceList(folder), findDestList(folder, db), FindFolderDirectories(folder, db));
                }
            }
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public string CreateFileDirectory(File file, Entity db)
        {
            var path = file.Name.Trim() + file.Extension.Trim();
            var tempItem = db.Items.Find(file.Id);
            var flag = true;
            while (flag)
            {
                tempItem = db.Items.Find(tempItem.ParentId);
                if (tempItem.ParentId == null)
                    flag = false;
                path = Path.Combine(tempItem.Name.Trim(), path);
            }
            return path;
        }

        public string CreateFolderDirectory(Folder folder, Entity db)
        {
            var path = folder.Name.Trim();
            var tempItem = db.Items.Find(folder.Id);
            var flag = true;
            while (flag)
            {
                tempItem = db.Items.Find(tempItem.ParentId);
                if (tempItem.ParentId == null)
                    flag = false;
                path = Path.Combine(tempItem.Name.Trim(), path);
            }
            return path;
        }

        public List<Item> GetItemsFromId(int id)
        {
            using (var db = new Entity())
            {
                var tempItem = new List<Item>();
                tempItem.Add(db.Items.Find(id));
                var flag = true;
                int counter = 0;
                while (flag)
                {
                    if (tempItem[counter].ParentId == null)
                        break;
                    tempItem.Add(db.Items.Find(tempItem[counter].ParentId));
                    counter++;
                }
                return tempItem;
            }
        }

        public string GetIdOfElement(HomeViewModel model)
        {
            model.name = GetName(model.name.Trim());
            using (var db = new Entity())
            {
                if (model.parentId == -1)
                    return db.Items.FirstOrDefault(m => m.Name == model.name && m.ParentId == null).Id.ToString();
                return db.Items.FirstOrDefault(m => m.Name == model.name && m.ParentId == model.parentId).Id.ToString();
            }
        }

        public FileResult Zip(string zipName, List<string> source, List<string> dest, List<string> dir)
        {
            Response.Clear();
            Response.BufferOutput = false;
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "filename=" + zipName + ".zip");
            using (ZipFile zip = new ZipFile())
            {
                foreach (string directory in dir)
                {
                    zip.AddDirectoryByName(directory);
                }
                if (source.Count == dest.Count)
                    for (int i = 0; i < source.Count; i++)
                    {
                        zip.AddFile(source[i]).FileName = dest[i];
                    }
                else
                    return null;

                zip.Save(Response.OutputStream);

                return File(Response.OutputStream, System.Net.Mime.MediaTypeNames.Application.Octet, zipName + ".zip");
            }

        }

        public List<string> findDestList(Folder folder, Entity db)
        {
            var dest = new List<string>();
            if (folder.files.Count != 0)
                foreach (File singleFile in folder.files)
                {
                    dest.Add(CreateFileDirectory(singleFile, db));
                }
            if (folder.folders.Count != 0)
                foreach (Folder singleFolder in folder.folders)
                {
                    //dest.Add(CreateFolderDirectory(singleFolder, db));
                    dest.AddRange(findDestList(singleFolder, db));
                }
            return dest;
        }

        public List<string> findSourceList(Folder folder)
        {
            var source = new List<string>();
            if (folder.files != null)
                foreach (File singleFile in folder.files)
                {
                    source.Add(PathGenerator(singleFile.Id));

                }
            if (folder.folders != null)
                foreach (Folder singleFolder in folder.folders)
                {
                    //source.Add(PathGenerator(singleFolder.Id));
                    source.AddRange(findSourceList(singleFolder));
                }
            return source;
        }

        public List<string> FindFolderDirectories(Folder folder, Entity db)
        {
            var dir = new List<string>();
            if (folder.folders != null)
                foreach (Folder singleFolder in folder.folders)
                {
                    dir.Add(CreateFolderDirectory(singleFolder, db));
                    dir.AddRange(FindFolderDirectories(singleFolder, db));
                }
            return dir;
        }

        [HttpPost]
        public string OrganizeSize(HomeViewModel model)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = Convert.ToDouble(model.size);
            int order = 0;
            while (len >= 1024 && ++order < sizes.Length)
            {
                len = len / 1024;
            }
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }

        public Folder FindAllFilesUnderThatFolder(int id, Entity database)
        {
            var folder = new Folder(id, database.Items.Find(id));

            if (database.Items.Any(m => m.ParentId == id))
                folder.AddItem(database.Items.Where(m => m.ParentId == id && m.Status == 1).ToList());
            else
                return folder;

            if (folder.folders.Count == 0)
            {
                return folder;
            }
            else
            {
                for (int i = 0; i < folder.folders.Count; i++)
                {
                    folder.folders[i] = (FindAllFilesUnderThatFolder(folder.folders[i].Id, database));
                }
            }
            return folder;
        }

        public double ConvertToByte(string size)
        {
            double doubleSize;
            if (size.Contains("KB"))
                doubleSize = (Convert.ToDouble(size.Replace("KB", "").Trim()) * (double)1024);
            else if (size.Contains("MB"))
                doubleSize = (Convert.ToDouble(size.Replace("MB", "").Trim()) * (double)(1024 * 1024));
            else if (size.Contains("GB"))
                doubleSize = (Convert.ToDouble(size.Replace("GB", "").Trim()) * (double)(1024 * 1024 * 1024));
            else
                return 0;
            return doubleSize;
        }

        public void OrganizeFolderSize(int Id, Entity db)
        {
            var itemToUpdate = db.Items.Find(Id);
            double size = ConvertToByte(itemToUpdate.Size);

            while (itemToUpdate.ParentId != null)
            {
                itemToUpdate = db.Items.Find(itemToUpdate.ParentId);
                var model = new HomeViewModel();
                var anothersize = ConvertToByte(itemToUpdate.Size);
                model.size = (anothersize + size).ToString();
                itemToUpdate.Size = OrganizeSize(model);
                db.SaveChanges();
            }
        }

        public bool WriteItemToDatabase(HomeViewModel model, string name)
        {
            try
            {
                using (var mydb = new Entity())
                {
                    int userId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
                    var itemToAdd = new Item();
                    itemToAdd.Name = GetName(name);
                    if (model.parentId != -1)
                        itemToAdd.ParentId = model.parentId;
                    itemToAdd.Status = 1;
                    itemToAdd.Extension = GetExtension(name);
                    itemToAdd.UserId = userId;
                    itemToAdd.Size = OrganizeSize(model);
                    itemToAdd.Time = DateTime.Today.ToString("dd/MM/yyyy").Trim();
                    itemToAdd.Type = 1;
                    bool isExist = mydb.Items.Any(m => m.Name == itemToAdd.Name && m.ParentId == itemToAdd.ParentId && m.Status == 0);
                    if (isExist)
                    {
                        var existItem = new Item();
                        existItem = mydb.Items.FirstOrDefault(m => m.Name == itemToAdd.Name && m.ParentId == itemToAdd.ParentId && m.Status == 0);
                        existItem.Status = 1;
                        mydb.SaveChanges();
                        return true;
                    }
                    isExist = mydb.Items.Any(m => m.Name == itemToAdd.Name && m.ParentId == itemToAdd.ParentId);
                    if (isExist)
                        return false;
                    mydb.Items.Add(itemToAdd);
                    mydb.SaveChanges();
                    model.id = mydb.Items.FirstOrDefault(m => m.Name == itemToAdd.Name && m.ParentId == itemToAdd.ParentId && m.Status == 1).Id;
                    OrganizeFolderSize(model.id, mydb);
                    return true;
                }
            }

            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

        }

        public int WriteFolderToDatabase(HomeViewModel model)
        {
            try
            {
                using (var mydb = new Entity())
                {
                    int userId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
                    Item dbItem = new Item();
                    dbItem.Name = model.folderName.Trim();
                    if (model.parentId != -1)
                        dbItem.ParentId = model.parentId;
                    dbItem.Status = 1;
                    dbItem.Extension = Path.GetExtension(model.folderName).Trim();
                    dbItem.UserId = userId;
                    dbItem.Size = "0 B";
                    dbItem.Time = DateTime.Today.ToString("dd/MM/yyyy").Trim();
                    dbItem.Type = 0;
                    bool isExist = mydb.Items.Any(m => m.Name == dbItem.Name && m.ParentId == dbItem.ParentId && m.Status == 0);
                    if (isExist)
                    {
                        var existItem = new Item();
                        existItem = mydb.Items.FirstOrDefault(m => m.Name == dbItem.Name && m.ParentId == dbItem.ParentId && m.Status == 0);
                        existItem.Status = 1;
                        mydb.SaveChanges();
                        return existItem.Id;
                    }
                    isExist = mydb.Items.Any(m => m.Name == dbItem.Name && m.ParentId == dbItem.ParentId);
                    if (isExist)
                        return -1;
                    mydb.Items.Add(dbItem);
                    mydb.SaveChanges();
                    model.id = mydb.Items.FirstOrDefault(m => m.Name == dbItem.Name && m.ParentId == dbItem.ParentId && m.Status == 1).Id;
                    return model.id;
                }
            }

            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);

                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

        }

        public string PathGenerator(int id)
        {
            string pathString = fileSystemBase;

            pathString = Path.Combine(pathString, id.ToString());

            return pathString;
        }

        public string PathGenerator(string name)
        {
            string pathString = Path.Combine(fileSystemBase, name);

            return pathString;
        }

        public string FilePathGenerator(HomeViewModel model, string fileName)
        {
            string pathString = fileSystemBase + System.Web.HttpContext.Current.Session["username"];

            if (model.parentId != -1)
                pathString = Path.Combine(pathString, model.parentId.ToString());

            pathString = System.IO.Path.Combine(pathString, fileName);

            return pathString;
        }

        public List<Item> ReadItemsFromDatabase(HomeViewModel model)
        {
            try
            {
                using (var db = new Entity())
                {
                    int userId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());

                    if (model.parentId == -1)
                        return db.Items.Where(m => m.ParentId == null && m.UserId == userId && m.Status == 1).ToList();
                    return db.Items.Where(m => m.ParentId == model.parentId && m.UserId == userId && m.Status == 1).ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public bool WriteFileToContainer(HomeViewModel model, HttpPostedFileBase file)
        {
            try
            {
                string pathString = PathGenerator(model.id);

                bool isExists = System.IO.Directory.Exists(pathString);

                if (!isExists)
                    System.IO.Directory.CreateDirectory(Path.GetDirectoryName(pathString));

                file.SaveAs(pathString);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}