﻿#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Signum.Utilities;
using Signum.Engine;
using Signum.Entities;
using Signum.Engine.Maps;
using Signum.Web.Extensions.Properties;
using Signum.Engine.DynamicQuery;
using Signum.Entities.Reflection;
using Signum.Entities.DynamicQuery;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using Signum.Entities.Reports;
using Signum.Entities.Basics;
using Signum.Engine.Reports;
using Signum.Engine.Basics;
#endregion

namespace Signum.Web.Reports
{
    [HandleException, AuthenticationRequired]
    public class ReportController : Controller
    {
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ToExcelPlain(FindOptions findOptions, string prefix)
        {
            if (!Navigator.IsFindable(findOptions.QueryName))
                throw new UnauthorizedAccessException(Signum.Web.Properties.Resources.ViewForType0IsNotAllowed.Formato(findOptions.QueryName));

            QueryRequest request = findOptions.ToQueryRequest();

            ResultTable queryResult = DynamicQueryManager.Current.ExecuteQuery(request);
            byte[] binaryFile = PlainExcelGenerator.WritePlainExcel(queryResult);

            return File(binaryFile, MimeType.FromExtension(".xlsx"), Navigator.ResolveWebQueryName(findOptions.QueryName) + ".xlsx");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExcelReport(FindOptions findOptions, Lite<ExcelReportDN> excelReport, string prefix)
        {
            if (!Navigator.IsFindable(findOptions.QueryName))
                throw new UnauthorizedAccessException(Signum.Web.Properties.Resources.ViewForType0IsNotAllowed.Formato(findOptions.QueryName));

            QueryRequest request = findOptions.ToQueryRequest();
           
            byte[] file = ReportsLogic.ExecuteExcelReport(excelReport, request);

            return File(file, MimeType.FromExtension(".xlsx"), Navigator.ResolveWebQueryName(findOptions.QueryName)  + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xlsx");
            //Known Bug in IE: When the file dialog is shown, if Open is chosen the Excel will be broken as a result of IE automatically adding [1] to the name. 
            //There's not workaround for this, so either click on Save instead of Open, or use Firefox or Chrome
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ViewResult Administer(string webQueryName)
        {
            object queryName = Navigator.ResolveQueryName(webQueryName);
            
            QueryDN query = QueryLogic.RetrieveOrGenerateQuery(queryName);

            if (query.IsNew) //If the Query is new there won't be any reports associated => navigate directly to create one
            {
                return Navigator.View(this, new ExcelReportDN { Query = query });
            }
            else
            {
                FindOptions fo = new FindOptions(typeof(ExcelReportDN))
                {
                    FilterMode = FilterMode.AlwaysHidden,
                    SearchOnLoad = true,
                    FilterOptions = new List<FilterOption> 
                    { 
                        new FilterOption("Query", query.ToLite())
                    },
                    Creating = Js.SubmitOnly(RouteHelper.New().Action("Create", "Report"), "{{query:{0}}}".Formato(query.Id)).ToJS()
                };

                return Navigator.Find(this, fo);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ViewResult Create(Lite<QueryDN> query)
        {
            ExcelReportDN report = new ExcelReportDN { Query = query.Retrieve() };
            return Navigator.View(this, report);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save()
        {
            var context = this.ExtractEntity<ExcelReportDN>().ApplyChanges(ControllerContext, null, true).ValidateGlobal();

            ExcelReportDN report = context.Value;

            if (context.GlobalErrors.Any())
            {
                this.ModelState.FromContext(context);
                // It's a submit, I cannot return ModelState
                return Navigator.View(this, report, true);
            }

            Database.Save(report);

            this.HttpContext.Response.Redirect(Navigator.ViewRoute(report), true);
            return null;
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult Delete(Lite<ExcelReportDN> excelReport)
        {
            ExcelReportDN report = excelReport.Retrieve();
            report.Deleted = true;
            report.Save();
            return Navigator.RedirectUrl(Navigator.ViewRoute(report));
        }

        public ActionResult DownloadTemplate(Lite<ExcelReportDN> excelReport)
        {
            ExcelReportDN report = excelReport.RetrieveAndForget();

            //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + Path.GetFileName(report.File.FileName));
            
            return File(report.File.BinaryFile,
                MimeType.FromFileName(report.File.FileName), 
                report.File.FileName);
        }
    }
}
