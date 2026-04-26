using HostelHub.Application.Features.Hostels.Commands.OnboardHostel;
using FluentValidation.TestHelper;
using Xunit;

namespace HostelHub.Tests.Unit.Features.Hostels.Commands.OnboardHostel;

public class OnboardEnterpriseHostelCommandValidatorTests
{
    private readonly OnboardEnterpriseHostelCommandValidator _validator;

    public OnboardEnterpriseHostelCommandValidatorTests()
    {
        _validator = new OnboardEnterpriseHostelCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_City_Is_Empty()
    {
        var command = new OnboardEnterpriseHostelCommand { City = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    [Fact]
    public void Should_Have_Error_When_GSTIN_Is_Empty()
    {
        var command = new OnboardEnterpriseHostelCommand { GSTIN = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.GSTIN);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var command = new OnboardEnterpriseHostelCommand 
        { 
            Name = "Valid Hostel",
            City = "Hyderabad", 
            GSTIN = "22AAAAA0000A1Z5",
            Street = "Test Street",
            ZipCode = "500001"
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
