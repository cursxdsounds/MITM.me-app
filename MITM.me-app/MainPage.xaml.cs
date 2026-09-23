using MITM.me_app.ViewModels;

namespace MITM.me_app;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        
        InitializeComponent();
        
        var chat = new ChatViewModel();
        BindingContext = chat;
        // ItemsUpdatingScrollMode="KeepLastItemInView" fix
        chat.Messages.CollectionChanged += (s, e) =>
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add) 
                return;
            
            var newItem = e.NewItems?[0];
            if (newItem == null) 
                return;

            MainThread.BeginInvokeOnMainThread(async void () =>
            {
                await Task.Delay(50); 
                
                ChatCollectionView.ScrollTo(newItem, position: ScrollToPosition.End, animate: true);
            });
        };
        
        Task.Run(async () =>
        {
            await chat.Initialize();
        });
    }
    
    private void OnCompleted(object? sender, EventArgs e)
    {
        if (sender is InputView input && BindingContext is ChatViewModel vm)
            switch (input.ClassId)
            {
                case "1":
                    vm.SendCommand.ExecuteAsync(1);
                    break;
                case "2":
                    vm.SendCommand.ExecuteAsync(2);
                    break;
            }
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var text = e.NewTextValue ?? string.Empty;
        if (!text.EndsWith("\n", StringComparison.Ordinal)) return;

        if (sender is InputView input && BindingContext is ChatViewModel vm)
            switch (input.ClassId)
            {
                case "1":
                    vm.InputText = text.TrimEnd('\r', '\n');
                    if (vm.SendCommand.CanExecute(1))
                        vm.SendCommand.ExecuteAsync(1);
                    break;
                case "2":
                    vm.InputText2 = text.TrimEnd('\r', '\n');
                    if (vm.SendCommand.CanExecute(2))
                        vm.SendCommand.ExecuteAsync(2);
                    break;
            }
    }
}