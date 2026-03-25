using System.ComponentModel;

namespace BankMore.Domain.Enums
{
    public enum TipoMovimento
    {
        [Description("C")]
        Credito,

        [Description("D")]
        Debito
    }
}
