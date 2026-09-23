using Microsoft.Extensions.DependencyInjection;
using MITM.me_app.ViewModels;

namespace MITM.me_app;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());

        window.Width = 500;
        window.Height = 800;

        window.MinimumWidth = 350;
        window.MinimumHeight = 600;

        return window;
    }
}