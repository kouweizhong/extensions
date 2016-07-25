﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 5 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
    using Newtonsoft.Json.Linq;
    
    #line default
    #line hidden
    
    #line 3 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
    using Signum.Engine;
    
    #line default
    #line hidden
    using Signum.Entities;
    
    #line 2 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
    using Signum.Entities.DynamicQuery;
    
    #line default
    #line hidden
    
    #line 1 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
    using Signum.Entities.UserQueries;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    #line 4 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
    using Signum.Web.UserQueries;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/UserQueries/Views/UserQueryCountPart.cshtml")]
    public partial class _UserQueries_Views_UserQueryCountPart_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _UserQueries_Views_UserQueryCountPart_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\n");

            
            #line 7 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
 using (var e = Html.TypeContext<UserQueryCountPartEntity>())
{
    var uq = e.Value.UserQuery.Retrieve();
    object queryName = Signum.Engine.Basics.QueryLogic.ToQueryName(uq.Query);
    FindOptions fo = new FindOptions(queryName)
    {
        ShowFilters = false
    };

    fo.ApplyUserQuery(uq);

    var queryNiceName = QueryUtils.GetNiceName(fo.QueryName);


            
            #line default
            #line hidden
WriteLiteral("    <a");

WriteAttribute("class", Tuple.Create(" class=\"", 523), Tuple.Create("\"", 580)
            
            #line 20 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
, Tuple.Create(Tuple.Create("", 531), Tuple.Create<System.Object, System.Int32>(!e.Value.ShowName ? "dashboard-tooltip" : null
            
            #line default
            #line hidden
, 531), false)
);

WriteAttribute("href", Tuple.Create(" href=\"", 581), Tuple.Create("\"", 602)
            
            #line 20 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
, Tuple.Create(Tuple.Create("", 588), Tuple.Create<System.Object, System.Int32>(fo.ToString()
            
            #line default
            #line hidden
, 588), false)
);

WriteLiteral("\n       data-toggle=\"tooltip\"");

WriteLiteral(" data-placement=\"top\"");

WriteLiteral(" data-original-title=\"");

            
            #line 21 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
                                                                  Write(queryNiceName);

            
            #line default
            #line hidden
WriteLiteral("\"");

WriteLiteral(">\n\n");

            
            #line 23 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
        
            
            #line default
            #line hidden
            
            #line 23 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
         if (e.Value.IconClass.HasText())
        {

            
            #line default
            #line hidden
WriteLiteral("            <i");

WriteAttribute("class", Tuple.Create(" class=\"", 759), Tuple.Create("\"", 795)
, Tuple.Create(Tuple.Create("", 767), Tuple.Create("glyphicon", 767), true)
            
            #line 25 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
, Tuple.Create(Tuple.Create(" ", 776), Tuple.Create<System.Object, System.Int32>(e.Value.IconClass
            
            #line default
            #line hidden
, 777), false)
);

WriteLiteral("></i>\n");

            
            #line 26 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("\n");

            
            #line 28 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
        
            
            #line default
            #line hidden
            
            #line 28 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
          
            var context = new Context(e, "{0}_cnt".FormatWith(e.Value.Id));
        
            
            #line default
            #line hidden
WriteLiteral("\n\n");

            
            #line 32 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
        
            
            #line default
            #line hidden
            
            #line 32 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
         if (e.Value.ShowName)
        {
            
            
            #line default
            #line hidden
            
            #line 34 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
       Write(queryNiceName);

            
            #line default
            #line hidden
            
            #line 34 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
                          
        }

            
            #line default
            #line hidden
WriteLiteral("\n        &nbsp;\n        <div");

WriteAttribute("id", Tuple.Create(" id=\"", 1017), Tuple.Create("\"", 1030)
            
            #line 38 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
, Tuple.Create(Tuple.Create("", 1022), Tuple.Create<System.Object, System.Int32>(context
            
            #line default
            #line hidden
, 1022), false)
);

WriteLiteral(" class=\"badge\"");

WriteLiteral("></div>\n\n");

            
            #line 40 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
        
            
            #line default
            #line hidden
            
            #line 40 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
          
            var function = JsModule.Finder["count"](
                fo.ToJS(context.Prefix),
                new JRaw("'" + context.Prefix + "'.get()"));
        
            
            #line default
            #line hidden
WriteLiteral("\n\n");

WriteLiteral("        ");

            
            #line 46 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
   Write(MvcHtmlString.Create("<script>" + function.ToHtmlString() + "</script>"));

            
            #line default
            #line hidden
WriteLiteral("\n    </a>\n");

            
            #line 48 "..\..\UserQueries\Views\UserQueryCountPart.cshtml"
}

            
            #line default
            #line hidden
WriteLiteral("\n<style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\n    div.tooltip-inner {\n        max-width: none;\n        white-space: nowrap;\n " +
"   }\n</style>\n\n");

        }
    }
}
#pragma warning restore 1591
