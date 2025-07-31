using NeeRuler.Interop;


namespace NeeRuler.FollowMouse
{
    public abstract class NativeOffset
    {
        public virtual void OnOpen() { }
        public virtual void OnClosed() { }
        public virtual void OnWindowPositionChanged() { }
        public virtual POINT GetNativeOffset(RECT rect) => default;
    }
}
