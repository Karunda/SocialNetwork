using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork.Controllers
{
    public class OperationsController : Controller
    {
        //
        // GET: /Operations/

        public ActionResult Profile()
        {
            //Get the logged in username
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var username = System.Web.HttpContext.Current.User.Identity.Name;
            Debug.WriteLine(username);
            UserDetails userdetails = Helpers.GetUserDetails(username);
            //Create the display model
            UserDetailsModel userdetailsmodel = new UserDetailsModel();
            userdetailsmodel.FirstName = userdetails.firstname;
            userdetailsmodel.LastName = userdetails.lastname;
            userdetailsmodel.About = userdetails.about;
            userdetailsmodel.DateOfBirth = userdetails.dateofbirth;
            userdetailsmodel.Gender = userdetails.gender;
            try
            {
                userdetailsmodel.ProfilePic = Helpers.GetUserProfilePic(username).pic;
            }
            catch (Exception ex)
            {
                userdetailsmodel.ProfilePic = null;
            }

            

            if (userdetailsmodel.ProfilePic == null)
            {
                ViewBag.ImagePath = "~/Images/defaultimage.png";

                //Use the default image instead

                using (Image image = Image.FromFile("~/Images/defaultimage.png"))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        // base64String = Convert.ToBase64String(imageBytes);
                        ViewBag.Image = imageBytes;
                    }
                }
            }
            else
            {
                ViewBag.Image = userdetailsmodel.ProfilePic;
            }
            
            return View(userdetailsmodel);
        }
        public ActionResult Notifications()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Upload(UserDetailsModel IG)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            // Apply Validation Here
            //Set the other profile pictures as inactive
            
            //Save the profile image
            if (IG.File.ContentLength > (2 * 1024 * 1024))
            {
                ModelState.AddModelError("CustomError", "File size must be less than 2 MB");
                return RedirectToAction("Profile");
            }
           /* if (!(IG.File.ContentType == "image/jpeg" || IG.File.ContentType == "image/gif"))
            {
                ModelState.AddModelError("CustomError", "File type allowed : jpeg and gif");
                return RedirectToAction("Profile");
            }*/
            Helpers.UpdateProfilePicStatus(System.Web.HttpContext.Current.User.Identity.Name);
            byte[] data = new byte[IG.File.ContentLength];
            IG.File.InputStream.Read(data, 0, IG.File.ContentLength);
            ProfilePics details = new ProfilePics();
            int id = Helpers.FetchUserId(System.Web.HttpContext.Current.User.Identity.Name);
            details.userid = id;
            details.pic = data;
            details.datecreated = DateTime.Now;
            details.datemodified = DateTime.Now;
            details.status = 1;//Status 1 means the profile picture is the active 1.
            using (UsersContext dc = new UsersContext())
            {

                dc.ProfilePics.Add(details);
                dc.SaveChanges();
            }
            return RedirectToAction("Profile");
        }

        public ActionResult NewsFeed()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        //This action is used to search friends and list them
        [HttpPost]
        public ActionResult SearchFriends(string searchstring)
        {
            Debug.WriteLine(searchstring);
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var lstuserdetails = Helpers.SearchFriends(searchstring);
            return View(lstuserdetails);
        }

        [HttpGet]
        public ActionResult InsertFriendRequest(int id)
        {
            Debug.WriteLine(id);
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            int toid = Helpers.FetchUserId(System.Web.HttpContext.Current.User.Identity.Name);
            Helpers.InsertFriendRequest(id, toid);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult GetMessages()
        {
            
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            string username = System.Web.HttpContext.Current.User.Identity.Name;
            var messages=Helpers.ReadMessages(username);
            return View(messages);
        }

        [HttpGet]
        public ActionResult InsertMessages()
        {
            var users = Helpers.GetUsers();
            
            return View(users);
        }

        [HttpGet]
        public ActionResult InsertMessageToDB(int toid, string data)
        {

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            int fromid = Helpers.FetchUserId(System.Web.HttpContext.Current.User.Identity.Name);
            var message = new Messages();
            message.data = data;
            message.datecreated = DateTime.Now;
            message.datemodified = DateTime.Now;
            message.fromuserid = fromid;
            message.touserid = toid;
            Helpers.InsertMessage(message);
            return RedirectToAction("Index", "Home");
        }
    }
}
