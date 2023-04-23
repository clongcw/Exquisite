using System.Windows;
using System.Windows.Media;

namespace Exquisite.UI.Utils;

public static class WpfUtils
{
    public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child != null && child is T) return (T)child;

            var childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null) return childOfChild;
        }

        return null;
    }
}