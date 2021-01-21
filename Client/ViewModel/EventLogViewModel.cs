namespace Pet_Project.ViewModel
{
    using System;
    using System.Windows;
    using System.Text.RegularExpressions;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Events;

    using Common.Network;

    public class EventLogViewModel : BindableBase
    {
        #region Constants

        private const string TIME_FORMAT = @"^(2[0-3]|[01]?[0-9]):([0-5]?[0-9]):([0-5]?[0-9])$";

        #endregion Constants

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

        #endregion Fields

        #region Properties

        public DateTime FirstDate
        {
            get => _firstDate;
            set => SetProperty(ref _firstDate, value);
        }

        public DateTime SecondDate
        {
            get => _secondDate;
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

        public DelegateCommand FilterCommand { get; }

        public DelegateCommand CancelCommand { get; }

        #endregion Properties

        #region Constructors

        public EventLogViewModel(IEventLogController eventLogController, IEventAggregator eventAggregator)
        {
            _eventLogController = eventLogController;
            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<CloseWindowsEventArgs>().Subscribe(HandleCloseEventlog);
            eventAggregator.GetEvent<ChangeStyleEventArgs>().Subscribe(HandleChangeStyle);

            _firstDate = DateTime.Now;
            _secondDate = DateTime.Now;

            FilterCommand = new DelegateCommand(ExecuteFilterCommand);
            CancelCommand = new DelegateCommand(ExecuteCancelCommand);
        }

        #endregion Constructors

        #region Methods

        private void ExecuteFilterCommand()
        {
            string firstTime = $"{FirstDateHours}:{FirstDateMinutes}:{FirstDateSeconds}";
            string secondTime = $"{SecondDateHours}:{SecondDateMinutes}:{SecondDateSeconds}";

            if (Regex.IsMatch(firstTime, TIME_FORMAT) && Regex.IsMatch(secondTime, TIME_FORMAT))
            {
                FirstDate = new DateTime(FirstDate.Year, FirstDate.Month, FirstDate.Day, FirstDateHours, FirstDateMinutes, FirstDateSeconds);
                SecondDate = new DateTime(SecondDate.Year, SecondDate.Month, SecondDate.Day, SecondDateHours, SecondDateMinutes, SecondDateSeconds);

                if (FirstDate <= SecondDate)
                {
                    var selectedTypes = IsEvents ? MessageType.Event : 0;
                    selectedTypes |= IsErrors ? MessageType.Error : 0;

                    _eventLogController.SendFilterRequest(FirstDate, SecondDate, selectedTypes);
                    _eventAggregator.GetEvent<OpenChatEventArgs>().Publish();
                }
                else
                {
                    MessageBox.Show("Incorrect date range!");
                }
            }
            else
            {
                MessageBox.Show("Incorrect time!");
            }
        }

        private void ExecuteCancelCommand()
        {
            _eventAggregator.GetEvent<OpenChatEventArgs>().Publish();
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
        }

        private void HandleChangeStyle(bool isDarkTheme)
        {
            IsDarkTheme = isDarkTheme;
        }

        #endregion Methods
    }
}
