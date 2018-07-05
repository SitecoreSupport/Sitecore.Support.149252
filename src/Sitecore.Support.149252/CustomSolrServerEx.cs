
namespace Sitecore.Support
{
  using Sitecore.ContentSearch.Diagnostics;
  using Sitecore.ContentSearch.SolrNetExtension;
  using Sitecore.ContentSearch.SolrNetExtension.Impl;
  using SolrNet;
  using SolrNet.Commands.Parameters;
  using SolrNet.Mapping.Validation;
  using System;
  using System.Reflection;
  using System.Text.RegularExpressions;

  public class CustomSolrServerEx<T> : SolrServerEx<T>, ISolrOperationsEx<T>, ISolrOperations<T>, ISolrReadOnlyOperations<T>, ISolrBasicReadOnlyOperations<T>, ISolrSpellCheckProvider<T>, ISolrSuggestProvider, ISolrSchemaProvider
  {
    public CustomSolrServerEx(ISolrBasicOperationsEx<T> basicServer, IReadOnlyMappingManager mappingManager, IMappingValidator _schemaMappingValidator) : base(basicServer, mappingManager, _schemaMappingValidator)
    {      
    }

    public string FixTildeBeforeParameter(string q)
    {
      string pattern = "\\\\~\\d{1,10}.{0,1}\\d{0,}\"";
      Match match = Regex.Match(q, pattern);
      if (match.Success)
      {
        string oldValue = match.Captures[match.Captures.Count - 1].Value;
        string newValue = oldValue.Replace("\"", string.Empty).Replace(@"\~", "\"~");
        q = q.Replace(oldValue, newValue);
        SearchLog.Log.Info("#Sitecore.Support.149252 Overridden Query - ?q=" + q, null);
        return this.FixTildeBeforeParameter(q);
      }
      return q;
    }

    SolrQueryResults<T> ISolrReadOnlyOperations<T>.Query(string q, QueryOptions options)
    {
      if (q.Contains("~"))
      {
        q = this.FixTildeBeforeParameter(q);
        return (this.GetField(this, typeof(SolrNet.Impl.SolrServer<T>), "basicServer") as ISolrBasicOperations<T>).Query(new SolrQuery(q), options);
      }
      return base.Query(q, options);
    }

    private object GetField(object obj, Type type, string name)
    {
      if ((obj != null) && (name.Length > 0))
      {
        FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
          return field.GetValue(obj);
        }
      }
      return null;
    }
  }
}