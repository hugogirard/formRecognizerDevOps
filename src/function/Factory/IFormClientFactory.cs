namespace DemoForm;

public interface IFormClientFactory
{
    DocumentModelAdministrationClient CreateAdministrationClient(MODEL_ENVIRONMENT env);

    DocumentAnalysisClient CreateAnalysisClient(MODEL_ENVIRONMENT env);
}
