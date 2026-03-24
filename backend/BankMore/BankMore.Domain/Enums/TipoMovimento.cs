using System.ComponentModel;
using System.Reflection;

namespace BankMore.Domain.Enums
{
    public enum TipoMovimento
    {
        [Description("C")]
        Credito,

        [Description("D")]
        Debito
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description ?? value.ToString();
        }
    }
}
