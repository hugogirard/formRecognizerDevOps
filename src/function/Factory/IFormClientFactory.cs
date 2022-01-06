namespace DemoForm;

public interface IFormClientFactory
{
    DocumentModelAdministrationClient CreateClient(FORM_RECOGNIZER_ENVIRONMENT env);
}
