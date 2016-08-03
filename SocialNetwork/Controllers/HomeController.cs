using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using SocialNetwork.Models;
using SocialNetwork.Controllers;
using System.Diagnostics;

namespace BootstrapMvcSample.Controllers
{
    public class HomeController : BootstrapBaseController
    {
        private static List<HomeInputModel> _models = ModelIntializer.CreateHomeInputModels();
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            UsersContext db = new UsersContext();
            var username = System.Web.HttpContext.Current.User.Identity.Name;
            var updateslist = Helpers.GetListOfUpdates(username);
            Debug.WriteLine(updateslist.Count());
            try
            {
                Debug.WriteLine(updateslist.FirstOrDefault().data);
            }
            catch
            {
                Debug.WriteLine("The data is empty");
            }
            return View(updateslist);
        }

        [HttpPost]
        public ActionResult Create(string data)
        {
            Updates update = new Updates();

            var username = System.Web.HttpContext.Current.User.Identity.Name;
            var id = Helpers.FetchUserId(username);
            update.data = data;
            update.datecreated = DateTime.Now;
            update.datemodified = DateTime.Now;
            update.status = 1;
            update.userid = id;
            if (ModelState.IsValid)
            {
                Helpers.InsertUpdate(update);
                Success("Status update was successful!");
                return RedirectToAction("Index");
            }
            Error("there were some errors in your form.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult InsertComment(string commentdata, int updateid)
        {
            var username = System.Web.HttpContext.Current.User.Identity.Name;
            var id = Helpers.FetchUserId(username);
            var comment = new Comments();
            comment.data = commentdata;
            comment.datecreated = DateTime.Now;
            comment.datemodified = DateTime.Now;
            comment.status = 1;
            comment.updateid = updateid;
            comment.userid = id;
            Helpers.InsertComment(comment);
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View(new HomeInputModel());
        }

        public ActionResult Delete(int id)
        {
            _models.Remove(_models.Get(id));
            Information("Your widget was deleted");
            if(_models.Count==0)
            {
                Attention("You have deleted all the models! Create a new one to continue the demo.");
            }
            return RedirectToAction("index");
        }
        public ActionResult Edit(int id)
        {
            var model = _models.Get(id);
            return View("Create", model);
        }
        [HttpPost]        
        public ActionResult Edit(HomeInputModel model,int id)
        {
            if(ModelState.IsValid)
            {
                _models.Remove(_models.Get(id));
                model.Id = id;
                _models.Add(model);
                Success("The model was updated!");
                return RedirectToAction("index");
            }
            return View("Create", model);
        }

		public ActionResult Details(int id)
        {
            var model = _models.Get(id);
            return View(model);
        }


        internal ActionResult Admin()
        {
            // used for demonstrationg route filters
            throw new NotImplementedException();
        }
    }
}
