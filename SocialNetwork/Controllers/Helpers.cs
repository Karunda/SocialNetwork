using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace SocialNetwork.Controllers
{
    public class Helpers
    {
        //This class is used to run various utilities, e.g getting userdetails
        //Fetches the user id, using username
        public static int FetchUserId(string username)
        {
            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == username.ToLower());
                // Check if user already exists
                return user.UserId;
            }

        }
        //Creates the user details 
        public static bool CreateUserDetails(UserDetails details)
        {

            using (UsersContext db = new UsersContext())
            {
                var userdetails = db.Set<UserDetails>();
                userdetails.Add(details);
                db.SaveChanges();
                return true;
            }
        }

        public static UserDetails GetUserDetails(string username)
        {
            UserDetails details = new UserDetails();
            int id = FetchUserId(username);
            using (UsersContext db = new UsersContext())
            {
                UserDetails user = db.UserDetails.FirstOrDefault(u => u.UserId == id);
                // Check if user already exists
                return user;
            }
        }

        public static UserDetails GetUserDetailsId(int id)
        {
            UserDetails details = new UserDetails();            
            using (UsersContext db = new UsersContext())
            {
                UserDetails user = db.UserDetails.FirstOrDefault(u => u.UserId == id);
                // Check if user already exists
                return user;
            }
        }

        public static ProfilePics GetUserProfilePic(string username)
        {
            ProfilePics pp = new ProfilePics();
            int id = FetchUserId(username);
            using (UsersContext db = new UsersContext())
            {
                ProfilePics profilepic = db.ProfilePics.FirstOrDefault(u => u.userid == id && u.status == 1);
                // Check if user already exists

                if (profilepic == null)
                {
                    string path = HostingEnvironment.MapPath(@"~/");
                    Debug.WriteLine(path);
                    string filename = path + "Images\\defaultimage.png";
                    Debug.WriteLine(filename);

                    using (Image image = Image.FromFile(filename))
                    {
                        profilepic = new ProfilePics();
                        profilepic.userid = id;
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();

                            // Convert byte[] to Base64 String
                            // base64String = Convert.ToBase64String(imageBytes);
                            profilepic.pic = imageBytes;
                        }
                    }
                }
                return profilepic;
            }
        }

        public static bool UpdateProfilePicStatus(string username)
        {
            int id = FetchUserId(username);
            using (var db = new UsersContext())
            {
                var friends = db.ProfilePics.Where(f => id == f.userid).ToList();
                friends.ForEach(a => a.status = 0);
                db.SaveChanges();
                return true;
            }
        }

        public static List<UpdatesModel> GetListOfUpdates(string username)
        {
            int id = 0;
            //fetch the userid. Handle the exception when the username is empty
            try
            {
               id= FetchUserId(username);
            }
            catch
            {

            }
            List<UpdatesModel> lstupdates = new List<UpdatesModel>();
            using (var db = new UsersContext())
            {
                var updates = db.Updates.OrderByDescending(f => f.datecreated).ToList();
                if (String.IsNullOrEmpty(username))
                {
                    db.Updates.OrderByDescending(f => f.datecreated).ToList();
                }
                db.Updates.OrderByDescending(f => f.datecreated).ToList();
                foreach (Updates up in updates)
                {
                    var upmodel = new UpdatesModel();
                    upmodel.data = up.data;
                    upmodel.datecreated = up.datecreated;
                    upmodel.datemodified = up.datemodified;
                    upmodel.status = up.status;
                    upmodel.updateid = up.updateid;
                    upmodel.userid = up.userid;
                    var updateuser = GetUserDetailsId(up.userid);
                    upmodel.updatename = updateuser.firstname + " " + updateuser.lastname;
                    upmodel.comments = new List<CommentsModel>();
                    var comments = db.Comments.Where(f => up.updateid == f.updateid).OrderByDescending(f => f.datecreated).ToList();
                    foreach (Comments cm in comments)
                    {
                        var cmmodel = new CommentsModel();
                        cmmodel.data = cm.data;
                        cmmodel.commentid = cm.commentid;
                        var commentuser = GetUserDetailsId(cm.userid);
                        cmmodel.commentsname = commentuser.firstname + " " + commentuser.lastname;
                        cmmodel.datecreated = cm.datecreated;
                        cmmodel.datemodified = cm.datemodified;
                        cmmodel.status = cm.status;
                        cmmodel.updateid = cm.updateid;
                        cmmodel.userid = cm.userid;
                        upmodel.comments.Add(cmmodel);
                    }
                    lstupdates.Add(upmodel);
                }
                return lstupdates;
            }
        }

        public static bool InsertUpdate(Updates update)
        {

            using (UsersContext db = new UsersContext())
            {
                var userupdates = db.Set<Updates>();
                userupdates.Add(update);
                db.SaveChanges();
                return true;
            }
        }
        //The function is used to search friends
        public static List<UserDetailsModel> SearchFriends(string searchstring)
        {

            List<UserDetailsModel> lstuserdetails = new List<UserDetailsModel>();
            using (var db = new UsersContext())
            {
                var userdetails = db.UserDetails.Where(f => f.firstname.ToLower()
                    .Contains(searchstring.ToLower()) ||
                    f.lastname.ToLower()
                    .Contains(searchstring.ToLower())).OrderBy(f => f.firstname).ToList();
                foreach (UserDetails up in userdetails)
                {
                    UserDetailsModel userdetailsmodel = new UserDetailsModel();
                    userdetailsmodel.UserId = up.UserId;
                    userdetailsmodel.FirstName = up.firstname;
                    userdetailsmodel.LastName = up.lastname;
                    userdetailsmodel.About = up.about;
                    userdetailsmodel.DateOfBirth = up.dateofbirth;
                    userdetailsmodel.Gender = up.gender;
                    userdetailsmodel.ProfilePic = Helpers.GetUserProfilePic(GetUserName(up.UserId)).pic;
                    lstuserdetails.Add(userdetailsmodel);
                }
                return lstuserdetails;
            }
        }


        //The function is used to get the username, using the userid
        public static string GetUserName(int id)
        {

            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserId == id);
                // Check if user already exists
                return user.UserName;
            }
        }

        //The function is used save friend requests
        public static bool InsertFriendRequest(int fromid, int toid)
        {
            var friendrequest = new FriendRequests();
            friendrequest.dateaccepted = DateTime.Now;
            friendrequest.datecreated = DateTime.Now;
            friendrequest.datemodified = DateTime.Now;
            friendrequest.status = 0;
            friendrequest.useridfrom = fromid;
            friendrequest.useridto = toid;

            using (UsersContext db = new UsersContext())
            {
                var friendrequests = db.Set<FriendRequests>();
                friendrequests.Add(friendrequest);
                db.SaveChanges();
                return true;
            }
        }
        //Used to insert comments
        public static bool InsertComment(Comments comment)
        {

            using (UsersContext db = new UsersContext())
            {
                var comments = db.Set<Comments>();
                comments.Add(comment);
                db.SaveChanges();
                return true;
            }
        }


        public static bool InsertMessage(Messages message)
        {

            using (UsersContext db = new UsersContext())
            {
                var messages = db.Set<Messages>();
                messages.Add(message);
                db.SaveChanges();
                return true;
            }
        }
        //Get list of all users
        public static List<UserDetailsModel> GetUsers()
        {

            List<UserDetailsModel> lstuserdetails = new List<UserDetailsModel>();
            using (var db = new UsersContext())
            {
                var userdetails = db.UserDetails.OrderBy(f => f.firstname).ToList();
                foreach (UserDetails up in userdetails)
                {
                    UserDetailsModel userdetailsmodel = new UserDetailsModel();
                    userdetailsmodel.UserId = up.UserId;
                    userdetailsmodel.FirstName = up.firstname;
                    userdetailsmodel.LastName = up.lastname;
                    userdetailsmodel.About = up.about;
                    userdetailsmodel.DateOfBirth = up.dateofbirth;
                    userdetailsmodel.Gender = up.gender;
                    userdetailsmodel.ProfilePic = Helpers.GetUserProfilePic(GetUserName(up.UserId)).pic;
                    lstuserdetails.Add(userdetailsmodel);
                }
                return lstuserdetails;
            }
        }
        public static List<MessagesModel> ReadMessages(string username)
        {
            List<MessagesModel> messagesModel = new List<MessagesModel>();
            UserDetails details = new UserDetails();
            int id = FetchUserId(username);
            using (UsersContext db = new UsersContext())
            {
                var ms = db.Messages.Where(u => u.touserid == id).ToList();
                foreach (var m in ms)
                {
                    MessagesModel mm = new MessagesModel();
                    mm.data = m.data;
                    mm.datecreated = m.datecreated;
                    mm.datemodified = m.datemodified;
                    mm.fromuserid = m.fromuserid;
                    var messageuser = GetUserDetailsId(m.fromuserid);
                    mm.fromname = messageuser.firstname + " " + messageuser.lastname;
                    //mm.fromname = "";
                    mm.messageid = m.messageid;
                    mm.status = m.status;
                    mm.touserid = m.touserid;
                    messagesModel.Add(mm);
                }
                // Check if user already exists
                return messagesModel;
            }
        }

    }
}