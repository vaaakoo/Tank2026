using System.Media;
using System.Threading.Tasks;

namespace Tank2026.Audio;

public static class SoundManager
{
    // Need multiple players to prevent cutting off sounds if we want to play
    // overlapping effects, but System.Media.SoundPlayer represents an exclusive
    // single sound channel for Windows. For retro accuracy, sound interrupting is fine.
    private static readonly SoundPlayer _player = new SoundPlayer();

    public static void PlayShoot()
    {
        Task.Run(() => 
        {
            try
            {
                using var stream = RetroAudio.GenerateShootSound();
                var sound = new SoundPlayer(stream);
                sound.Play();
            }
            catch { }
        });
    }

    public static void PlayExplosion()
    {
        Task.Run(() => 
        {
            try
            {
                using var stream = RetroAudio.GenerateExplosionSound();
                var sound = new SoundPlayer(stream);
                sound.Play();
            }
            catch { }
        });
    }

    public static void PlayPowerup()
    {
        Task.Run(() => 
        {
            try
            {
                using var stream = RetroAudio.GeneratePowerupSound();
                var sound = new SoundPlayer(stream);
                sound.Play();
            }
            catch { }
        });
    }
}
