using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FileWebsite.Models;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Entity.Validation;
using FileWebsite.Security;

namespace FileWebsite.Controllers
{

    [CustomAuthorization]
    public class HomeController : Controller
    {
        ManageItemController itemController = new ManageItemController();

        public ActionResult Index(int directoryId = -1)
        {
            var model = new HomeViewModel();
            model.parentId = directoryId;
            if (directoryId != -1)
            {
                model.parentFolders = new List<Item>();
                model.parentFolders.AddRange(itemController.GetItemsFromId(directoryId));
            }             
            LoadItems(model);
            return View(model);
        }

        [HttpPost]
        public HttpPostedFileBase SaveDropzoneJsUploadedFiles(int id)
        {
            var model = new HomeViewModel();
            model.parentId = id;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        model.size = file.ContentLength.ToString();
                        if (itemController.WriteItemToDatabase(model, file.FileName))
                        {
                            itemController.WriteFileToContainer(model, file);
                            return file;
                        }
                    }
                    return null;

                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void LoadItems(HomeViewModel model)
        {
            model.AllItems = itemController.ReadItemsFromDatabase(model);
            if (model.AllItems != null)
            {
                model.Files = model.AllItems.Where(m => m.Type == 1 && m.Status == 1).ToList();
                model.Folders = model.AllItems.Where(m => m.Type == 0 && m.Status == 1).ToList();
            }
        }
    }
}