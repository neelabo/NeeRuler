using NeeRuler.Interop;


namespace NeeRuler.FollowMouse
{
    public class CenterNativeOffset : NativeOffset
    {
        public override POINT GetNativeOffset(RECT rect)
        {
            POINT offset = new();
            offset.X = rect.Width / 2;
            offset.Y = rect.Height / 2;
            return offset;
        }
    }
}
