using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Windows.Storage;

namespace FiaMedKnuff
{
    public class PlayerScore
    {
        public string Name { get; set; }
        public int Moves { get; set; }
    }

    public static class PlayerScoreManager
    {
        private static readonly string FilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "highscores.json");

        // method to append playerscore to highscore file
        public static void SavePlayerScore(PlayerScore playerScore)
        {
            List<PlayerScore> playerScores = LoadAllPlayerScores();

            playerScores.Add(playerScore);

            string json = JsonSerializer.Serialize(playerScores, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        // return top 10 PlayerScores sorted by moves (ascending)
        public static List<PlayerScore> LoadTopPlayerScores()
        {
            List<PlayerScore> playerScores = LoadAllPlayerScores();
            return LoadAllPlayerScores().OrderBy(ps => ps.Moves).Take(10).ToList();
        }

        public static List<PlayerScore> LoadAllPlayerScores()
        {
            if (!File.Exists(FilePath))
            {
                return new List<PlayerScore>();
            }

            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<PlayerScore>>(json) ?? new List<PlayerScore>();
        }
    }
}