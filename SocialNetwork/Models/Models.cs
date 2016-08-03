using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class Models
    {
    }

    public class Friends
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int friendid { get; set; }
        public int userid { get; set; }
        public int status { get; set; }
        public DateTime createddate { get; set; }
        public DateTime modifieddate { get; set; }
    }

    public class Updates
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public  int  updateid { get; set; }
        public int userid { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime datemodified { get; set; }
        public int status { get; set; }
        public string data { get; set; }

    }

    public class Comments
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int commentid { get; set; }
        public int updateid { get; set; }
        public int userid { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime datemodified { get; set; }
        public int status { get; set; }
        public string data { get; set; }
    }

    public class Photos
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int photoid { get; set; }
        public int userid { get; set; }
        public int status { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime datemodified { get; set; }
    }

    public class FriendRequests
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int requestid { get; set; }
        public int useridfrom { get; set; }
        public int useridto { get; set; }
        public int status { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime dateaccepted { get; set; }
        public DateTime datemodified { get; set; }
    }
    public class Statuses 
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int statusid { get; set; }
        public string details { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime datemodified { get; set; }
    }
    public class ProfilePics
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int picid { get; set; }
        public byte[] pic { get; set; }
        public int userid { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime datemodified { get; set; }
        public int status { get; set; }
    }

    public class PostedPics
    {
        public HttpPostedFileBase File { get; set; }
        public int ImageSize { get; set; }
        public string FileName { get; set; }
        public byte[] ImageData { get; set; }
    }

    public class UpdatesModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int updateid { get; set; }
        public int userid { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime datemodified { get; set; }
        public int status { get; set; }
        public string data { get; set; }
        public string updatename { get; set; }
        public List<CommentsModel> comments { get; set; }

    }

    public class CommentsModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int commentid { get; set; }
        public int updateid { get; set; }
        public int userid { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime datemodified { get; set; }
        public int status { get; set; }
        public string data { get; set; }
        public string commentsname { get; set; }
    }
    //The messages table
    public class Messages
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int messageid { get; set; }
        public int fromuserid { get; set; }
        public int touserid { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime datemodified { get; set; }
        public int status { get; set; }
        public string data { get; set; }
    }

    //The messages model

    public class MessagesModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int messageid { get; set; }
        public int fromuserid { get; set; }
        public int touserid { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime datemodified { get; set; }
        public int status { get; set; }
        public string data { get; set; }
        public string fromname{get;set;}
    }
}