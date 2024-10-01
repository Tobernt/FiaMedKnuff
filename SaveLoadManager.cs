using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace FiaMedKnuff
{
    public class SaveLoadManager
    {
        public async void SaveGame(GameState state)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            StorageFile saveFile = await localFolder.CreateFileAsync("savegame.json", CreationCollisionOption.ReplaceExisting);
            using (Stream stream = await saveFile.OpenStreamForWriteAsync())
            {
                var serializer = new DataContractJsonSerializer(typeof(GameState));
                serializer.WriteObject(stream, state);
            }
        }

        public async Task<GameState> LoadGame()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            StorageFile saveFile = await localFolder.GetFileAsync("savegame.json");
            using (Stream stream = await saveFile.OpenStreamForReadAsync())
            {
                var serializer = new DataContractJsonSerializer(typeof(GameState));
                return (GameState)serializer.ReadObject(stream);
            }
        }
    }

    public class GameState
    {
        // Define the game state to save (players, positions, etc.)
    }
}
