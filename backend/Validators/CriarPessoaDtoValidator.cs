using backend.Dtos;
using FluentValidation;

namespace backend.Validators;

public class CriarPessoaDtoValidator : AbstractValidator<CriarPessoaDto>
{
    public CriarPessoaDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.Idade).GreaterThanOrEqualTo(0).WithMessage("A idade não pode ser negativa.");
    }
}
