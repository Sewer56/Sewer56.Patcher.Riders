using System;
using ManagedBass;

namespace Sewer56.Patcher.Riders.Utility;

public class BassMusicPlayer
{
    private static bool _canUseBass;

    static BassMusicPlayer()
    {
        _canUseBass = Bass.Init();
        Bass.GlobalMusicVolume = 5000;
    }

    private int _bassHandle;
    private bool _isPlaying;

    public BassMusicPlayer(string musicPath)
    {
        if (!_canUseBass)
            return;

        _bassHandle = Bass.MusicLoad(musicPath, 0, 0, BassFlags.Loop);
    }

    public void Play()
    {
        if (!_canUseBass)
            return;
        
        Bass.ChannelPlay(_bassHandle);
        _isPlaying = true;
    }

    public void Pause()
    {
        if (!_canUseBass)
            return;

        Bass.ChannelPause(_bassHandle);
        _isPlaying = false;
    }

    public void Toggle()
    {
        if (_isPlaying)
            Pause();
        else
            Play();
    }
}