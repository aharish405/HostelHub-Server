using FluentValidation;

namespace HostelHub.Application.Features.Hostels.Commands.OnboardHostel;

public class OnboardEnterpriseHostelCommandValidator : AbstractValidator<OnboardEnterpriseHostelCommand>
{
    public OnboardEnterpriseHostelCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
        RuleFor(x => x.GSTIN).NotEmpty().MaximumLength(20);
        RuleFor(x => x.BasePrivatePrice).GreaterThan(0);
        RuleFor(x => x.BaseDormPrice).GreaterThan(0);
        
        RuleFor(x => x.Floors).NotEmpty().WithMessage("At least one floor must be defined.");
    }
}
