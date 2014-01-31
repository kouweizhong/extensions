﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.SMS.Views
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
    
    #line 1 "..\..\SMS\Views\MultipleSMS.cshtml"
    using Signum.Engine;
    
    #line default
    #line hidden
    
    #line 2 "..\..\SMS\Views\MultipleSMS.cshtml"
    using Signum.Entities.SMS;
    
    #line default
    #line hidden
    
    #line 6 "..\..\SMS\Views\MultipleSMS.cshtml"
    using Signum.Utilities;
    
    #line default
    #line hidden
    
    #line 3 "..\..\SMS\Views\MultipleSMS.cshtml"
    using Signum.Web;
    
    #line default
    #line hidden
    
    #line 5 "..\..\SMS\Views\MultipleSMS.cshtml"
    using Signum.Web.Extensions.SMS.Models;
    
    #line default
    #line hidden
    
    #line 4 "..\..\SMS\Views\MultipleSMS.cshtml"
    using Signum.Web.SMS;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/SMS/Views/MultipleSMS.cshtml")]
    public partial class MultipleSMS : System.Web.Mvc.WebViewPage<dynamic>
    {
        public MultipleSMS()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n\r\n");

            
            #line 9 "..\..\SMS\Views\MultipleSMS.cshtml"
Write(Html.ScriptCss("~/SMS/Content/SF_SMS.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 11 "..\..\SMS\Views\MultipleSMS.cshtml"
 using (var e = Html.TypeContext<MultipleSMSModel>())
{

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"sf-sms-edit-container\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 14 "..\..\SMS\Views\MultipleSMS.cshtml"
   Write(Html.ValueLine(e, s => s.Message, vl =>
        {
            vl.ValueLineType = ValueLineType.TextArea;
            vl.ValueHtmlProps["cols"] = "30";
            vl.ValueHtmlProps["rows"] = "6";
            vl.ValueHtmlProps["class"] = "sf-sms-msg-text";
        }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"sf-sms-characters-left\"");

WriteLiteral(">\r\n            <p>\r\n                <span>");

            
            #line 23 "..\..\SMS\Views\MultipleSMS.cshtml"
                 Write(SmsMessage.RemainingCharacters.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("</span>: <span");

WriteLiteral(" class=\"sf-sms-chars-left\"");

WriteLiteral("></span>\r\n            </p>\r\n        </div>\r\n        <div>\r\n            <input");

WriteLiteral(" type=\"button\"");

WriteLiteral(" class=\"sf-button sf-sms-remove-chars\"");

WriteLiteral(" value=\"Remove non valid characters\"");

WriteLiteral(" />\r\n        </div>\r\n    </div>\r\n");

WriteLiteral("    <br />\r\n");

            
            #line 31 "..\..\SMS\Views\MultipleSMS.cshtml"
    
            
            #line default
            #line hidden
            
            #line 31 "..\..\SMS\Views\MultipleSMS.cshtml"
Write(Html.ValueLine(e, s => s.From));

            
            #line default
            #line hidden
            
            #line 31 "..\..\SMS\Views\MultipleSMS.cshtml"
                                   
    
            
            #line default
            #line hidden
            
            #line 32 "..\..\SMS\Views\MultipleSMS.cshtml"
Write(Html.Hidden(e.Compose("ProviderKeys"), e.Value.ProviderKeys));

            
            #line default
            #line hidden
            
            #line 32 "..\..\SMS\Views\MultipleSMS.cshtml"
                                                                 ;
    
            
            #line default
            #line hidden
            
            #line 33 "..\..\SMS\Views\MultipleSMS.cshtml"
Write(Html.Hidden(e.Compose("WebTypeName"), e.Value.WebTypeName));

            
            #line default
            #line hidden
            
            #line 33 "..\..\SMS\Views\MultipleSMS.cshtml"
                                                               ;
}

            
            #line default
            #line hidden
            
            #line 35 "..\..\SMS\Views\MultipleSMS.cshtml"
Write(Html.ScriptsJs("~/SMS/Scripts/SF_SMS.js"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("\r\n");

        }
    }
}
#pragma warning restore 1591
