using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RPGMod.Octokit
{
    internal class Players
    {
        [JsonIgnore]
        public static string PAT => "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";

        [JsonIgnore]
        private static GitHubClient GlobalClient = new(new ProductHeaderValue("BTD6.RPGMod.DarkTerraYT"))
        {
            Credentials = new(PAT)
        };

        public List<RpgUserData> RPGPlayers = [];

        public static Players instance;

        public void UpdatePlayers()
        {

        }
    }
}
