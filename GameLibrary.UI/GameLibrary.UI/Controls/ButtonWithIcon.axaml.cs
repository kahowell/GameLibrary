using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using ReactiveUI;

namespace GameLibrary.UI.Controls;

public class ButtonWithIcon : TemplatedControl
{
    public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<ButtonWithIcon, string>(nameof(Icon));

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<ButtonWithIcon, string>(nameof(Text));

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> CommandProperty =
        AvaloniaProperty.Register<ButtonWithIcon, ReactiveCommand<Unit, Unit>>(nameof(Command));

    public static readonly StyledProperty<FlyoutBase?> FlyoutProperty =
        AvaloniaProperty.Register<ButtonWithIcon, FlyoutBase?>(nameof(Flyout));

    public static readonly StyledProperty<RotateTransform?> IconRotateTransformProperty =
        AvaloniaProperty.Register<ButtonWithIcon, RotateTransform?>(nameof(IconRotateTransform));

    public static ReactiveCommand<Unit, Unit> TestCommand { get; } = ReactiveCommand.Create(() => { }, Observable.FromAsync(async() => false));

    public string Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ReactiveCommand<Unit, Unit> Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public FlyoutBase? Flyout
    {
        get => GetValue(FlyoutProperty);
        set => SetValue(FlyoutProperty, value);
    }

    public RotateTransform? IconRotateTransform
    {
        get => GetValue(IconRotateTransformProperty);
        set => SetValue(IconRotateTransformProperty, value);
    }
}
