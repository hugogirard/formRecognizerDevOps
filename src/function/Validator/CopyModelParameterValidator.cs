namespace DemoForm;

public class CopyModelParameterValidator : AbstractValidator<CopyModelParameter>
{
    public CopyModelParameterValidator()
    {
        RuleFor(p => p.SourceModelId).NotEmpty().NotNull();
        RuleFor(p => p.SourceEnvironment).IsInEnum();
        RuleFor(p => p.DestinationEnvironment).IsInEnum();
        RuleFor(p => p.DestinationEnvironment).NotEqual(p => p.SourceEnvironment);
    }
}
