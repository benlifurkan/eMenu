using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eMenu.Models.MenuModels
{
    public class MenuDuzenleViewModel
    {
        public IEnumerable<SelectListItem> Menus { get; set; }

        public IEnumerable<SelectListItem> Foods { get; set; }
    }
}