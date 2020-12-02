namespace Pet_Project
{
    using System.Windows;

    using Prism.Ioc;
    using Prism.Mvvm;
    using Prism.Unity;

    using Unity;

    using Common;
    using Common.Network;
    using View;
    using ViewModel;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IConnectionController, ConnectionController>();
            containerRegistry.RegisterSingleton<IChatController, ChatController>();
            containerRegistry.RegisterSingleton<IEventLogController, EventLogController>();
            containerRegistry.Register<ConnectionViewModel>();
            containerRegistry.Register<ChatViewModel>();
            containerRegistry.Register<EventLogViewModel>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            BindViewModelToView<ConnectionViewModel, ConnectionView>();
            BindViewModelToView<ChatViewModel, ChatView>();
            BindViewModelToView<EventLogViewModel, EventLogView>();
        }

        protected override Window CreateShell()
        {
            var mainView = Container.Resolve<MainWindow>();
            return mainView;
        }

        private void BindViewModelToView<TViewModel, TView>()
        {
            ViewModelLocationProvider.Register(typeof(TView).ToString(), () => Container.Resolve<TViewModel>());
        }
    }
}
