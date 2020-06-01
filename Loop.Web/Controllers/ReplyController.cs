﻿using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Loop.Database;
using Loop.Entities;
using Loop.Services;
using Loop.Web.Models;
using Microsoft.AspNet.Identity;

namespace Loop.Web.Controllers
{
    public class ReplyController : Controller
    {
        private readonly UnitOfWork db = new UnitOfWork(new ApplicationDbContext());

        //GET: Reply/Create
        //Getting from Post Controller Details View the id of post
        //Goal here is to bind post id with the reply
        public ActionResult Create(int id)
        {

            var post = db.Posts.GetAll().ToList().Where(x => x.PostId == id).FirstOrDefault();
            var model = new ReplyViewModel()
            {
                PostId = id,
                CurrentPost = post
            };

            return View(model);
        }

        // POST: Reply/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReplyViewModel model)
        {
            var reply = new Reply()
            {
                Text = model.Text,
                ApplicationUser = db.Users.GetUserById(User.Identity.GetUserId()),
                PostDate = DateTime.Now,
                Post = db.Posts.GetById(model.PostId)
            };

            if (ModelState.IsValid)
            {
                db.Replies.Insert(reply);
                db.Save();
                //Redirection To Post Details View
            }
            return RedirectToAction("Details", "Post", new { id = reply.Post.PostId });
        }

        // GET: Reply/Edit/5
        public ActionResult Edit(int? id)
        {
            Reply reply = db.Replies.GetById(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (reply == null)
            {
                return HttpNotFound();
            }
            var model = CreateModel(reply);


            return View(model);
        }

        // POST: Reply/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReplyViewModel model)
        {

            var reply = CreateReply(model);
            if (ModelState.IsValid)
            {
                db.Replies.Update(reply);
                db.Save();
            }
            return RedirectToAction("Details", "Post", new { id = reply.Post.PostId });
        }

        // GET: Reply/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reply reply = db.Replies.GetById(id);
            if (reply == null)
            {
                return HttpNotFound();
            }
            return View(reply);
        }

        // POST: Reply/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reply reply = db.Replies.GetById(id);
            var post = reply.Post.PostId;
            db.Replies.Remove(reply);
            db.Save();
            return RedirectToAction("Details", "Post", new { id = post });
        }


        [NonAction]
        //Responsible for Reply Creation from model (edit post method)
        //DTO from model to reply and updating values
        private Reply CreateReply(ReplyViewModel model)
        {
            var reply = db.Replies.GetById(model.ReplyId);
            reply.Text = model.Text;
            reply.PostDate = DateTime.Now;
            reply.Post = db.Posts.GetById(model.PostId);
            reply.ApplicationUser = db.Users.GetUserById(User.Identity.GetUserId());

            return reply;
        }

        [NonAction]
        //Reponsible for Model creation after getting id of an reply from GET edit method.
        private ReplyViewModel CreateModel(Reply reply)
        {
            var model = new ReplyViewModel()
            {
                ReplyId = reply.ReplyId,
                Text = reply.Text,
                PostDate = reply.PostDate,
                PostId = reply.Post.PostId
            };

            return model;
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
