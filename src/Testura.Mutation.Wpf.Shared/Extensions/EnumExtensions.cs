using System;
using System.Runtime.Serialization;

namespace Testura.Mutation.Wpf.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static string GetValue(this Enum enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);
            return (attributes.Length > 0) ? ((EnumMemberAttribute)attributes[0]).Value : null;
        }
    }
}
