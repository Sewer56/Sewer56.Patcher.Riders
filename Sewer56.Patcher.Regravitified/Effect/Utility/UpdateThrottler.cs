using System.Runtime.CompilerServices;

namespace Sewer56.Patcher.Riders.Effect.Utility;

public struct UpdateThrottler
{
    /// <summary>
    /// Maximum FPS allowed by this throttler.
    /// </summary>
    public float MaxFps 
    { 
        get => 1000 / TargetFrameTime; 
        set => TargetFrameTime = 1000 / value;
    }

    /// <summary>
    /// Target Frame Time.
    /// </summary>
    public float TargetFrameTime { get; set; }
    
    private float _currentFrameTime;

    public UpdateThrottler(float maxFps) => MaxFps = maxFps;

    /// <summary>
    /// Updates the current throttler state.
    /// </summary>
    /// <returns>True if the state of the throttled item is to be updated, else false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public bool Update(float deltaTime)
    {
        _currentFrameTime += deltaTime;
        if (_currentFrameTime > TargetFrameTime)
        {
            _currentFrameTime -= TargetFrameTime;
            return true;
        }

        return false;
    }
}