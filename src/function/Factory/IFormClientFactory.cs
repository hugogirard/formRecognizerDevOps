using Azure.AI.FormRecognizer.Training;

namespace DemoForm;

public interface IFormClientFactory
{
    FormTrainingClient CreateClient(ENVIRONMENT env);
}
