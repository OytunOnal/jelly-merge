using System.Reflection;

namespace JellyMerge
{
    public abstract class NativePropertyDrawer
    {
        public abstract void DrawNativeProperty(UnityEngine.Object target, PropertyInfo property);
    }
}