using System.Numerics;
using System.Windows;
using System.Windows.Media;

namespace Sewer56.Patcher.Riders.Effect.SRDX.Utility;

public struct ScrollEffectHelper
{

    /// <summary>
    /// Duration of the effect in milliseconds.
    /// </summary>
    public float DurationMs { get; private set; }

    /// <summary>
    /// Whether to go left to right, or light to left.
    /// </summary>
    public bool LeftToRight { get; private set; } = true;

    /// <summary>
    /// The element affected by this effect.
    /// </summary>
    public FrameworkElement Element { get; private set; }

    /// <summary>
    /// Time that has elapsed since start of the effect.
    /// </summary>
    public float TimeElapsed { get; private set; }

    private HorizontalAlignment _originalAlignment;
    private Transform _originalTransform;

    private Size _elementSize;
    private TranslateTransform _translation = new TranslateTransform();
    
    private Vector2 _initialTransform = Vector2.Zero;
    private Vector2 _finalransform = Vector2.Zero;

    private readonly float? _horizontalDisplacement;
    private readonly float _initialTransformDisplacement;
    private float? _velocity;

    public ScrollEffectHelper(float durationMs, bool leftToRight, FrameworkElement element, float initialTransformDisplacement, float? horizontalDisplacement = null, float ? velocity = null)
    {
        _horizontalDisplacement = horizontalDisplacement;
        _initialTransformDisplacement = initialTransformDisplacement;
        _velocity = velocity;
        DurationMs = durationMs;
        LeftToRight = leftToRight;
        Element = element;
    }

    /// <summary>
    /// Initializes the effect for showing.
    /// </summary>
    public void Init()
    {
        _originalAlignment = Element.HorizontalAlignment;
        _originalTransform = Element.RenderTransform;

        Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        _elementSize = Element.DesiredSize;

        if (LeftToRight)
        {
            Element.HorizontalAlignment = HorizontalAlignment.Left;
            _initialTransform = new Vector2((float)-_elementSize.Width - _initialTransformDisplacement, 0);

            if (_horizontalDisplacement.HasValue)
                _finalransform = new Vector2((float)_horizontalDisplacement, 0);
            
            if (_velocity.HasValue)
                _velocity = _velocity.Value * -1;
        }
        else
        {
            Element.HorizontalAlignment = HorizontalAlignment.Right;
            _initialTransform = new Vector2((float)(_elementSize.Width + _initialTransformDisplacement), 0);

            if (_horizontalDisplacement.HasValue)
                _finalransform = new Vector2((float)-_horizontalDisplacement, 0);
        }

        _translation = new TranslateTransform(_initialTransform.X, _initialTransform.Y);
        Element.RenderTransform = _translation;
    }

    /// <summary>
    /// Updates the current scroll helper
    /// </summary>
    /// <returns>True if scrolling has finished, else false.</returns>
    public bool Update(float deltaTime)
    {
        TimeElapsed += deltaTime;
        var ratio = TimeElapsed / DurationMs;
        if (ratio > 1)
            return true;

        // Set new translation.
        if (!_velocity.HasValue)
        {
            var translation = Vector2.Lerp(_initialTransform, _finalransform, ratio);
            _translation.X = translation.X;
            _translation.Y = translation.Y;
        }
        else
        {
            _translation.X -= (_velocity.Value * deltaTime);
        }

        return false;
    }

    public void Reset()
    {
        TimeElapsed = 0;
    }

    /// <summary>
    /// Restores the original framework element settings from before when this helper was created.
    /// </summary>
    public void Dispose()
    {
        Element.HorizontalAlignment = _originalAlignment;
        Element.RenderTransform = _originalTransform;
    }
}