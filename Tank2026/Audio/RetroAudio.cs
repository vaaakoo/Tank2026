using System;
using System.IO;

namespace Tank2026.Audio;

public static class RetroAudio
{
    private const int SampleRate = 44100;

    public static Stream GenerateShootSound()
    {
        var durationMs = 100;
        var frequency = 880.0;
        var samples = new byte[SampleRate * durationMs / 1000];
        
        for (int i = 0; i < samples.Length; i++)
        {
            double t = (double)i / SampleRate;
            // High pitch descending square wave
            frequency -= 800.0 / samples.Length; 
            var wave = Math.Sign(Math.Sin(2 * Math.PI * frequency * t));
            samples[i] = (byte)(wave * 64 + 128); // 8-bit PCM (0-255)
        }
        
        return WrapInWavStream(samples);
    }

    public static Stream GenerateExplosionSound()
    {
        var durationMs = 250;
        var samples = new byte[SampleRate * durationMs / 1000];
        var random = new Random();
        
        for (int i = 0; i < samples.Length; i++)
        {
            var progress = 1.0 - ((double)i / samples.Length);
            var noise = (random.NextDouble() * 2 - 1) * progress; // fading noise
            samples[i] = (byte)(noise * 64 + 128);
        }
        
        return WrapInWavStream(samples);
    }

    public static Stream GeneratePowerupSound()
    {
        var durationMs = 150;
        var frequency = 440.0;
        var samples = new byte[SampleRate * durationMs / 1000];
        
        for (int i = 0; i < samples.Length; i++)
        {
            double t = (double)i / SampleRate;
            // Ascending triangle wave
            frequency += 1200.0 / samples.Length;
            var wave = 2.0 * Math.Abs(2.0 * (t * frequency - Math.Floor(t * frequency + 0.5))) - 1.0;
            samples[i] = (byte)(wave * 64 + 128);
        }
        
        return WrapInWavStream(samples);
    }

    private static MemoryStream WrapInWavStream(byte[] samples)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        
        // RIFF header
        writer.Write(new[] { 'R', 'I', 'F', 'F' });
        writer.Write(36 + samples.Length);
        writer.Write(new[] { 'W', 'A', 'V', 'E' });
        
        // fmt chunk
        writer.Write(new[] { 'f', 'm', 't', ' ' });
        writer.Write(16); // Subchunk size
        writer.Write((short)1); // Audio format (PCM)
        writer.Write((short)1); // Num channels
        writer.Write(SampleRate);
        writer.Write(SampleRate); // Byte rate (SampleRate * 1 * 8 / 8)
        writer.Write((short)1); // Block align
        writer.Write((short)8); // Bits per sample
        
        // data chunk
        writer.Write(new[] { 'd', 'a', 't', 'a' });
        writer.Write(samples.Length);
        writer.Write(samples);
        
        stream.Position = 0;
        return stream;
    }
}
