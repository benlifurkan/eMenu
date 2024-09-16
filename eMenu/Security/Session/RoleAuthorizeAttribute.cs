using eMenu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace eMenu.Security.Session
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private int _requiredRoleId;

        public RoleAuthorizeAttribute(int requiredRoleId)
        {
            _requiredRoleId = requiredRoleId;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.Session["user"] as User;

            // Kullanıcının yetkiID'sini kontrol edin
            if (user != null)
            {
                int userRoleId = Convert.ToInt32(user.authorityID);

                // Eğer kullanıcının yetkiID'si gerekli yetkiID'sine eşitse, yetkilendirme başarılı kabul edilir
                if (userRoleId == _requiredRoleId)
                {
                    return true;
                }
            }

            return false; // Yetki ID eşleşmezse, erişim reddedilir
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Yetkisiz erişim durumunda yönlendirilecek sayfayı belirleyebilirsiniz
            filterContext.Result = new RedirectResult("~/Error/Unauthorized");
        }
    }
}