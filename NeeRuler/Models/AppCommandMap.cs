using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using NeeLaboratory.Windows.Input;

namespace NeeRuler.Models
{
    public class AppCommandMap : CommandMap
    {
        private readonly AppContext _appContext;
        private readonly string _keyConfigFilePath;

        public AppCommandMap(AppContext appContext, ReadingRuler ruler)
        {
            _appContext = appContext;
            _keyConfigFilePath = Path.Combine(_appContext.ProfileDirectory, "KeyConfig.json");

            AddCommand(new StoreLocationCommand(ruler), "Shift+Q");
            AddCommand(new RestoreLocationCommand(ruler), "Q");

            AddCommand(new ToggleIsVerticalCommand(ruler), "V");
            AddCommand(new ToggleIsAutoStrideCommand(ruler), "E");
            AddCommand(new ToggleIsFlatPanelCommand(ruler), "F");
            AddCommand(new ToggleIsFollowMouseCommand(ruler));

            AddCommand(new MoveUpCommand(ruler), "W,Up");
            AddCommand(new MoveDownCommand(ruler), "S,Down");
            AddCommand(new MoveLeftCommand(ruler), "A,Left");
            AddCommand(new MoveRightCommand(ruler), "D,Right");

            AddCommand(new AdjustUpCommand(ruler), "Shift+W,Shift+Up");
            AddCommand(new AdjustDownCommand(ruler), "Shift+S,Shift+Down");
            AddCommand(new AdjustLeftCommand(ruler), "Shift+A,Shift+Left");
            AddCommand(new AdjustRightCommand(ruler), "Shift+D,Shift+Right");

            AddCommand(new ExitCommand(ruler));
            AddCommand(new OpenProfilesFolderCommand(ruler));

            ruler.UpdateProfileIds();
            foreach (var id in ruler.ProfileIds)
            {
                AddCommand(new ProfileCommand(ruler, id));
            }

            ////SaveKeyConfig();
        }

        private void AddCommand(ReadingRulerCommand command)
        {
            AddCommand(command.Name, command);
        }

        private void AddCommand(ReadingRulerCommand command, string gestures)
        {
            AddCommand(command.Name, command, gestures);
        }


        public void SaveKeyConfig()
        {
            JsonConverter.Serialize(CreateKeyConfig(), _keyConfigFilePath);
        }

        public void LoadKeyConfig()
        {
            try
            {
                if (File.Exists(_keyConfigFilePath))
                {
                    var keyConfig = JsonConverter.Deserialize<Dictionary<string, string>>(_keyConfigFilePath);
                    SetKeyConfig(keyConfig);
                }
            }
            catch (Exception ex)
            {
                NotificationBox.ShowFailedToLoadFile(_keyConfigFilePath, ex.Message, NotificationType.Error);
            }
        }

        private Dictionary<string, string> CreateKeyConfig()
        {
            return Commands.ToDictionary(e => e.Key, e => string.Join(",", e.Value.InputGestures.Select(e => TypeDescriptor.GetConverter(e.GetType()).ConvertToString(e))));
        }

        public void SetKeyConfig(Dictionary<string, string> keyConfig)
        {
            ClearInputGesture();

            foreach (var pair in keyConfig)
            {
                SetInputGesture(pair.Key, pair.Value);
            }
        }

    }

}