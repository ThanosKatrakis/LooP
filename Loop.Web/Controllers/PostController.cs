﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Loop.Database;
using Loop.Entities;
using Loop.Services;
using Microsoft.AspNet.Identity;
using PagedList;

namespace Loop.Web.Controllers
{
    public class PostController : Controller
    {
        private UnitOfWork db = new UnitOfWork(new ApplicationDbContext());

        // GET: Post
        public ActionResult Index(string sortOrder, string searchTitle, int? page, int? pSize)
        {
            ViewBag.TitleSortParam = string.IsNullOrEmpty(sortOrder) ? "TitleDesc" : "";
            ViewBag.RepliesSortParam = string.IsNullOrEmpty(sortOrder) ? "RepliesDesc" : "";
            ViewBag.CurrentpSize = pSize;

            var posts = db.Posts.GetAll();

            //FILTERING
            if (!string.IsNullOrWhiteSpace(searchTitle))
            {
                posts = posts.Where(x => x.Title.ToUpper().Contains(searchTitle.ToUpper()));
            }

            switch (sortOrder)
            {
                case "TitleDesc": posts = posts.OrderByDescending(x => x.Title); break;
                case "RepliesAsc": posts = posts.OrderByDescending(x => x.Replies.Count()); break;
                case "RepliesDesc": posts = posts.OrderByDescending(x => x.Replies.Count()); break;

                default: posts = posts.OrderBy(x => x.Title); break;
            }
            int pageSize = pSize ?? 9;
            int pageNumber = page ?? 1;

            return View(posts.ToPagedList(pageNumber,pageSize));
        }

        // GET: Post/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.GetById(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Post/Create
        public ActionResult Create()
        {
            //Get UserId and store it to ViewBag
            var userId = User.Identity.GetUserId();
            ViewBag.ApplicationUserId = userId;

            //here we make a Viewbag for Tags
            ViewBag.SelectedTagsIds = db.Tags.GetAll().Select(x => new SelectListItem()
            {
                Value = x.TagId.ToString(),
                Text = x.Title
            });

            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId,Title,Text,DateTime,ApplicationUserId")] Post post, IEnumerable<int> SelectedTagsIds)
        {
            if (ModelState.IsValid)
            {
                post.ApplicationUserId = User.Identity.GetUserId();
                db.Posts.Insert(post, SelectedTagsIds);
                db.Save();
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Post/Edit/5
        public ActionResult Edit(int? id)
        {
            var userId = User.Identity.GetUserId();
            ViewBag.ApplicationUserId = userId;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.GetById(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            ViewBag.SelectedTagsIds = db.Tags.GetAll().Select(x => new SelectListItem()
            {
                Value = x.TagId.ToString(),
                Text = x.Title
            });
            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId,Title,Text,DateTime,ApplicationUserId")] Post post, IEnumerable<int> SelectedTagsIds)
        {
            if (ModelState.IsValid)
            {
                post.ApplicationUserId = User.Identity.GetUserId();
                db.Posts.Update(post, SelectedTagsIds);
                db.Save();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Post/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.GetById(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.GetById(id);
            db.Posts.Remove(post);
            db.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}