using FluentValidation;

namespace DemoForm;

public class DeleteModelParameterValidator : AbstractValidator<DeleteModelParameter>
{
    public DeleteModelParameterValidator()
    {
        RuleFor(p => p.ModelId).NotNull().NotEmpty();
        RuleFor(p => p.Environment).IsInEnum();
    }
}
