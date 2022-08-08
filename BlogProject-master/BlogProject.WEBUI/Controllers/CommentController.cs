﻿using BlogProject.BLL;
using BlogProject.Entities;
using BlogProject.WEBUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BlogProject.WEBUI.Controllers
{
    public class CommentController : Controller
    {
        private BlogManager blogManager = new BlogManager();
        private CommentManager commentManager = new CommentManager();
        // GET: Comment
        public ActionResult ShowBlogComments(int? id)
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Blog blog = blogManager.ListQueryable().Include("Comments").FirstOrDefault(x => x.Id == id);

            if(blog == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialComments",blog.Comments);
        }

        public ActionResult Create(Comment comment, int? blogid)
        {
            if (ModelState.IsValid)
            {
                if (blogid == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Blog blog = blogManager.Find(x => x.Id == blogid);

                if (blog == null)
                {
                    return HttpNotFound();
                }
                comment.Blog = blog;
                comment.Owner = CurrentSession.User;

                if (commentManager.Insert(comment) > 0)
                {
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }               
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int? id,string text)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = commentManager.Find(x => x.Id == id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            comment.Text = text;

            if (commentManager.Update(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = commentManager.Find(x => x.Id == id);

            if (comment == null)
            {
                return HttpNotFound();
            }                       

            if (commentManager.Delete(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);


        }
    }
}