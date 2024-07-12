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
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            foreach (var data in dataToSave)
            {
                if (!data.IgnoreSave)
                {
                    if (SandboxFlag && PreventSandboxSaving)
                    {
                        ModHelper.Log<RPGMod>("Tried saving data for a sandbox game!");
                    }
                    else
                    {
                        string serializedData = JsonConvert.SerializeObject(data, Settings);
                        string fileName = data.MapName + "_" + data.ModeName + "_" + data.Difficuly + ".json";

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
                    var data = GetSavedRpgGameData(fileName);

                    if (data != null)
                    { foundGameData.Add(data); }
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
            var ruds = new RpgUserDataSave()
            {
                ui = RpgUserData.UniversalItems,
                hue1 = RpgUserData.HasUnitedXP,
                hue2 = RpgUserData.HasUniversalXP,
                ue1 = RpgUserData.UnitedXP,
                ue2 = RpgUserData.UniversalXP,
                uem1 = RpgUserData.UnitedXPMutliper,
                uem2 = RpgUserData.UniversalXPMutliper
            };

            var json = JsonConvert.SerializeObject(ruds, Formatting.Indented);

            if (File.Exists(Path.Combine(SavePath, "Rud.json")))
            {
                File.Delete(Path.Combine(SavePath, "Rud.json"));
            }

            File.WriteAllText(Path.Combine(SavePath, "Rud.json"), json);
        }

        public static void LoadRpgUserData()
        {
            if (File.Exists(Path.Combine(SavePath, "Rud.json")))
            {
                var json = File.ReadAllText(Path.Combine(SavePath, "Rud.json"));

                var ruds = JsonConvert.DeserializeObject<RpgUserDataSave>(json);

                RpgUserData.UniversalXPMutliper = ruds.uem2;
                RpgUserData.UnitedXPMutliper = ruds.uem1;
                RpgUserData.HasUnitedXP = ruds.hue1;
                RpgUserData.HasUniversalXP = ruds.hue2;
                RpgUserData.UnitedXP = ruds.ue1;
                RpgUserData.SetUniversalXP(ruds.ue2);
                RpgUserData.UniversalItems = ruds.ui;
            }
        }
    }
}
