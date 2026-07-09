using backend.Dtos;
using backend.Models;
using FluentValidation;

namespace backend.Validators;

public class CriarTransacaoDtoValidator : AbstractValidator<CriarTransacaoDto>
{
    public CriarTransacaoDtoValidator()
    {
        RuleFor(x => x.Descricao).NotEmpty().WithMessage("A descrição é obrigatória.");
        RuleFor(x => x.Valor).GreaterThan(0).WithMessage("O valor deve ser maior que zero.");
        RuleFor(x => x.Tipo).Must(TipoTransacao.IsValido).WithMessage("O tipo deve ser 'Despesa' ou 'Receita'.");
        RuleFor(x => x.PessoaId).GreaterThan(0).WithMessage("Pessoa inválida.");
    }
}
