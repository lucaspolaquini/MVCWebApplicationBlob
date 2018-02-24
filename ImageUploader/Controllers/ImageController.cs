using ImageUploader.Services;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ImageUploader.Controllers
{
    public class ImageController : Controller
    {
        // GET: Image
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(HttpPostedFileBase image)
        {
            try
            {
                ImageService imageService = new ImageService();
                var uploadedImage = await imageService.CreateUploadedImage(image);
                await imageService.AddImageToBlobStorageAsync(uploadedImage);

                TempData.Add("Sucesso", "Sucesso");                
            }
            catch (Exception)
            {
                TempData.Add("Erro", "Erro");
            }
            return RedirectToAction("Index");
        }

        // GET: Image
        [HttpGet]
        public ActionResult ListImages()
        {
            ImageService imageService = new ImageService();
            var images = imageService.GetAllImages();

            return View(images);
        }
    }
}