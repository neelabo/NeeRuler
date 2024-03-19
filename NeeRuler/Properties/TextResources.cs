using NeeLaboratory.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NeeRuler.Properties
{
    /// <summary>
    /// NeeRuler.Properties.Resources に代わるテキストリソース
    /// </summary>
    internal class TextResources
    {
        static TextResources()
        {
            LanguageResource.AddCulture(new CultureInfo("ja"));
        }


        public static CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        public static AppLanguageResource LanguageResource { get; } = new("/Resources/Languages/");

        public static TextResourceManager Resource { get; } = new(LanguageResource);


        public static string GetString(string name)
        {
            return Resource.GetString(name) ?? "@" + name;
        }

        public static string? GetStringRaw(string name)
        {
            return Resource.GetString(name);
        }

        public static string? GetStringRaw(string name, CultureInfo culture)
        {
            return Resource.GetString(name, culture);
        }
    }
}
