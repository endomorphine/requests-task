using System.Runtime.Serialization;

namespace requests_task.Entities;

public enum Decision
{
    [EnumMember(Value = "Grant")]
    Grant,
    [EnumMember(Value = "Deny")]
    Deny
}
