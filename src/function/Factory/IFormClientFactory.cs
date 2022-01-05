namespace DemoForm;

public interface IFormClientFactory
{
    DocumentModelAdministrationClient CreateClient(ENVIRONMENT env);
}
