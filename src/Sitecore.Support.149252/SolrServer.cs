using Sitecore.ContentSearch.Diagnostics;
using Sitecore.Diagnostics;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Sitecore.Support
{
    public class SolrServer<T> : SolrNet.Impl.SolrServer<T>, ISolrReadOnlyOperations<T>
    {
        public SolrServer(ISolrBasicOperations<T> basicServer, IReadOnlyMappingManager mappingManager, IMappingValidator _schemaMappingValidator) : base(basicServer, mappingManager, _schemaMappingValidator)
        {

        }

        SolrQueryResults<T> ISolrReadOnlyOperations<T>.Query(string q, QueryOptions options)
        {
            if (q.Contains("~"))
            {
                q = FixTildeBeforeParameter(q);// try fixing query
                ISolrBasicOperations<T> basicServer = ReflectionUtil.GetField(this, typeof(SolrNet.Impl.SolrServer<T>), "basicServer") as ISolrBasicOperations<T>;
                return basicServer.Query(new SolrQuery(q), options);
            }
            return base.Query(q, options);           
        }

        public string FixTildeBeforeParameter(string q)
        {
            string pattern = @"\\~\d{1,10}.{0,1}\d{0,}" + '"';
            Match match = Regex.Match(q, pattern);
            if (match.Success)
            {
                string oldValue = match.Captures[match.Captures.Count - 1].Value;
                string newValue = oldValue.Replace("\"", String.Empty).Replace("\\~", "\"~");
                q = q.Replace(oldValue, newValue);
                SearchLog.Log.Info("#Sitecore.Support.149252 Overridden Query - ?q=" + q, null);
                return q;
            }
            return q;
        }
    }
}
