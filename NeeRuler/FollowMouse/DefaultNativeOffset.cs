using NeeRuler.Interop;


namespace NeeRuler.FollowMouse
{
    public class DefaultNativeOffset : NativeOffset
    {
        public override POINT GetNativeOffset(RECT rect)
        {
            return default;
        }
    }
}
