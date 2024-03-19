using NeeRuler.Models;
using NeeRuler.Properties;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace NeeRuler
{
    public class AppContext
    {
        public static AppContext Current { get; }
        static AppContext() { Current = new AppContext(); }

        private AppContext()
        {
            AssemblyLocation = Assembly.GetEntryAssembly().Location;

            PackageType = ConfigurationManager.AppSettings["PackageType"];

            UseSharedAppData = string.Compare(ConfigurationManager.AppSettings["UseSharedAppData"], "True", true) == 0;

            ProfileDirectory = GetProfileDirectory();
            SettingsFilePath = Path.Combine(ProfileDirectory, "Settings.json");

            Trace.WriteLine($"AssemblyLocation: {AssemblyLocation}");
            Trace.WriteLine($"PackageType: {PackageType}");
            Trace.WriteLine($"UseSharedAppData: {UseSharedAppData}");
            Trace.WriteLine($"ProfileDirectory: {ProfileDirectory}");
        }


        public string CompanyName { get; } = "NeeLaboratory";

        public static string ProductName { get; } = "NeeRuler";

        public string AssemblyLocation { get; }

        public bool UseSharedAppData { get; }

        public string PackageType { get; }

        public string ProfileDirectory { get; private set; } = "";

        public string SettingsFilePath { get; }

        public Profile? Settings { get; set; }


        public bool IsDevPackage => PackageType == ".dev";
        public bool IsZipPackage => PackageType == ".zip";
        public bool IsMsiPackage => PackageType == ".msi";
        public bool IsAppxPackage => PackageType == ".appx";
        public bool IsCanaryPackage => PackageType == ".canary";
        public bool IsBetaPackage => PackageType == ".beta";




        private string GetApplicationDataPath()
        {
            if (UseSharedAppData)
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), CompanyName, ProductName);
                EnsureDirectory(path);
                return path;
            }
            else
            {
                return Path.GetDirectoryName(AssemblyLocation);
            }
        }

        private string EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }


        private string GetProfileDirectory()
        {
            var directory = GetApplicationDataPath();

            var path = ConfigurationManager.AppSettings["ProfilesDirectory"] ?? "Profiles";
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                return Path.Combine(directory, path);
            }
        }

        private string GetOriginalProfileDirectory()
        {
            return Path.Combine(Path.GetDirectoryName(AssemblyLocation), "Profiles");
        }

        public void EnsureProfileDirectory()
        {
            if (!UseSharedAppData) return;
            if (Directory.Exists(ProfileDirectory)) return;

            CopyDirectory(GetOriginalProfileDirectory(), ProfileDirectory, true);
        }

        // from https://learn.microsoft.com/ja-jp/dotnet/standard/io/how-to-copy-directories
        private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public void LoadTextResource()
        {
            var culture = ConfigurationManager.AppSettings["Culture"];
            TextResources.Culture = string.IsNullOrWhiteSpace(culture) ? CultureInfo.CurrentCulture : CultureInfo.GetCultureInfo(culture);
            TextResources.Resource.Load(TextResources.Culture);
        }

        public void LoadSettings()
        {
            try
            {
                var profile = ProfileLoader.Load(SettingsFilePath);
                if (profile is not null)
                {
                    Settings = profile;
                }
            }
            catch (Exception ex)
            {
                NotificationBox.ShowFailedToLoadFile(SettingsFilePath, ex.Message, NotificationType.Error);
            }
        }

        public void SaveSettings()
        {
            if (Settings is not null)
            {
                ProfileLoader.Save(Settings, SettingsFilePath);
            }
        }
    }



}