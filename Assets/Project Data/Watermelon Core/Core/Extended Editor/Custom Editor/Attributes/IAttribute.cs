using System;

namespace JellyMerge
{
    public interface IAttribute
    {
        Type TargetAttributeType { get; }
    }
}
