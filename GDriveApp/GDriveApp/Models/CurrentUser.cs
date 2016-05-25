using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GDriveApi.Model;

namespace GDriveApp.Models
{
    public static class CurrentUser
    {
        #region Constants

        public static readonly string CURRENT_USER_KEY = "CurrentUser";

        #endregion

        public static GoogleProfile Info
        {
            get
            {
                return (GoogleProfile)HttpContext.Current.Session[CURRENT_USER_KEY];
            }
            set
            {
                HttpContext.Current.Session[CURRENT_USER_KEY] = value;
            }
        }

        public static bool IsLoggedIn
        {
            get
            {
                var result = HttpContext.Current.Session[CURRENT_USER_KEY] != null;

                return result;
            }
        }
    }
}