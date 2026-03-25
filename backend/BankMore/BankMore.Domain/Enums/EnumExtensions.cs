using System.ComponentModel;
using System.Reflection;

namespace BankMore.Domain.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description ?? value.ToString();
        }

        public static T ToEnum<T>(this string value) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Valor não pode ser vazio");

            if (Enum.TryParse<T>(value, true, out var result))
                return result;

            throw new ArgumentException($"Valor '{value}' inválido para {typeof(T).Name}");
        }
    }
}
