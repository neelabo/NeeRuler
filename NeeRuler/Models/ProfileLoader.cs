using System;
using System.Diagnostics;
using System.IO;

namespace NeeRuler.Models
{
    public static class ProfileLoader
    {
        public static Profile? Load(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return JsonConverter.Deserialize<Profile>(path);
                }
            }
            catch (Exception ex)
            {
                NotificationBox.ShowFailedToLoadFile(path, ex.Message, NotificationType.Error);
            }
            return null;
        }

        public static void Save(Profile profile, string path)
        {
            try
            {
                var newFile = path + ".new";
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }
                if (File.Exists(path))
                {
                    JsonConverter.Serialize(profile, newFile);
                    File.Replace(newFile, path, null);
                }
                else
                {
                    JsonConverter.Serialize(profile, path);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

    }
}