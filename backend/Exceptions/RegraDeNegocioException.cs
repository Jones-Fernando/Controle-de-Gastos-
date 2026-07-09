using System;

namespace backend.Exceptions
{
    /// <summary>
    /// Exceção para representar violações de regras de negócio que devem
    /// resultar em respostas HTTP 4xx controladas (ex: 422 Unprocessable Entity).
    /// </summary>
    public class RegraDeNegocioException : Exception
    {
        public RegraDeNegocioException(string message) : base(message) { }
    }
}
