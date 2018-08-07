using System.Runtime.Serialization;

namespace Cama.Core
{
    public enum MutationOperators
    {
        [EnumMember(Value = "Replace boundary conditionals like <, <=, >, >= with their counterparts. ")]
        ConditionalBoundary,

        [EnumMember(Value = "Negate all conditionals by replacing for exaple == with !=")]
        NegateCondtional,

        [EnumMember(Value = "Replace binary arithmetic operations")]
        Math,

        [EnumMember(Value = "Negate all type compatibility checks (\"x is y\") by replacing it with \"!(x is y)\"")]
        NegateTypeCompability
    }
}
