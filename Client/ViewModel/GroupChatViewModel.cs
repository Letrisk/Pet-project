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

    public class GroupChatViewModel : BindableBase
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly IGroupChatController _groupChatController;

        private ObservableCollection<string> _clients;
        private ObservableCollection<string> _groupClients;

        private string _currentTarget;
        private string _deleteCurrentTarget;
        private string _groupName;

        private bool _isApplyEnable;
        private bool _isDarkTheme;

        private Visibility _groupChatVisibility;

        #endregion Fields

        #region Properties

        public ObservableCollection<string> Clients
        {
            get => _clients;
            set => SetProperty(ref _clients, value);
        }

        public ObservableCollection<string> GroupClients
        {
            get => _groupClients;
            set
            {
                SetProperty(ref _groupClients, value);
                IsApplyEnable = _groupClients.Count != 0;
            }
        }

        public string CurrentTarget
        {
            get => _currentTarget;
            set
            {
                SetProperty(ref _currentTarget, value);
                if (!_groupClients.Contains(value))
                {
                    _groupClients.Add(value);
                    _clients.Remove(value);
                }
                if (_groupClients.Count != 0)
                {
                    IsApplyEnable = true;
                }
            }
        }

        public string DeleteCurrentTarget
        {
            get => _deleteCurrentTarget;
            set 
            { 
                SetProperty(ref _deleteCurrentTarget, value);
                if (!_clients.Contains(value))
                {
                    _clients.Add(value);
                    _groupClients.Remove(value);
                }
                if(_groupClients.Count == 0)
                {
                    IsApplyEnable = false;
                }
            }
        }

        public string GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);
        }

        public bool IsApplyEnable
        {
            get => _isApplyEnable;
            set => SetProperty(ref _isApplyEnable, value);
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set => SetProperty(ref _isDarkTheme, value);
        }

        public Visibility GroupChatVisibility
        {
            get => _groupChatVisibility;
            set => SetProperty(ref _groupChatVisibility, value);
        }

        public DelegateCommand GroupChatCommand { get; }

        public DelegateCommand CancelCommand { get; }

        #endregion Properties

        #region Constructors

        public GroupChatViewModel(IGroupChatController groupChatController, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<OpenGroupChatEventArgs>().Subscribe(HandleOpenGroupChat);
            eventAggregator.GetEvent<CloseWindowsEventArgs>().Subscribe(HandleCloseGroupChat);
            eventAggregator.GetEvent<ChangeStyleEventArgs>().Subscribe(HandleChangeStyle);

            _groupChatController = groupChatController;

            _clients = new ObservableCollection<string>();
            _groupClients = new ObservableCollection<string>();
            _isApplyEnable = false;
            _groupChatVisibility = Visibility.Collapsed;

            GroupChatCommand = new DelegateCommand(ExecuteGroupChatCommand);
            CancelCommand = new DelegateCommand(ExecuteCancelCommand);
            
        }

        #endregion Constructors

        #region Methods

        private void ExecuteGroupChatCommand()
        {
            _groupChatController.CreateGroupRequest(_groupName, _groupClients.ToList());
            _eventAggregator.GetEvent<OpenChatEventArgs>().Publish();
            GroupChatVisibility = Visibility.Collapsed;
        }

        private void ExecuteCancelCommand()
        {
            _eventAggregator.GetEvent<OpenChatEventArgs>().Publish();
            GroupChatVisibility = Visibility.Collapsed;
        }

        private void HandleOpenGroupChat(ObservableCollection<string> clients)
        {
            Clients = clients;
            GroupChatVisibility = Visibility.Visible;
        }

        private void HandleCloseGroupChat()
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                Clients.Clear();
                GroupClients.Clear();
            });
            GroupName = String.Empty;
            IsApplyEnable = false;
            GroupChatVisibility = Visibility.Collapsed;
        }

        private void HandleChangeStyle(bool isDarkTheme)
        {
            IsDarkTheme = isDarkTheme;
        }

        #endregion Methods
    }
}
