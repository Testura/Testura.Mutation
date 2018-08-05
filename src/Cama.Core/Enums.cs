using System.Runtime.Serialization;

namespace Cama.Core
{
    public enum MutationOperators
    {
        [EnumMember(Value = "If conditional operator change")]
        IfConditional,

        SA
    }
}
