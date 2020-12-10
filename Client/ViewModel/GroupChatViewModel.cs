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

        private IEventAggregator _eventAggregator;
        private IGroupChatController _groupChatController;

        private ObservableCollection<string> _clients = new ObservableCollection<string>();
        private ObservableCollection<string> _groupClients = new ObservableCollection<string>();

        private string _currentTarget, _deleteCurrentTarget, _groupName;

        private bool _isApplyEnable = false;

        private Visibility _groupChatVisibility = Visibility.Collapsed;

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
                if(_groupClients.Count!=0)
                {
                    IsApplyEnable = true;
                }
                else
                {
                    IsApplyEnable = false;
                }
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

            _groupChatController = groupChatController;

            GroupChatCommand = new DelegateCommand(ExecuteGroupChatCommand);
            CancelCommand = new DelegateCommand(ExecuteCancelCommand);
            
        }

        #endregion Constructors

        #region Methods

        private void ExecuteGroupChatCommand()
        {
            _groupClients.Add(_groupChatController.Login);
            _groupChatController.SendCreateGroupRequest(_groupName, _groupClients.ToList());
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
            GroupChatVisibility = Visibility.Collapsed;
        }

        #endregion Methods
    }
}
