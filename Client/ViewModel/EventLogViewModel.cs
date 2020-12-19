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

    public class EventLogViewModel : BindableBase
    {
        #region Fields

        private readonly IEventLogController _eventLogController;
        private readonly IEventAggregator _eventAggregator;

        private DateTime _firstDate;
        private DateTime _secondDate;
        private int _firstDateHours;
        private int _firstDateMinutes;
        private int _firstDateSeconds;
        private int _secondDateHours;
        private int _secondDateMinutes;
        private int _secondDateSeconds;

        private bool _isMessages;
        private bool _isEvents;
        private bool _isErrors;
        private bool _isDarkTheme;

        private Visibility _eventLogVisibility;

        #endregion Fields

        #region Properties

        public DateTime FirstDate
        {
            get => new DateTime(_firstDate.Year, _firstDate.Month, _firstDate.Day,
                                _firstDateHours, _firstDateMinutes, _firstDateSeconds);
            set =>SetProperty(ref _firstDate, value);
        }

        public DateTime SecondDate
        {
            get => new DateTime(_secondDate.Year, _secondDate.Month, _secondDate.Day,
                                _secondDateHours, _secondDateMinutes, _secondDateSeconds);
            set => SetProperty(ref _secondDate, value);
        }

        public int FirstDateHours
        {
            get => _firstDateHours;
            set => SetProperty(ref _firstDateHours, value);
        }

        public int FirstDateMinutes
        {
            get => _firstDateMinutes;
            set => SetProperty(ref _firstDateMinutes, value);
        }

        public int FirstDateSeconds
        {
            get => _firstDateSeconds;
            set => SetProperty(ref _firstDateSeconds, value);
        }

        public int SecondDateHours
        {
            get => _secondDateHours;
            set => SetProperty(ref _secondDateHours, value);
        }

        public int SecondDateMinutes
        {
            get => _secondDateMinutes;
            set => SetProperty(ref _secondDateMinutes, value);
        }

        public int SecondDateSeconds
        {
            get => _secondDateSeconds;
            set => SetProperty(ref _secondDateSeconds, value);
        }

        public bool IsMessages
        {
            get => _isMessages;
            set => SetProperty(ref _isMessages, value);
        }

        public bool IsEvents
        {
            get => _isEvents;
            set => SetProperty(ref _isEvents, value);
        }

        public bool IsErrors
        {
            get => _isErrors;
            set => SetProperty(ref _isErrors, value);
        }
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set => SetProperty(ref _isDarkTheme, value);
        }

        public Visibility EventLogVisibility
        {
            get => _eventLogVisibility;
            set => SetProperty(ref _eventLogVisibility, value);
        }

        public DelegateCommand FilterCommand { get; }

        public DelegateCommand CancelCommand { get; }

        #endregion Properties

        #region Constructors

        public EventLogViewModel(IEventLogController eventLogController, IEventAggregator eventAggregator)
        {
            _eventLogController = eventLogController;
            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<OpenEventLogEventArgs>().Subscribe(HandleOpenEventLog);
            eventAggregator.GetEvent<CloseWindowsEventArgs>().Subscribe(HandleCloseEventlog);
            eventAggregator.GetEvent<ChangeStyleEventArgs>().Subscribe(HandleChangeStyle);

            _firstDate = DateTime.Now;
            _secondDate = DateTime.Now;
            _eventLogVisibility = Visibility.Collapsed;

            FilterCommand = new DelegateCommand(ExecuteFilterCommand);
            CancelCommand = new DelegateCommand(ExecuteCancelCommand);
        }

        #endregion Constructors

        #region Methods

        private void ExecuteFilterCommand()
        {
            var selectedTypes = IsMessages ? MessageType.Message : 0;
            selectedTypes |= IsEvents ? MessageType.Event : 0;
            selectedTypes |= IsErrors ? MessageType.Error : 0;

            _eventLogController.SendFilterRequest(FirstDate, SecondDate, selectedTypes);
            _eventAggregator.GetEvent<OpenChatEventArgs>().Publish();
            EventLogVisibility = Visibility.Collapsed;
        }

        private void ExecuteCancelCommand()
        {
            _eventAggregator.GetEvent<OpenChatEventArgs>().Publish();
            EventLogVisibility = Visibility.Collapsed;
        }

        private void HandleOpenEventLog()
        {
            EventLogVisibility = Visibility.Visible;
        }

        private void HandleCloseEventlog()
        {
            FirstDateHours = 0;
            FirstDateMinutes = 0;
            FirstDateSeconds = 0;
            SecondDateHours = 0;
            SecondDateMinutes = 0;
            SecondDateSeconds = 0;
            IsMessages = false;
            IsEvents = false;
            IsErrors = false;
            EventLogVisibility = Visibility.Collapsed;
        }

        private void HandleChangeStyle(bool isDarkTheme)
        {
            IsDarkTheme = isDarkTheme;
        }

        #endregion Methods
    }
}
