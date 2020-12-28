namespace Pet_Project.ViewModel
{
    using System.Collections.ObjectModel;

    using Prism.Mvvm;
    using Prism.Events;

    using Common.Network;
    using View;

    public class MainWindowViewModel : BindableBase
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private readonly object _connectionView;
        private readonly object _chatView;
        private readonly object _eventLogView;
        private readonly object _groupChatView;

        private object _selectedView;

        #endregion Fields

        #region Properties

        public object SelectedView
        {
            get => _selectedView;
            set => SetProperty(ref _selectedView, value);
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

            _selectedView = _connectionView;

            _eventAggregator.GetEvent<OpenChatEventArgs>().Subscribe(OpenChatView);
            _eventAggregator.GetEvent<OpenGroupChatEventArgs>().Subscribe(OpenGroupChatView);
            _eventAggregator.GetEvent<OpenEventLogEventArgs>().Subscribe(OpenEventLogView);
            _eventAggregator.GetEvent<CloseWindowsEventArgs>().Subscribe(OpenConnectionView);
        }

        #endregion Constructors

        #region Methods

        private void OpenConnectionView()
        {
            SelectedView = _connectionView;
        }

        private void OpenChatView()
        {
            SelectedView = _chatView;           
        }

        private void OpenGroupChatView(ObservableCollection<Client> clients)
        {
            SelectedView = _groupChatView;
        }

        private void OpenEventLogView()
        {
            SelectedView = _eventLogView;
        }

        #endregion Methods
    }
}
