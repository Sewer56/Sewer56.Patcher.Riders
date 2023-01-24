using Sewer56.Patcher.Riders.Effect.Utility;

namespace Sewer56.Patcher.Riders.Effect.TitleEffectStates;

public struct NextLevelSince2017State : ITitleEffectState
{
    private RandomFillTextEffectHelper _titleTyper = new RandomFillTextEffectHelper("Pushing Riders to The Next Level Since 2017", 100, 'x');
    private DelayHelper _nextStateDelayer = new DelayHelper(4000);

    public NextLevelSince2017State() { }

    public bool Update(TitleDXv2Effect effect, float deltaTime)
    {
        if (_titleTyper.Update(deltaTime, out var reachedEnd, out string newString))
            effect.TitleBlock1.Text = newString;

        if (!reachedEnd)
            return false;

        return _nextStateDelayer.Update(deltaTime);
    }

    public bool Init(TitleDXv2Effect effect)
    {
        _nextStateDelayer.Reset();
        _titleTyper.Reset();
        return true;
    }

    public bool Exit(TitleDXv2Effect effect)
    {
        return true;
    }
}