namespace DemoForm;

public class DocumentInfoParameterValidator : AbstractValidator<DocumentInfo>
{
    public DocumentInfoParameterValidator()
    {
        RuleFor(d => d.ModelId).NotEmpty().NotNull();
        RuleFor(d => d.DocumentUrl).NotEmpty().NotNull();
        RuleFor(d => d.Environment).IsInEnum();
    }
}
