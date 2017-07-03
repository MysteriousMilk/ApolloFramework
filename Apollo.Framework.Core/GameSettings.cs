using System.Collections.Generic;

namespace Apollo.Framework.Core
{
    public class GameSettings
    {
        public Dictionary<string, string> SettingsMap;

        public GameSettings()
        {
            SettingsMap = new Dictionary<string, string>();
        }

        public void SetValue(string settingName, string settingValue)
        {
            SettingsMap.Remove(settingName);
            SettingsMap.Add(settingName, settingValue);
        }
        
        public string GetValue(string settingName)
        {
            SettingsMap.TryGetValue(settingName, out string value);
            return value;
        }

        public static GameSettings CreateDefaultSettings()
        {
            GameSettings settings = new GameSettings();

            settings.SetValue("ResolutionX", "1280");
            settings.SetValue("ResolutionY", "720");
            settings.SetValue("IsFullScreen", "False");

            return settings;
        }
    }
}
