using Microsoft.Practices.ServiceLocation;
using Sitecore.ContentSearch.SolrProvider;
using Sitecore.ContentSearch.SolrProvider.DocumentSerializers;
using Sitecore.ContentSearch.SolrProvider.Parsers;
using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using System;
using System.Collections.Generic;

namespace Sitecore.Support.ContentSearch.SolrProvider.SolrNetIntegration
{
    public class SolrStartup : DefaultSolrStartUp
    {
        public override void Initialize()
        {
            if (!SolrContentSearchManager.IsEnabled)
            {
                throw new InvalidOperationException("Solr configuration is not enabled. Please check your settings and include files.");
            }
            this.Operations.DocumentSerializer = new SolrFieldBoostingDictionarySerializer(this.Operations.FieldSerializer);
            this.Operations.HttpWebRequestFactory = SolrContentSearchManager.HttpWebRequestFactory;
            this.Operations.SchemaParser = new SolrSchemaParser();
            foreach (string str in SolrContentSearchManager.Cores)
            {
                this.AddCore(str, typeof(Dictionary<string, object>), $"{SolrContentSearchManager.ServiceAddress}/{str}");
            }
            if (SolrContentSearchManager.EnableHttpCache)
            {
                ISolrCache cache = ReflectionUtil.GetField(this, typeof(DefaultSolrStartUp), "solrCache") as ISolrCache;
                this.Operations.HttpCache = cache;
            }
            this.Operations.CoreAdmin = this.BuildCoreAdmin();
            ServiceLocator.SetLocatorProvider(() => new DefaultServiceLocator<Dictionary<string, object>>(this.Operations));
            SolrContentSearchManager.SolrAdmin = this.Operations.CoreAdmin;
            this.ReplaceSolrOperations();
            SolrContentSearchManager.Initialize();
        }


        public void ReplaceSolrOperations()
        {
            IServiceLocator test2 = ServiceLocator.Current;
            DefaultSolrLocator<Dictionary<string, object>> OpPr = Sitecore.Reflection.ReflectionUtil.GetProperty(test2, "Operations") as DefaultSolrLocator<Dictionary<string, object>>;
            Dictionary<string, Dictionary<string, object>> KeyPr = Sitecore.Reflection.ReflectionUtil.GetProperty(OpPr, "KeyedServiceCollection") as Dictionary<string, Dictionary<string, object>>;

            foreach (string key in KeyPr.Keys)
            {
                Dictionary<string, object> objects = KeyPr[key];
                string typeKey = typeof(ISolrOperations<Dictionary<string, object>>).Name;
                objects.Remove(typeKey);

                var operations = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Dictionary<string, object>>>(key);
                var validator = ServiceLocator.Current.GetInstance<IMappingValidator>();
                var manager = ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>();
                Sitecore.Support.SolrServer<Dictionary<string, object>> server = new Sitecore.Support.SolrServer<Dictionary<string, object>>(operations, manager, validator);
                objects.Add(typeKey, server);
            }
        }

    }
}