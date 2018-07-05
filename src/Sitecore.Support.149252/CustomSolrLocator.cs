using Sitecore.ContentSearch.SolrNetExtension;
using SolrNet;
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;

namespace Sitecore.Support
{
  public class CustomSolrLocator<T> : Sitecore.ContentSearch.SolrProvider.SolrNetIntegration.DefaultSolrLocator<T>
  {
    public override ISolrOperationsEx<T> GetServer(ISolrConnection connection)
    {
      ISolrBasicOperationsEx<T> basicServer = this.GetBasicServer(connection);
      IValidationRule[] rules = new IValidationRule[] { new MappedPropertiesIsInSolrSchemaRule(), new RequiredFieldsAreMappedRule(), new UniqueKeyMatchesMappingRule(), new MultivaluedMappedToCollectionRule() };
      this.MappingValidator = new MappingValidator(this.MappingManager, rules);
      return new CustomSolrServerEx<T>(basicServer, this.MappingManager, this.MappingValidator);
    }
  }
}