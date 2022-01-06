namespace DemoForm;

public class CopyModelParameterValidator : AbstractValidator<CopyModelParameter>
{
    public CopyModelParameterValidator()
    {
        RuleFor(p => p.SourceModelId).NotEmpty().NotNull();
    }
}
