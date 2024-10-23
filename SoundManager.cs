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
    public enum SoundType
    {
        Win,
        PieceMove,
        Turn,
        DiceRoll,
        Error
    }

    public static class SoundManager
    {
        public static bool SoundEnabled { get; set; } = true;
        private static readonly Dictionary<SoundType, string> soundMap;
        private static readonly MediaPlayer mediaPlayer;

        // initialize soundmap and mediaplayer
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

        // method to play audio with soundtype
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
