using System;

namespace NeeLaboratory.Natives
{
    internal static class WindowProcedureExtensions
    {
        public static void AddMouseActivateAndEat(this WindowProcedure windowProcedure)
        {
            windowProcedure.AddHook(WindowMessages.WM_MOUSEACTIVATE, (IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
            {
                handled = true;
                return (IntPtr)MouseActions.MA_ACTIVATEANDEAT;
            });
        }
    }

}
