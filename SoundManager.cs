using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FiaMedKnuff.UserControls.Sound_Button;
using Windows.Media.Core;
using Windows.Media.Playback;
using System.Diagnostics;

namespace FiaMedKnuff
{
    /// <summary>
    /// Represents the different types of sounds that can be played in the game.
    /// </summary>
    public enum SoundType
    {
        Win,
        PieceMove,
        Turn,
        DiceRoll,
        Error
    }

    /// <summary>
    /// Manages sound playback for the game.
    /// </summary>
    public static class SoundManager
    {
        public static bool SoundEnabled { get; set; } = true;
        private static readonly Dictionary<SoundType, string> soundMap;
        private static readonly MediaPlayer mediaPlayer;

        /// <summary>
        /// Initializes the <see cref="SoundManager"/> class, setting up the sound map and media player.
        /// </summary>
        static SoundManager()
        {
            mediaPlayer = new MediaPlayer();
            soundMap = new Dictionary<SoundType, string>
            {
                { SoundType.Win, "ms-appx:///Assets/game_win.wav" },
                { SoundType.PieceMove, "ms-appx:///Assets/game_piece_place.wav" },
                { SoundType.Turn, "ms-appx:///Assets/game_turn.wav" },
                { SoundType.DiceRoll, "ms-appx:///Assets/game_dice_rolling.wav" },
                { SoundType.Error, "ms-appx:///Assets/game_pling.wav" }
            };
        }

        /// <summary>
        /// Plays the specified sound based on the <see cref="SoundType"/>.
        /// </summary>
        /// <param name="soundType">The type of sound to play.</param>
        public static void PlaySound(SoundType soundType)
        {
            if (!SoundEnabled)
                return;

            if (soundMap.TryGetValue(soundType, out string soundUri))
            {
                mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(soundUri));
                mediaPlayer.Play();
            }
        }
    }
}
