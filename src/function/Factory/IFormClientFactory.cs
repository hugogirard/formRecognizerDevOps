namespace DemoForm;

public interface IFormClientFactory
{
    DocumentModelAdministrationClient CreateAdministrationClient(FORM_RECOGNIZER_ENVIRONMENT env);

    DocumentAnalysisClient CreateAnalysisClient(FORM_RECOGNIZER_ENVIRONMENT env);
}
