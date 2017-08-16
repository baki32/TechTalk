namespace TechTalk.Controllers
{ 
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using OfficeDevPnP.Core.Framework.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using NetCoreWeb.Models;
    using Mvc.JQuery.DataTables;
    using System.Linq;
    using System.Web.UI.WebControls;
    using System;
    using System.Linq.Dynamic;
    using NetCoreWeb;

    [Authorize]
    [AuditLog(EventTypeName = "SharePointListAPIAudit", IncludeHeaders = true, IncludeModel = true)]
    public class ListsAPIController : Controller
    {
        [HttpPost]
        public ActionResult UpdateListTitle(SharePointListViewModel data)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            if (spContext == null) throw new Exception("PROBLEM"); //issues with configuration

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var list = clientContext.Web.Lists.GetById(data.ListId);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    list.Title = data.ListTitle;
                    list.Update();
                    clientContext.ExecuteQuery();
                }
            }
            return Json(new { data = true });
        }
    }
}
