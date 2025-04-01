using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace User.Utils.Helper;

public static class EnumConversionHelper
{
    public static ValueConverter<TEnum, string> GetEnumConverter<TEnum>() where TEnum : struct, Enum
    {
        return new ValueConverter<TEnum, string>(
            v => v.ToString(),
            v => (TEnum)Enum.Parse(typeof(TEnum), v)
        );
    }
}