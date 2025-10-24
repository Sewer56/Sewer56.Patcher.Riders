using System.Windows;
using Sewer56.Patcher.Riders.Effect.SRDX.Utility;

namespace Sewer56.Patcher.Riders.Effect.SRDX.TitleEffectStates;

public struct DisplayReleaseNotesState : ITitleEffectState
{
    private string _text  = "Thank you to everyone who supported us throughout these years <3";
    private string _text2 = "We hope you enjoy this release. Riders is all about fun!";

    private ScrollEffectHelper _scrollFirstMessageHelper;
    private ScrollEffectHelper _scrollSecondMessageHelper;
    private DelayHelper _secondMessageDelayHelper;

    private DelayHelper _blinkEndHelper;
    private BlinkEffectHelper _blinkSecondEffect;

    private FrameworkElement _viewBoxOne;
    private FrameworkElement _viewBoxTwo;

    public DisplayReleaseNotesState(FrameworkElement viewBoxOne, FrameworkElement viewBoxTwo)
    {
        _viewBoxOne = viewBoxOne;
        _viewBoxTwo = viewBoxTwo;
        _secondMessageDelayHelper = new DelayHelper(5800);
        _blinkEndHelper = new DelayHelper(6000);
        _blinkSecondEffect = new BlinkEffectHelper(_text2, 1200, 800, true);
    }

    public bool Update(TitleDXv2Effect effect, float deltaTime)
    {
        effect.TitleBlock1.Visibility = Visibility.Visible;
        bool firstComplete = _scrollFirstMessageHelper.Update(deltaTime);

        // Delay for 2nd message.
        if (!_secondMessageDelayHelper.Update(deltaTime)) 
            return false;

        // Show second message.
        effect.TitleBlock2.Visibility = Visibility.Visible;
        if (!_scrollSecondMessageHelper.Update(deltaTime)) 
            return false;

        // Blink second message.
        if (_blinkSecondEffect.Update(deltaTime, out var newSecondText))
            effect.TitleBlock2.Text = newSecondText;

        bool blinkSecondComplete = _blinkEndHelper.Update(deltaTime);
        return firstComplete && blinkSecondComplete;
    }

    public bool Init(TitleDXv2Effect effect)
    {
        effect.TitleBlock1.Text = _text;
        effect.TitleBlock2.Text = _text2;
        _scrollFirstMessageHelper  = new ScrollEffectHelper(14000, true, _viewBoxOne, 50, null, 0.1f);
        _scrollSecondMessageHelper = new ScrollEffectHelper(6250, true, _viewBoxTwo, 50, null, 0.1f);
        _scrollFirstMessageHelper.Init();
        _scrollSecondMessageHelper.Init();
        _secondMessageDelayHelper.Reset();
        effect.TitleBlock1.Visibility = Visibility.Hidden;
        effect.TitleBlock2.Visibility = Visibility.Hidden;
        return true;
    }

    public bool Exit(TitleDXv2Effect effect)
    {
        effect.TitleBlock1.Text = "";
        effect.TitleBlock2.Text = "";
        _scrollFirstMessageHelper.Dispose();
        _scrollSecondMessageHelper.Dispose();
        return true;
    }
}