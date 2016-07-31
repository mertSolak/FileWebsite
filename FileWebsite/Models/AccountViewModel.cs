using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FileWebsite.Models
{
    public class LoginViewModel
    {       
        public string username { set; get; }
        public string password { set; get; }
    }
    

    public class RegisterViewModel
    {

        public string username { set; get; }
        public string password { set; get; }
        public string confirmPassword { set; get; }
    }
}