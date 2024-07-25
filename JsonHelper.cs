using BTD_Mod_Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RPGMod
{
    /// <summary>
    /// Provides useful RPG Mod specific methods
    /// </summary>
    public static class JsonHelper
    {
        internal static readonly JsonSerializerSettings Settings = new()
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        public static void SaveRpgGameData(RpgGameData data)
        {
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            if (!data.IgnoreSave)
            {
                if (SandboxFlag || data.ModeName == "Sandbox")
                {
                    ModHelper.Log<RPGMod>("Tried saving data for a sandbox game!");
                }
                else
                {
                    string serializedData = JsonConvert.SerializeObject(data, Settings);
                    string fileName = data.MapName + ".json";

                    string filePath = Path.Combine(SavePath, fileName);
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        File.WriteAllText(filePath, serializedData);
                    }
                    catch (Exception e)
                    {
                        ModHelper.Error<RPGMod>("Error saving " + fileName + "!");
                        ModHelper.Error<RPGMod>(e);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the given <see cref="RpgGameData"/>
        /// </summary>
        /// <param name="dataToSave"><see cref="RpgGameData"/> to save.</param>
        public static void SaveRpgGameData(params RpgGameData[] dataToSave)
        {
            SaveRpgGameData(dataToSave.ToList());
        }
        /// <summary>
        /// Saves the given <see cref="RpgGameData"/>
        /// </summary>
        /// <param name="dataToSave"><see cref="RpgGameData"/> to save.</param>
        public static void SaveRpgGameData(List<RpgGameData> dataToSave)
        {
            foreach (var data in dataToSave)
            {
                SaveRpgGameData(data);
            }
        }

        /// <summary>
        /// Returns an <see cref="RpgGameData"/> loaded with the data from a json file.
        /// Returns null if file doesn't exist or it isn't an RPG Mod save.
        /// </summary>
        /// <param name="fileName">ID of the file to get the data from (Not including path or .json)</param>
        public static RpgGameData? GetSavedRpgGameData(string fileName)
        {
            string filePath = Path.Combine(SavePath, fileName);

            if (!File.Exists(filePath))
            {
                return null;
            }
            else
            {
                var json = File.ReadAllText(filePath); ;

                return JsonConvert.DeserializeObject<RpgGameData>(json);
            }
        }

        /// <summary>
        /// Returns every saved <see cref="RpgGameData"/> in the save path. Returns a list with Dummy Data if the directory isn't found
        /// </summary>
        public static List<RpgGameData> GetSavedRpgGameData()
        {
            if (!Directory.Exists(SavePath))
            {
                return [RpgGameData.DummyData];
            }
            else
            {
                List<RpgGameData> foundGameData = new(Directory.GetFiles(SavePath).Length);

                foreach (var fileName in Directory.GetFiles(SavePath))
                {
                    if (Path.GetExtension(fileName) == ".json")
                    {
                        var data = GetSavedRpgGameData(fileName);

                        if (data != null)
                        { foundGameData.Add(data); }
                    }
                }

                return foundGameData;
            }
        }

        private class RpgUserDataSave
        {
            public List<ItemData> ui = [];
            public Dictionary<string, double> ue1 = new();
            public double ue2 = 0;

            public bool hue2 = false;
            public bool hue1 = false;

            public double uem2 = 0.01;
            public double uem1 = 0.05;
        }

        public static void SaveRpgUserData()
        {
            var json = JsonConvert.SerializeObject(Player, Formatting.Indented);

            if (File.Exists(Path.Combine(SavePath, "Player.player")))
            {
                File.Delete(Path.Combine(SavePath, "Player.player"));
            }

            File.WriteAllText(Path.Combine(SavePath, "Player.player"), json);
        }

        public static void LoadRpgUserData()
        {
            if (File.Exists(Path.Combine(SavePath, "Player.player")))
            {
                var json = File.ReadAllText(Path.Combine(SavePath, "Player.player"));

                var ruds = JsonConvert.DeserializeObject<RpgUserData>(json);

                Player = ruds;
            }
        }
    }
}
