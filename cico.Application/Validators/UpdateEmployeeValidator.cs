using FluentValidation;
using cico.Application.Features.Employees.Commands.UpdateEmployee;

namespace cico.Application.Validators.Employee;

public class UpdateEmployeeValidator
    : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^(0|\+84)[0-9]{9,10}$");
    }
}