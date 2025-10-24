using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Sewer56.Patcher.Riders.Effect.SRDX.Utility;

#nullable enable
public class WpfUtilities
{
    /// <summary>
    /// Finds child of an object that satisfies the given condition.
    /// </summary>
    /// <param name="item">Current item in the recursion hierarchy.</param>
    /// <param name="filter">Filter used to select the correct element.</param>
    public static DependencyObject? FindChild(DependencyObject item, Func<DependencyObject, bool> filter) => FindChild(item, item, filter);

    /// <summary>
    /// Finds child of an object that satisfies the given condition.
    /// </summary>
    /// <param name="parent">The parent/root from which we are searching from.</param>
    /// <param name="item">Current item in the recursion hierarchy.</param>
    /// <param name="filter">Filter used to select the correct element.</param>
    public static DependencyObject? FindChild(DependencyObject parent, DependencyObject item, Func<DependencyObject, bool> filter)
    {
        var children = VisualTreeHelper.GetChildrenCount(item);
        for (int x = 0; x < children; x++)
        {
            var child = VisualTreeHelper.GetChild(item, x);
            if (child == null!)
                continue;

            if (filter(child))
                return child;

            var recursiveResult = FindChild(parent, child, filter);
            if (recursiveResult != null)
                return recursiveResult;
        }

        return null;
    }

    /// <summary>
    /// Gets the key name for a dynamicresource assigned to an object.
    /// </summary>
    public static string? GetDynamicResourceKey(DependencyObject item, DependencyProperty property)
    {
        try
        {
            var value = item.ReadLocalValue(property);
            var converter = new ResourceReferenceExpressionConverter();
            var dynamicResource = converter.ConvertTo(value, typeof(MarkupExtension)) as DynamicResourceExtension;
            return dynamicResource?.ResourceKey as string;
        }
        catch (Exception)
        {
            return String.Empty;
        }
    }
}
#nullable disable