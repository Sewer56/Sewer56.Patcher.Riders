using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Reloaded.WPF.Animations.Samples;
using Reloaded.WPF.Theme.Default;
using Reloaded.WPF.Utilities;
using Sewer56.Patcher.Riders.Effect.SRDX.Utility;
using Sewer56.Patcher.Riders.Utility;
using Color = System.Windows.Media.Color;

namespace Sewer56.Patcher.Riders.Effect.SRDX;

public class ThemeHueShiftEffect
{
    private CycleColorAnimation _glowColourAnimation;
    private CycleColorAnimation _accentColourAnimation;
    private CycleColorAnimation _accentColourLightAnimation;

    private readonly ReloadedWindow _window;

    private readonly WindowViewModel _windowVm;
    private DictionaryResourceManipulator _resources;
    
    private readonly Stopwatch _watch = Stopwatch.StartNew();
    
    private UpdateThrottler _glowChangeThrottler = new UpdateThrottler(30);
    private UpdateThrottler _colourChangeThrottler = new UpdateThrottler(60);

    private SolidColorBrush _accentColorLightBrush;
    private SolidColorBrush _accentColorBrush;

    public ThemeHueShiftEffect(ReloadedWindow reloadedWindow)
    {
        _window = reloadedWindow;
        _windowVm = _window.ViewModel;
        _resources = new DictionaryResourceManipulator(Application.Current.Resources);

        // Glow
        EnableWindowGlow();
        EnableAccentShift();

        // Update each frame!
        CompositionTargetEx.FrameUpdating += OnRendering;
    }

    private void OnRendering(object sender, EventArgs e)
    {
        var deltaTime = (float) _watch.Elapsed.TotalMilliseconds;

        if (_glowChangeThrottler.Update(deltaTime))
            _glowColourAnimation?.ManualUpdate(_glowChangeThrottler.TargetFrameTime);

        if (_colourChangeThrottler.Update(deltaTime))
        {
            _accentColourLightAnimation?.ManualUpdate(_colourChangeThrottler.TargetFrameTime);
            _accentColourAnimation?.ManualUpdate(_colourChangeThrottler.TargetFrameTime);
        }

        _watch.Restart();
    }

    private void EnableWindowGlow()
    {
        var colour = _resources.Get<Color>("GlowHueCycleColor");
        _glowColourAnimation = new CycleColorAnimation(color =>
        {
            _windowVm.SetGlowColor(color, true);
        }, colour, 6000, 20);

        _windowVm.AllowGlowStateChange = false;
        _windowVm.GlowColorAnimationEnable = false;
    }

    private void EnableAccentShift()
    {
        var colourLight = _resources.Get<Color>("AccentColorLight");

        _accentColorLightBrush = _resources.Get<SolidColorBrush>("AccentColorLightBrush");
        _accentColorBrush = _resources.Get<SolidColorBrush>("AccentColorBrush");
        _accentColourLightAnimation = new CycleColorAnimation(color =>
        {
            // Some elements don't get updated right, it's strange.
            ModifyOrSetNewBrush(ref _accentColorLightBrush, "AccentColorLightBrush", color);
            _resources.Set<Color>("AccentColorLight", color);
        }, colourLight, 6000, 20);

        var colour = _resources.Get<Color>("AccentColor");
        _accentColourAnimation = new CycleColorAnimation(color =>
        {
            ModifyOrSetNewBrush(ref _accentColorBrush, "AccentColorBrush", color);
            _resources.Set<Color>("AccentColor", color);
        }, colour, 6000, 20);
    }

    private void ModifyOrSetNewBrush(ref SolidColorBrush existingBrush, string resourceName, Color color)
    {
        // Changing colour is more efficient than changing brush, but not always possible.
        if (existingBrush.IsFrozen)
        {
            existingBrush = new SolidColorBrush(color);
            _resources.Set(resourceName, existingBrush);
        }
        else
            existingBrush.Color = color;
    }
}