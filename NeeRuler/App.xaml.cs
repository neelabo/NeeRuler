using NeeLaboratory.Natives;
using System;
using System.Configuration;
using System.Data;
using System.Windows;

namespace NeeRuler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // DLL 検索パスから現在の作業ディレクトリ (CWD) を削除
            NativeMethods.SetDllDirectory("");

            try
            {
                AppContext.Current.EnsureProfileDirectory();
                AppContext.Current.LoadTextResource();
                AppContext.Current.LoadSettings();
            }
            catch (Exception ex)
            {
                NotificationBox.Show(ex.Message, NotificationType.Error);
                throw;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AppContext.Current.SaveSettings();
        }
    }

}