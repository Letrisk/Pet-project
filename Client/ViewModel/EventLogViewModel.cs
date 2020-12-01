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

        private IEventAggregator _eventAggregator;

        private DateTime _firstDate, _secondDate;

        private bool _isMessages, _isEvents, _isErrors;

        private Visibility _eventLogVisibility = Visibility.Collapsed;

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

        public Visibility EventLogVisibility
        {
            get => _eventLogVisibility;
            set => SetProperty(ref _eventLogVisibility, value);
        }

        public DelegateCommand FilterCommand { get; }

        #endregion Properties

        #region Constructors

        public EventLogViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<OpenEventLogEventArgs>().Subscribe(HandleOpenEventLog);

            FilterCommand = new DelegateCommand(ExecuteFilterCommand);
        }

        #endregion Constructors

        #region Methods

        private void ExecuteFilterCommand()
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
