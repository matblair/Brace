using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace Brace.Utils
{
    public class SoundManager
    {
        private static SoundManager soundManager;

        public enum SoundsEnum {
            Rain,
            Thunder1,
            Thunder2,
            Thunder3
        };

        private Dictionary<SoundsEnum, SoundEffect> sounds;

        public static void Initialise()
        {
            soundManager = new SoundManager();
        }

        public static SoundManager GetCurrent()
        {
            if (soundManager == null)
            {
                Initialise();
            }
            return soundManager;
        }

        private SoundManager()
        {
            sounds = new Dictionary<SoundsEnum, SoundEffect>();
            sounds.Add(SoundsEnum.Rain, new SoundEffect("Assets/Audio/rain.wav", true));
            sounds.Add(SoundsEnum.Thunder1, new SoundEffect("Assets/Audio/thunder1.wav", false));
            sounds.Add(SoundsEnum.Thunder2, new SoundEffect("Assets/Audio/thunder2.wav", false));
            sounds.Add(SoundsEnum.Thunder3, new SoundEffect("Assets/Audio/thunder3.wav", false));
        }

        public void PlaySound(SoundsEnum sound)
        {
            sounds[sound].Play();
        }
    }
}
