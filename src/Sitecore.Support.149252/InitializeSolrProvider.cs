namespace Sitecore.Support.ContentSearch.SolrProvider.Pipelines.Loader
{
    using Sitecore.ContentSearch.SolrProvider;
    using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
    using Sitecore.Pipelines;
    using Sitecore.Support.ContentSearch.SolrProvider.SolrNetIntegration;
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    public class InitializeSolrProvider
    {
        private bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors) => 
            true;

        private void IgnoreBadCertificates()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.AcceptAllCertifications);
        }

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
                    if (SolrContentSearchManager.ServiceAddress.ToLower().Contains("https"))
                    {
                        this.IgnoreBadCertificates();
                    }
                    new SolrStartup().Initialize();
                }
            }
        }
    }
}

