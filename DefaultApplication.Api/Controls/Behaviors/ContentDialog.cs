using Avalonia;
using Avalonia.Data;

namespace DefaultApplication.Controls.Behaviors;

public sealed class ContentDialog : AvaloniaObject
{
    public static readonly AttachedProperty<bool> IsFullScreenProperty = AvaloniaProperty.RegisterAttached<ContentDialog, AvaloniaObject, bool>(
      "IsFullScreen", false, false, BindingMode.OneWay);

    public static void SetIsFullScreen(AvaloniaObject? element, bool value) => element?.SetValue(IsFullScreenProperty, value);

    public static bool GetIsFullScreen(AvaloniaObject? element) => element?.GetValue(IsFullScreenProperty) is bool value && value;

    public static readonly AttachedProperty<object?> NoneContentProperty = AvaloniaProperty.RegisterAttached<ContentDialog, AvaloniaObject, object?>(
      "NoneContent", null, false, BindingMode.OneWay);

    public static void SetNoneContent(AvaloniaObject? element, object? value) => element?.SetValue(NoneContentProperty, value);

    public static object? GetNoneContent(AvaloniaObject? element) => element?.GetValue(NoneContentProperty);

    public static readonly AttachedProperty<object?> PrimaryContentProperty = AvaloniaProperty.RegisterAttached<ContentDialog, AvaloniaObject, object?>(
      "PrimaryContent", null, false, BindingMode.OneWay);

    public static void SetPrimaryContent(AvaloniaObject? element, object? value) => element?.SetValue(PrimaryContentProperty, value);

    public static object? GetPrimaryContent(AvaloniaObject? element) => element?.GetValue(PrimaryContentProperty);

    public static readonly AttachedProperty<bool> CanReturnPrimaryProperty = AvaloniaProperty.RegisterAttached<ContentDialog, AvaloniaObject, bool>(
      "CanReturnPrimary", true, false, BindingMode.OneWay);

    public static void SetCanReturnPrimary(AvaloniaObject? element, bool value) => element?.SetValue(CanReturnPrimaryProperty, value);

    public static bool GetCanReturnPrimary(AvaloniaObject? element) => element?.GetValue(CanReturnPrimaryProperty) is bool value && value;

    public static readonly AttachedProperty<object?> SecondaryContentProperty = AvaloniaProperty.RegisterAttached<ContentDialog, AvaloniaObject, object?>(
      "SecondaryContent", null, false, BindingMode.OneWay);

    public static void SetSecondaryContent(AvaloniaObject? element, object? value) => element?.SetValue(SecondaryContentProperty, value);

    public static object? GetSecondaryContent(AvaloniaObject? element) => element?.GetValue(SecondaryContentProperty);

    public static readonly AttachedProperty<bool> CanReturnSecondaryProperty = AvaloniaProperty.RegisterAttached<ContentDialog, AvaloniaObject, bool>(
      "CanReturnSecondary", true, false, BindingMode.OneWay);

    public static void SetCanReturnSecondary(AvaloniaObject? element, bool value) => element?.SetValue(CanReturnSecondaryProperty, value);

    public static bool GetCanReturnSecondary(AvaloniaObject? element) => element?.GetValue(CanReturnSecondaryProperty) is bool value && value;
}
