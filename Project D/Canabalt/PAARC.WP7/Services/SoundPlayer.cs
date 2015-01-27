using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PAARC.WP7.Services
{
    public static class SoundPlayer
    {
        public const string SelectSound = "Content/Sounds/Select.wav";
        public const string DeselectSound = "Content/Sounds/Deselect.wav";

        public static void Play(string sound)
        {
            Stream stream = TitleContainer.OpenStream(sound);
            SoundEffect effect = SoundEffect.FromStream(stream);
            FrameworkDispatcher.Update();
            effect.Play();
        }
    }
}
