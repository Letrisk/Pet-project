namespace Pet_Project.ViewModel
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Threading;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Events;

    using Common.Network;
    using View;
    using Common.Network.Messages;

    public class MainWindowViewModel : BindableBase
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private readonly object _connectionView;
        private readonly object _chatView;
        private readonly object _eventLogView;
        private readonly object _groupChatView;

        private object _selectedViewModel;

        #endregion Fields

        #region Properties

        public object SelectedViewModel
        {
            get => _selectedViewModel;
            set => SetProperty(ref _selectedViewModel, value);
        }

        #endregion Properties

        #region Constructors

        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _connectionView = new ConnectionView();
            _chatView = new ChatView();
            _groupChatView = new GroupChatView();
            _eventLogView = new EventLogView();

            _selectedViewModel = _connectionView;

            _eventAggregator.GetEvent<OpenChatEventArgs>().Subscribe(OpenChat);
            _eventAggregator.GetEvent<OpenGroupChatEventArgs>().Subscribe(OpenGroupChat);
            _eventAggregator.GetEvent<OpenEventLogEventArgs>().Subscribe(OpenEventLog);
            _eventAggregator.GetEvent<CloseWindowsEventArgs>().Subscribe(OpenConnectionView);
        }

        #endregion Constructors

        #region Methods

        private void OpenConnectionView()
        {
            SelectedViewModel = _connectionView;
        }

        private void OpenChat()
        {
            SelectedViewModel = _chatView;           
        }

        private void OpenGroupChat(ObservableCollection<string> clients)
        {
            SelectedViewModel = _groupChatView;
        }

        private void OpenEventLog()
        {
            SelectedViewModel = _eventLogView;
        }

        #endregion Methods
    }
}
