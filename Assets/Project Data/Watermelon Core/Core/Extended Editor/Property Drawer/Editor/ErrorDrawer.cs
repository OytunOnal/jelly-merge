using UnityEditor;

namespace JellyMerge
{
    [CustomPropertyDrawer(typeof(ErrorAttribute))]
    public class ErrorDrawer : HelpBoxDrawer
    {
        protected override MessageType GetMessageType()
        {
            return MessageType.Error;
        }
    }
}
