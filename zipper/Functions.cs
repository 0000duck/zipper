using System;

namespace zipper
{
    public static class Functions
    {
        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue)
           where TEnum : struct
        {
            var result = defaultValue;

            if (!string.IsNullOrEmpty(value) && Enum.TryParse(value.ToUpper(), true, out result))
            {
                result = (TEnum)Enum.Parse(typeof(TEnum), value.ToUpper(), true);
            }

            return result;
        }
    }
}