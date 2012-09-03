﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using Signum.Web;
using Signum.Web.Extensions.Properties;
using Signum.Entities.Chart;
using Signum.Utilities;
using Signum.Engine.DynamicQuery;
using Signum.Entities.DynamicQuery;
using System.Web.Script.Serialization;
using Signum.Entities;
using Signum.Engine;
using System.Web.Routing;
using System.Web.Mvc;
using Signum.Entities.Basics;
using Signum.Engine.Basics;
using Signum.Web.Reports;
using Signum.Entities.UserQueries;
using Signum.Engine.Chart;
using Signum.Engine.Authorization;
using Signum.Entities.Authorization;
using Signum.Entities.Reflection;
using System.Diagnostics;
using System.Text;

namespace Signum.Web.Chart
{
    public static class ChartClient
    {
        public static string ViewPrefix = "~/Chart/Views/{0}.cshtml";

        public static string ChartRequestView = ViewPrefix.Formato("ChartRequest");
        public static string ChartBuilderView = ViewPrefix.Formato("ChartBuilder");
        public static string ChartResultsView = ViewPrefix.Formato("ChartResults");
        public static string ChartScriptCodeView = ViewPrefix.Formato("ChartScriptCode");

        public static void Start()
        {
            if (Navigator.Manager.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                Navigator.RegisterArea(typeof(ChartClient));

                Navigator.AddSettings(new List<EntitySettings>
                {
                    new EmbeddedEntitySettings<ChartRequest>(),
                    new EmbeddedEntitySettings<ChartColumnDN> { PartialViewName = _ => ViewPrefix.Formato("ChartColumn") },
                    new EmbeddedEntitySettings<ChartScriptColumnDN>{ PartialViewName = _ => ViewPrefix.Formato("ChartScriptColumn") },
                    new EntitySettings<ChartScriptDN>(EntityType.Admin) { PartialViewName = _ => ViewPrefix.Formato("ChartScript") },
                });

                ButtonBarQueryHelper.RegisterGlobalButtons(ButtonBarQueryHelper_GetButtonBarForQueryName);

                RouteTable.Routes.MapRoute(null, "ChartFor/{webQueryName}",
                    new { controller = "Chart", action = "Index", webQueryName = "" });

                UserChartClient.Start();
                ChartColorClient.Start();
            }
        }

        public static EntityMapping<ChartColumnDN> MappingChartColumn = new EntityMapping<ChartColumnDN>(true)
            .SetProperty(ct => ct.Token, ctx =>
            {
                var tokenName = "";

                var chartTokenInputs = ctx.Parent.Inputs;
                bool stop = false;
                for (var i = 0; !stop; i++)
                {
                    var subtokenName = chartTokenInputs.TryGetC("ddlTokens_" + i);
                    if (string.IsNullOrEmpty(subtokenName))
                        stop = true;
                    else
                        tokenName = tokenName.HasText() ? (tokenName + "." + subtokenName) : subtokenName;
                }

                if (string.IsNullOrEmpty(tokenName))
                    return ctx.None();

                var qd = DynamicQueryManager.Current.QueryDescription(
                    Navigator.ResolveQueryName(ctx.ControllerContext.HttpContext.Request.Params["webQueryName"]));

                var chartToken = (ChartColumnDN)ctx.Parent.UntypedValue;

                return QueryUtils.Parse(tokenName, qt => qt.SubTokensChart(qd.Columns, chartToken.ParentChart.GroupResults));
            })
            .SetProperty(ct => ct.DisplayName, ctx =>
            {
                if (string.IsNullOrEmpty(ctx.Input))
                    return ctx.None();

                return ctx.Input;
            });

        public static EntityMapping<ChartRequest> MappingChartRequest = new EntityMapping<ChartRequest>(true)
            .SetProperty(cr => cr.Filters, ctx => ExtractChartFilters(ctx))
            .SetProperty(cr => cr.Orders, ctx => ExtractChartOrders(ctx))
            .SetProperty(cb => cb.Columns, new MListCorrelatedOrDefaultMapping<ChartColumnDN>(MappingChartColumn));


        public class MListCorrelatedOrDefaultMapping<S> : MListMapping<S>
        {
            public MListCorrelatedOrDefaultMapping()
                : base()
            {
            }

            public MListCorrelatedOrDefaultMapping(Mapping<S> elementMapping)
                : base(elementMapping)
            {
            }

            public override MList<S> GetValue(MappingContext<MList<S>> ctx)
            {
                MList<S> list = ctx.Value;
                int i = 0;

                foreach (MappingContext<S> itemCtx in GenerateItemContexts(ctx).OrderBy(mc => mc.ControlID.Substring(mc.ControlID.LastIndexOf("_") + 1).ToInt().Value))
                {
                    if (i < list.Count)
                    {
                        itemCtx.Value = list[i];
                        itemCtx.Value = ElementMapping(itemCtx);

                        ctx.AddChild(itemCtx);
                        list[i] = itemCtx.Value;
                    }
                    i++;
                }

                return list;
            }
        }

        static List<Entities.DynamicQuery.Filter> ExtractChartFilters(MappingContext<List<Entities.DynamicQuery.Filter>> ctx)
        {
            var qd = DynamicQueryManager.Current.QueryDescription(
                Navigator.ResolveQueryName(ctx.ControllerContext.HttpContext.Request.Params["webQueryName"]));

            ChartRequest chartRequest = (ChartRequest)ctx.Parent.UntypedValue;

            return FindOptionsModelBinder.ExtractFilterOptions(ctx.ControllerContext.HttpContext, qt => qt.SubTokensChart(qd.Columns, chartRequest.GroupResults))
                .Select(fo => fo.ToFilter()).ToList();
        }

        static List<Order> ExtractChartOrders(MappingContext<List<Order>> ctx)
        {
            var qd = DynamicQueryManager.Current.QueryDescription(
                Navigator.ResolveQueryName(ctx.ControllerContext.HttpContext.Request.Params["webQueryName"]));

            ChartRequest chartRequest = (ChartRequest)ctx.Parent.UntypedValue;

            return FindOptionsModelBinder.ExtractOrderOptions(ctx.ControllerContext.HttpContext, qt => qt.SubTokensChart(qd.Columns, chartRequest.GroupResults))
                .Select(fo => fo.ToOrder()).ToList();
        }

        static ToolBarButton[] ButtonBarQueryHelper_GetButtonBarForQueryName(QueryButtonContext ctx)
        {
            if (ctx.Prefix.HasText())
                return null;

            return new[] { ChartQueryButton(ctx.Prefix) };
        }

        public static ToolBarButton ChartQueryButton(string prefix)
        {
            if (!ChartPermissions.ViewCharting.IsAuthorized())
                return null;

            string chartNewText = Resources.Chart_Chart;

            return
                new ToolBarButton
                {
                    Id = TypeContextUtilities.Compose(prefix, "qbChartNew"),
                    AltText = chartNewText,
                    Text = chartNewText,
                    OnClick = Js.SubmitOnly(RouteHelper.New().Action("Index", "Chart"), JsFindNavigator.GetFor(prefix).requestData()).ToJS(),
                    DivCssClass = ToolBarButton.DefaultQueryCssClass
                };
        }

        public static MvcHtmlString ChartTokenCombo(this HtmlHelper helper, QueryTokenDN chartToken, IChartBase chart, QueryDescription qd, Context context)
        {
            var tokenPath = chartToken.Token.FollowC(qt => qt.Parent).Reverse().NotNull().ToList();

            QueryToken queryToken = chartToken.Token;
            if (tokenPath.Count > 0)
                queryToken = tokenPath[0];

            HtmlStringBuilder sb = new HtmlStringBuilder();

            bool canAggregate = (chartToken as ChartColumnDN).TryCS(ct => ct.IsGroupKey == false) ?? true;

            var rootTokens = ChartUtils.SubTokensChart(null, qd.Columns, chart.GroupResults && canAggregate);

            sb.AddLine(SearchControlHelper.TokenOptionsCombo(
                helper, qd.QueryName, SearchControlHelper.TokensCombo(rootTokens, queryToken), context, 0, false));
            
            for (int i = 0; i < tokenPath.Count; i++)
            {
                QueryToken t = tokenPath[i];
                List<QueryToken> subtokens = t.SubTokensChart(qd.Columns, canAggregate);
                if (!subtokens.IsEmpty())
                {
                    bool moreTokens = i + 1 < tokenPath.Count;
                    sb.AddLine(SearchControlHelper.TokenOptionsCombo(
                        helper, qd.QueryName, SearchControlHelper.TokensCombo(subtokens, moreTokens ? tokenPath[i + 1] : null), context, i + 1, !moreTokens));
                }
            }
            
            return sb.ToHtml();
        }

        public static MvcHtmlString ChartRootTokens(this HtmlHelper helper, IChartBase chart, QueryDescription qd, Context context)
        {
            var subtokens = ChartUtils.SubTokensChart(null, qd.Columns, chart.GroupResults);

            return SearchControlHelper.TokenOptionsCombo(
                helper, qd.QueryName, SearchControlHelper.TokensCombo(subtokens, null), context, 0, false);
        }

        public static string ChartTypeImgClass(MList<ChartColumnDN> columns, ChartScriptDN current, ChartScriptDN script)
        {
            string css = "sf-chart-img";

            if (script.IsCompatibleWith(columns))
                css += " sf-chart-img-equiv";

            if (script.Is(current))
                css += " sf-chart-img-curr";

            return css;
        }

        public static string ToJS(this ChartRequest request)
        {
            return new JsOptionsBuilder(true)
            {
                { "webQueryName", request.QueryName.TryCC(q => Navigator.ResolveWebQueryName(q).SingleQuote()) },
                { "orders", request.Orders.IsEmpty() ? null : ("[" + request.Orders.ToString(oo => oo.ToJS().SingleQuote(), ",") + "]") }
            }.ToJS();
        }

        public static string ToJS(this Order order)
        {
            return (order.OrderType == OrderType.Descending ? "-" : "") + order.Token.FullKey();
        }

        public static string ToJS(this UserChartDN userChart)
        {
            return new JsOptionsBuilder(true)
            {
                { "webQueryName", userChart.QueryName.TryCC(q => Navigator.ResolveWebQueryName(q).SingleQuote()) },
                { "orders", userChart.Orders.IsEmpty() ? null : ("[" + userChart.Orders.ToString(oo => oo.ToJS().SingleQuote(), ",") + "]") }
            }.ToJS();
        }

        public static string ToJS(this QueryOrderDN order)
        {
            return (order.OrderType == OrderType.Descending ? "-" : "") + order.Token.FullKey();
        }
    }
}
