using System;
using System.IO;
using System.Windows.Forms;
using WMPLib;

namespace Game_Project.Managers
{
    /// <summary>
    /// Single Responsibility: plays and manages all audio during a fight.
    /// Encapsulates WMPLib usage so no other class depends on it directly.
    /// </summary>
    public class SoundManager : IDisposable
    {
        private WindowsMediaPlayer _stageMusic;
        private WindowsMediaPlayer _resultSound;

        public void StartStageMusic()
        {
            string path = Path.Combine(Application.StartupPath, "Assets", "Audio", "stageMusic.mp3");
            if (!File.Exists(path)) return;

            _stageMusic = new WindowsMediaPlayer();
            _stageMusic.URL = path;
            _stageMusic.settings.setMode("loop", true);
            _stageMusic.controls.play();
        }

        public void StopStageMusic()
        {
            _stageMusic?.controls.stop();
            _stageMusic?.close();
            _stageMusic = null;
        }

        /// <param name="isDraw">True plays a draw sound; false plays the KO sound.</param>
        public void PlayResultSound(bool isDraw)
        {
            string file     = isDraw ? Path.Combine("Assets", "Audio", "draw.mp3") : Path.Combine("Assets", "Audio", "ko.mp3");
            string fullPath = Path.Combine(Application.StartupPath, file);
            if (!File.Exists(fullPath)) return;

            _resultSound = new WindowsMediaPlayer();
            _resultSound.URL = fullPath;
            _resultSound.controls.play();
        }

        public void StopResultSound()
        {
            _resultSound?.controls.stop();
            _resultSound?.close();
            _resultSound = null;
        }

        public void Dispose()
        {
            StopStageMusic();
            StopResultSound();
        }
    }
}
