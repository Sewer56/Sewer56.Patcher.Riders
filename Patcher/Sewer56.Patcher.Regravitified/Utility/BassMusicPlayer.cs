using System;
using ManagedBass;

namespace Sewer56.Patcher.Riders.Utility;

public class BassMusicPlayer
{
    static BassMusicPlayer()
    {
        if (!Bass.Init())
            throw new Exception("Failed to Init BASS");

        Bass.GlobalMusicVolume = 5000;
    }

    private int _bassHandle;

    public BassMusicPlayer(string musicPath) => _bassHandle = Bass.MusicLoad(musicPath, 0, 0, BassFlags.Loop);

    public void Play() => Bass.ChannelPlay(_bassHandle);
}