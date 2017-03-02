namespace Sitecore.Support.ContentSearch.SolrProvider.Pipelines.Loader
{
    using Sitecore.ContentSearch.SolrProvider;
    using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
    using Sitecore.Pipelines;
    using SolrNetIntegration;
    using System;

    public class InitializeSolrProvider
    {
        public void Process(PipelineArgs args)
        {
            if (SolrContentSearchManager.IsEnabled)
            {
                if (IntegrationHelper.IsSolrConfigured())
                {
                    IntegrationHelper.ReportDoubleSolrConfigurationAttempt(base.GetType());
                }
                else
                {
                    new SolrStartup().Initialize();
                }
            }
        }
    }
}
