﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Engine.Maps;
using Signum.Entities.Authorization;
using Signum.Entities.Basics;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Basics;
using Signum.Utilities;
using Signum.Utilities.DataStructures;
using System.Threading;
using Signum.Entities;
using System.Reflection;
using Signum.Entities.DynamicQuery;

namespace Signum.Engine.Authorization
{

    public static class QueryAuthLogic
    {
        static AuthCache<RuleQueryEntity, QueryAllowedRule, QueryEntity, object, QueryAllowed> cache;

        public static IManualAuth<object, QueryAllowed> Manual { get { return cache; } }

        public static bool IsStarted { get { return cache != null; } }

        public readonly static HashSet<object> AvoidAutomaticUpgradeCollection = new HashSet<object>();

        public static void Start(SchemaBuilder sb, DynamicQueryManager dqm)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                AuthLogic.AssertStarted(sb);
                QueryLogic.Start(sb, dqm);

                dqm.AllowQuery += new Func<object, bool, bool>(dqm_AllowQuery);

                cache = new AuthCache<RuleQueryEntity, QueryAllowedRule, QueryEntity, object, QueryAllowed>(sb,
                    qn => QueryLogic.ToQueryName(qn.Key),
                    QueryLogic.GetQueryEntity,
                    merger: new QueryMerger(), 
                    invalidateWithTypes : true,
                    coercer: QueryCoercer.Instance);

                AuthLogic.ExportToXml += exportAll => cache.ExportXml("Queries", "Query", QueryUtils.GetKey, b => b.ToString(), 
                    exportAll ? QueryLogic.QueryNames.Values.ToList(): null);
                AuthLogic.ImportFromXml += (x, roles, replacements) => 
                {
                    string replacementKey = "AuthRules:" + typeof(QueryEntity).Name;

                    replacements.AskForReplacements(
                        x.Element("Queries").Elements("Role").SelectMany(r => r.Elements("Query")).Select(p => p.Attribute("Resource").Value).ToHashSet(),
                        QueryLogic.QueryNames.Keys.ToHashSet(),
                        replacementKey);

                    return cache.ImportXml(x, "Queries", "Query", roles, s =>
                    {
                        var qn = QueryLogic.TryToQueryName(replacements.Apply(replacementKey, s));

                        if (qn == null)
                            return null;

                        return QueryLogic.GetQueryEntity(qn);
                    }, str =>
                    {
                        if (Enum.TryParse<QueryAllowed>(str, out var result))
                            return result;

                        var bResult = bool.Parse(str); //For backwards compatibilityS
                        return bResult ? QueryAllowed.Allow : QueryAllowed.None;

                    });
                };
            }
        }

        static bool dqm_AllowQuery(object queryName, bool fullScreen)
        {
            var allowed = GetQueryAllowed(queryName);
            return allowed == QueryAllowed.Allow || allowed == QueryAllowed.EmbeddedOnly && !fullScreen;
        }

        public static DefaultDictionary<object, QueryAllowed> QueryRules()
        {
            return cache.GetDefaultDictionary();
        }

        public static QueryRulePack GetQueryRules(Lite<RoleEntity> role, TypeEntity typeEntity)
        {
            var result = new QueryRulePack { Role = role, Type = typeEntity };
            cache.GetRules(result, QueryLogic.GetTypeQueries(typeEntity));

            var coercer = QueryCoercer.Instance.GetCoerceValue(role);
            result.Rules.ForEach(r => r.CoercedValues = EnumExtensions.GetValues<QueryAllowed>()
                .Where(a => !coercer(QueryLogic.ToQueryName(r.Resource.Key), a).Equals(a))
                .ToArray());

            return result;
        }

        public static void SetQueryRules(QueryRulePack rules)
        {
            string[] queryKeys = DynamicQueryManager.Current.GetTypeQueries(TypeLogic.EntityToType[rules.Type]).Keys.Select(qn => QueryUtils.GetKey(qn)).ToArray();

            cache.SetRules(rules, r => queryKeys.Contains(r.Key));
        }

        public static QueryAllowed GetQueryAllowed(object queryName)
        {
            if (!AuthLogic.IsEnabled || ExecutionMode.InGlobal)
                return QueryAllowed.Allow;

            return cache.GetAllowed(RoleEntity.Current, queryName);
        }

        public static QueryAllowed GetQueryAllowed(Lite<RoleEntity> role, object queryName)
        {
            return cache.GetAllowed(role, queryName);
        }

        public static AuthThumbnail? GetAllowedThumbnail(Lite<RoleEntity> role, Type entityType)
        {
            return DynamicQueryManager.Current.GetTypeQueries(entityType).Keys.Select(qn => cache.GetAllowed(role, qn)).Collapse(); 
        }

        internal static bool AllCanRead(this Implementations implementations, Func<Type, TypeAllowedAndConditions> getAllowed)
        {
            if (implementations.IsByAll)
                return true;

            return implementations.Types.All(t => getAllowed(t).MaxUI() != TypeAllowedBasic.None);
        }
    }

    class QueryMerger : IMerger<object, QueryAllowed>
    {
        public QueryAllowed Merge(object key, Lite<RoleEntity> role, IEnumerable<KeyValuePair<Lite<RoleEntity>, QueryAllowed>> baseValues)
        {
            QueryAllowed best = AuthLogic.GetMergeStrategy(role) == MergeStrategy.Union ?
                Max(baseValues.Select(a => a.Value)) :
                Min(baseValues.Select(a => a.Value));

            if (!BasicPermission.AutomaticUpgradeOfQueries.IsAuthorized(role) || QueryAuthLogic.AvoidAutomaticUpgradeCollection.Contains(key))
                return best;

            if (baseValues.Where(a => a.Value.Equals(best)).All(a => GetDefault(key, a.Key).Equals(a.Value)))
                return GetDefault(key, role);

            return best;
        }


        static QueryAllowed Max(IEnumerable<QueryAllowed> baseValues)
        {
            QueryAllowed result = QueryAllowed.None;

            foreach (var item in baseValues)
            {
                if (item > result)
                    result = item;

                if (result == QueryAllowed.Allow)
                    return result;
            }
            return result;
        }

        static QueryAllowed Min(IEnumerable<QueryAllowed> baseValues)
        {
            QueryAllowed result = QueryAllowed.Allow;

            foreach (var item in baseValues)
            {
                if (item < result)
                    result = item;

                if (result == QueryAllowed.None)
                    return result;
            }
            return result;
        }

        public Func<object, QueryAllowed> MergeDefault(Lite<RoleEntity> role)
        {
            return key =>
            {
                if (!BasicPermission.AutomaticUpgradeOfQueries.IsAuthorized(role) || 
                OperationAuthLogic.AvoidAutomaticUpgradeCollection.Contains(key))
                    return AuthLogic.GetDefaultAllowed(role) ? QueryAllowed.Allow: QueryAllowed.None;

                return GetDefault(key, role);
            };
        }

        QueryAllowed GetDefault(object key, Lite<RoleEntity> role)
        {
            return DynamicQueryManager.Current.GetEntityImplementations(key).AllCanRead(t => TypeAuthLogic.GetAllowed(role, t)) ? QueryAllowed.Allow : QueryAllowed.None;
        }
    }

    class QueryCoercer : Coercer<QueryAllowed, object>
    {
        public static readonly QueryCoercer Instance = new QueryCoercer();

        private QueryCoercer()
        {
        }

        public override Func<object, QueryAllowed, QueryAllowed> GetCoerceValue(Lite<RoleEntity> role)
        {
            return (queryName, allowed) =>
            {
                if (allowed == QueryAllowed.None)
                    return allowed;

                var implementations = DynamicQueryManager.Current.GetEntityImplementations(queryName);

                return implementations.AllCanRead(t => TypeAuthLogic.GetAllowed(role, t)) ? allowed : QueryAllowed.None;
            };
        }

        public override Func<Lite<RoleEntity>, QueryAllowed, QueryAllowed> GetCoerceValueManual(object queryName)
        {
            return (role, allowed) =>
            {
                if (allowed == QueryAllowed.None)
                    return allowed;

                var implementations = DynamicQueryManager.Current.GetEntityImplementations(queryName);

                return implementations.AllCanRead(t => TypeAuthLogic.Manual.GetAllowed(role, t)) ? allowed : QueryAllowed.None;
            };
        }
    }
}
