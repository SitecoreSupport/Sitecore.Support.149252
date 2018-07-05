using Sitecore.ContentSearch.SolrProvider;
using Sitecore.ContentSearch.SolrProvider.Abstractions;
using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.Support.ContentSearch.SolrProvider.SolrNetIntegration
{
  public class SolrStartup : DefaultSolrStartUp
  {
    public SolrStartup() : this(SolrContentSearchManager.HttpWebRequestFactory, SolrContentSearchManager.SolrSettings)
    {
    }

    public SolrStartup(HttpWebAdapters.IHttpWebRequestFactory requestFactory) : this(requestFactory, SolrContentSearchManager.SolrSettings)
    {
      
    }

    public SolrStartup(HttpWebAdapters.IHttpWebRequestFactory requestFactory, BaseSolrSpecificSettings solrSettings) : base(requestFactory, solrSettings)
    {
      FieldInfo field = typeof(DefaultSolrStartUp).GetField
         ("operations", BindingFlags.Instance | BindingFlags.NonPublic);
      field.SetValue(this, new CustomSolrLocator<Dictionary<string, object>>());
    }
  }
}