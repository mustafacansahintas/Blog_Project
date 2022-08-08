using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogProject.BLL;
using BlogProject.Entities;
using BlogProject.WEBUI.Models;

namespace BlogProject.WEBUI.Controllers
{
    public class BlogController : Controller
    {
        private BlogManager blogManager = new BlogManager();
        private CategoryManager categoryManager = new CategoryManager();
        private LikedManager likedManager = new LikedManager();

        // GET: Blog
        public ActionResult Index()
        {
            var blogs = blogManager.ListQueryable().Include("Category").Include("Owner").Where(
                x => x.Owner.Id == CurrentSession.User.Id).OrderByDescending(
                x => x.ModifiedOn);

            return View(blogs.ToList());
        }

        public ActionResult MyBlogs()
        {
            var blogs = likedManager.ListQueryable().Include("LikedUser").Include("Blog").Where(

                x => x.LikedUser.Id == CurrentSession.User.Id).Select(
                x => x.Blog).Include("Category").Include("Owner").OrderByDescending(
                x => x.ModifiedOn);

            return View("Index", blogs.ToList());
        }

        // GET: Blog/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = blogManager.Find(x => x.Id == id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // GET: Blog/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(categoryManager.List(), "Id", "Title");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.Owner = CurrentSession.User;
                blogManager.Insert(blog);
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(categoryManager.List(), "Id", "Title", blog.CategoryId);
            return View(blog);
        }


      

        // GET: Blog/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = blogManager.Find(x => x.Id == id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(categoryManager.List(), "Id", "Title", blog.CategoryId);
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Blog blog)
        {
            if (ModelState.IsValid)
            {
                Blog db_blog = blogManager.Find(x => x.Id == blog.Id);
                db_blog.IsDraft = blog.IsDraft;
                db_blog.CategoryId = blog.CategoryId;
                db_blog.Text = blog.Text;
                db_blog.Title = blog.Title;

                blogManager.Update(db_blog);
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(categoryManager.List(), "Id", "Title", blog.CategoryId);
            return View(blog);
        }

        // GET: Blog/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = blogManager.Find(x => x.Id == id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blog blog = blogManager.Find(x => x.Id == id);
            blogManager.Delete(blog);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            List<int> likedBlogIds = likedManager.List(
                x => x.LikedUser.Id == CurrentSession.User.Id && ids.Contains(x.Blog.Id)).Select(
                x => x.Blog.Id).ToList();

            return Json(new { result = likedBlogIds });
        }
    }
}
