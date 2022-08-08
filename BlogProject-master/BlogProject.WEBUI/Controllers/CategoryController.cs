using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogProject.BLL;
using BlogProject.BLL.Results;
using BlogProject.Entities;

namespace BlogProject.WEBUI.Controllers
{
    public class CategoryController : Controller
    {
        #region Core Project
        /*
         Kategorileri ekleme,silme,güncelleme,listleme

        Ben insert delete update alanlarına BLL üzerinden categoryManager nesnesi tarafından Repository classının metotlarını çekmem lazım amöa  categoryManager'a repository'i miras bile alsam metotlar görülemez. Çünkü repository DAL katmanında. Dal katmanı UI tarafından tanınmıyor.

        Bu yüzden ben bir katman daha oluşturucam repository classını içine atıcam bu katmanı UI tanınamasını sağlıcam ve metotları çekeceğim.
         */
        #endregion

        private CategoryManager categoryManager = new CategoryManager();


        
        public ActionResult Index()
        {
            return View(categoryManager.List());
        }
                
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = categoryManager.Find(x=> x.Id==id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
                
        public ActionResult Create()
        {
            return View();
        }

        
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryManager.Insert(category);
               
                return RedirectToAction("Index");
            }

            return View(category);
        }

 
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = categoryManager.Find(x => x.Id == id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                Category cat = categoryManager.Find(x => x.Id == category.Id);
                cat.Title = category.Title;
                cat.Description = category.Description;

                categoryManager.Update(cat);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = categoryManager.Find(x => x.Id == id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Category category = categoryManager.Find(x => x.Id == id);
            categoryManager.Delete(category);
            return RedirectToAction("Index");
        }
      
    }
}
