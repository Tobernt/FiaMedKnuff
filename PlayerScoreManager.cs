using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Windows.Storage;

namespace FiaMedKnuff
{
    /// <summary>
    /// Represents a player's score in the game.
    /// </summary>
    public class PlayerScore
    {
        public string Name { get; set; }
        public int Moves { get; set; }
        public string Time {  get; set; }
    }

    /// <summary>
    /// Manages the saving and loading of player scores.
    /// </summary>
    public static class PlayerScoreManager
    {
        private static readonly string FilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "highscores.json");

        /// <summary>
        /// Appends a player's score to the high score file.
        /// </summary>
        /// <param name="playerScore">The player score to save.</param>
        public static void SavePlayerScore(PlayerScore playerScore)
        {
            List<PlayerScore> playerScores = LoadAllPlayerScores();

            playerScores.Add(playerScore);

            string json = JsonSerializer.Serialize(playerScores, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        /// <summary>
        /// Loads the top 10 player scores sorted by the number of moves (ascending).
        /// </summary>
        /// <returns>A list of the top 10 <see cref="PlayerScore"/> objects.</returns>
        public static List<PlayerScore> LoadTopPlayerScores()
        {
            return LoadAllPlayerScores().OrderBy(ps => ps.Moves).Take(10).ToList();
        }

        /// <summary>
        /// Loads all player scores from the high score file.
        /// </summary>
        /// <returns>A list of all <see cref="PlayerScore"/> objects. If the file does not exist, returns an empty list.</returns>
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