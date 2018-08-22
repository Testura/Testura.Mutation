using System.Runtime.Serialization;

namespace Cama.Core
{
    public enum MutationOperators
    {
        [EnumMember(Value = "Replace boundary conditionals(<, <=, >, >=) with their counterpart. ")]
        ConditionalBoundary,

        [EnumMember(Value = "Replace conditionals(==, !=) with their counterpart.")]
        NegateCondtional,

        [EnumMember(Value = "Replace binary arithmetic operations(+, -, /, %, <, >, &) with their counterpart")]
        Math,

        [EnumMember(Value = "Replace type compatibility checks (\"x is y\") with \"!(x is y)\"")]
        NegateTypeCompability,

        [EnumMember(Value = "Replace post increment and decrement with their counterparts.")]
        Increment,

        [EnumMember(Value = "Replace return values of methods or properties.")]
        ReturnValue
    }
}
