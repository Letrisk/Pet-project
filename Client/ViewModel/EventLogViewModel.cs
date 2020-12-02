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

        private IEventLogController _eventLogController;
        private IEventAggregator _eventAggregator;

        private DateTime _firstDate = DateTime.Now, _secondDate = DateTime.Now;
        private int _firstDateHours, _firstDateMinutes, _firstDateSeconds, _secondDateHours, _secondDateMinutes, _secondDateSeconds;

        private bool _isMessages, _isEvents, _isErrors;
        List<string> _messageTypes;

        private Visibility _eventLogVisibility = Visibility.Collapsed;

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
            set { 
                SetProperty(ref _isMessages, value);
                if (value)
                {
                    _messageTypes.Add(MessageType.Message.ToString());
                }
                else
                {
                    _messageTypes.Remove(MessageType.Message.ToString());
                }
                }
        }

        public bool IsEvents
        {
            get => _isEvents;
            set
            {
                SetProperty(ref _isEvents, value);
                if (value)
                {
                    _messageTypes.Add(MessageType.Event.ToString());
                }
                else
                {
                    _messageTypes.Remove(MessageType.Event.ToString());
                }
            }
        }

        public bool IsErrors
        {
            get => _isErrors;
            set
            {
                SetProperty(ref _isErrors, value);
                if (value)
                {
                    _messageTypes.Add(MessageType.Error.ToString());
                }
                else
                {
                    _messageTypes.Remove(MessageType.Error.ToString());
                }
            }
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

            FilterCommand = new DelegateCommand(ExecuteFilterCommand);
            CancelCommand = new DelegateCommand(ExecuteCancelCommand);

            _messageTypes = new List<string>();
        }

        #endregion Constructors

        #region Methods

        private void ExecuteFilterCommand()
        {
            _eventLogController.SendFilterRequest(FirstDate, SecondDate, _messageTypes.ToArray());
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

        #endregion Methods
    }
}
