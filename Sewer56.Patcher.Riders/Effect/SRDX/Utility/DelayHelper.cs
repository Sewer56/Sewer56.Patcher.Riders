namespace Sewer56.Patcher.Riders.Effect.SRDX.Utility;

public struct DelayHelper
{
    /// <summary>
    /// Time to wait until item will return true.
    /// </summary>
    public float Time { get; private set; }

    /// <summary>
    /// Time currently elapsed.
    /// </summary>
    public float TimeElapsed { get; private set; }

    public DelayHelper(float time)
    {
        Time = time;
    }

    /// <summary>
    /// Updates the stored time, returns true if hit over maximum.
    /// </summary>
    public bool Update(float deltaTime)
    {
        TimeElapsed += deltaTime;
        return TimeElapsed > Time;
    }

    /// <summary>
    /// Resets the state.
    /// </summary>
    public void Reset()
    {
        TimeElapsed = 0;
    }
}