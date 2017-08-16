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
    [AuditLog(EventTypeName = "SharePointAudit", IncludeHeaders = false, IncludeModel = true)]
    public class ListsController : Controller
    {
        SharePointContext spContext;
        List<SharePointListViewModel> spLists;
        // GET: /<controller>/
        public IActionResult Index()
        {
            //if (SharePointContextProvider.Current == null) return View(); //null if middleware not intercepted

            //spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            //if (spContext == null) return View(); //issues with configuration

            //spLists = new List<SharePointListViewModel>();

            ////build a client context to work with data
            //using (var clientContext = spContext.CreateUserClientContextForSPHost())
            //{
            //    if (clientContext != null)
            //    {
            //        var lists = clientContext.Web.Lists;
            //        clientContext.Load(lists);
            //        clientContext.ExecuteQuery();

            //        foreach (var list in lists)
            //        {
            //            spLists.Add(new SharePointListViewModel() { ListTitle = list.Title, ListId = list.Id });
            //        }
            //    }
            //}

            //return View(spLists);
            return View();
        }


        [HttpPost]
        public ActionResult LoadDataPaged()
        {

            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];
            //Find Order Column
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"];
            var sortColumnDir = Request.Form["order[0][dir]"];


            int pageSize = length.FirstOrDefault() != null ? Convert.ToInt32(length) : 0;
            int skip = start.FirstOrDefault() != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            if (spContext == null) throw new Exception("PROBLEM"); //issues with configuration


            //var getDataUrl = Url.Action(nameof(ListsController.GetLists), spContext);
            //var vm = DataTablesHelper.DataTableVm<SharePointListViewModel>("idForTableElement", getDataUrl);

            spLists = new List<SharePointListViewModel>();

            //build a client context to work with data
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var lists = clientContext.Web.Lists;
                    clientContext.Load(lists);
                    clientContext.ExecuteQuery();

                    foreach (var list in lists)
                    {
                        spLists.Add(new SharePointListViewModel() { ListTitle = list.Title, ListId = list.Id});
                    }
                }
                // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
                var v = (from a in spLists select a);

                //SORT
                if (!(string.IsNullOrEmpty(sortColumn.FirstOrDefault()) && string.IsNullOrEmpty(sortColumnDir.FirstOrDefault())))
                {
                    v = v.OrderBy(sortColumn.FirstOrDefault() + " " + sortColumnDir.FirstOrDefault());
                }

                recordsTotal = v.Count();
                var data = v.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
        }
        //public ActionResult LoadData()
        //{
        //    spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

        //    if (spContext == null) throw new Exception("PROBLEM"); //issues with configuration


        //    //var getDataUrl = Url.Action(nameof(ListsController.GetLists), spContext);
        //    //var vm = DataTablesHelper.DataTableVm<SharePointListViewModel>("idForTableElement", getDataUrl);

        //    spLists = new List<SharePointListViewModel>();

        //    //build a client context to work with data
        //    using (var clientContext = spContext.CreateUserClientContextForSPHost())
        //    {
        //        if (clientContext != null)
        //        {
        //            var lists = clientContext.Web.Lists;
        //            clientContext.Load(lists);
        //            clientContext.ExecuteQuery();

        //            foreach (var list in lists)
        //            {
        //                spLists.Add(new SharePointListViewModel() { ListTitle = list.Title, ListId = list.Id });
        //            }
        //        }
        //    }
        //    return Json(new { data = spLists });
        //}       
    }
}
