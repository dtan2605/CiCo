using FluentValidation;
using cico.Application.Features.Employees.Commands.CreateEmployee;

namespace cico.Application.Validators.Employee;

public class CreateEmployeeValidator
    : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.EmployeeCode)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^(0|\+84)[0-9]{9,10}$");

        RuleFor(x => x.DepartmentId)
            .NotEmpty();

        RuleFor(x => x.PositionId)
            .NotEmpty();
    }
}