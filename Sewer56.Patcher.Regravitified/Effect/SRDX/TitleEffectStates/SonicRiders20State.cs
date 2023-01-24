using Sewer56.Patcher.Riders.Effect.SRDX.Utility;

namespace Sewer56.Patcher.Riders.Effect.SRDX.TitleEffectStates;

public struct SonicRiders20State : ITitleEffectState
{
    private BlinkEffectHelper _blinkEffect = new BlinkEffectHelper("Sonic Riders DX 2.0", 1200, 800);
    private DelayHelper _nextStateDelayer = new DelayHelper(6000);

    public SonicRiders20State()
    {
    }

    public bool Update(TitleDXv2Effect effect, float deltaTime)
    {
        if (_blinkEffect.Update(deltaTime, out var newText))
            effect.TitleBlock1.Text = newText;

        return _nextStateDelayer.Update(deltaTime);
    }

    public bool Init(TitleDXv2Effect effect)
    {
        effect.TitleBlock1.Text = "";
        _blinkEffect.Reset();
        _nextStateDelayer.Reset();
        return true;
    }

    public bool Exit(TitleDXv2Effect effect) => true;
}