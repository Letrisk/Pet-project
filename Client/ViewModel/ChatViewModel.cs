namespace Pet_Project.ViewModel
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows.Media;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Events;

    using Common.Network;
    using Common.Network.Messages;

    public class ChatViewModel : BindableBase
    {
        #region Fields

        private Timer _timer;

        private IChatController _chatController;
        private IEventAggregator _eventAggregator;

        private ObservableCollection<string> _clientsList = new ObservableCollection<string>();
        private ObservableCollection<string> _onlineClients = new ObservableCollection<string>();
        private ObservableCollection<string> _offlineClients = new ObservableCollection<string>();
        private ObservableCollection<string> _groupsList = new ObservableCollection<string>();
 
        private Dictionary<string, string> _chats = new Dictionary<string, string>();

        private string _currentMessage, _chatMessages, _currentTarget, _connectionParametres;

        private bool _isDarkTheme;

        private Visibility _chatVisibility = Visibility.Collapsed;

        #endregion Fields

        #region Properties
        public int counter = 0;

        public Visibility ChatVisibility
        {
            get => _chatVisibility;
            set => SetProperty(ref _chatVisibility, value);
        }

        public ObservableCollection<string> ClientsList
        {
            get => _clientsList;
            set => SetProperty(ref _clientsList, value);
        }

        public ObservableCollection<string> OnlineClients
        {
            get => _onlineClients;
            set => SetProperty(ref _onlineClients, value);
        }

        public ObservableCollection<string> OfflineClients
        {
            get => _offlineClients;
            set => SetProperty(ref _offlineClients, value);
        }

        public ObservableCollection<string> GroupsList
        {
            get => _groupsList;
            set => SetProperty(ref _groupsList, value);
        }

        public Dictionary<string, string> Chats
        {
            get => _chats;
            set => SetProperty(ref _chats, value);
        }

        public string ChatMessages
        {
            get => _chatMessages;
            set => SetProperty(ref _chatMessages, value);
        }

        public string CurrentMessage
        {
            get => _currentMessage;
            set => SetProperty(ref _currentMessage, value);
        }

        public string CurrentTarget
        {
            get => _currentTarget;
            set
            {
                
                if (value == null)
                {
                    SetProperty(ref _currentTarget, "General");
                    return;
                }
                else
                {
                    SetProperty(ref _currentTarget, value);
                }

                if (!_chats.ContainsKey(_currentTarget))
                {
                    _chats.Add(_currentTarget, String.Empty);
                }

                ChatMessages = _chats[_currentTarget];
            }
        }

        public string ConnectionParametres
        {
            get => _connectionParametres;
            set => SetProperty(ref _connectionParametres, value);
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set => SetProperty(ref _isDarkTheme, value);
        }

        public DelegateCommand SendCommand { get; }

        public DelegateCommand StopCommand { get; }

        public DelegateCommand OpenEventLogCommand { get; }

        public DelegateCommand OpenGroupChatCommand { get; }

        public DelegateCommand ChangeStyleCommand { get; }

        public DelegateCommand LeaveGroupCommand { get; }

        #endregion Properties

        #region Constructors

        public ChatViewModel(IChatController chatController, IEventAggregator eventAggregator)
        {
            _timer = new Timer();
            _timer.AutoReset = true;
            _timer.Interval = 1;
            _timer.Elapsed += OnTimerEvent;

            _chatController = chatController;
            _eventAggregator = eventAggregator;

            eventAggregator.GetEvent<OpenChatEventArgs>().Subscribe(HandleOpenChat);

            SendCommand = new DelegateCommand(ExecuteSendCommand);
            StopCommand = new DelegateCommand(ExecuteStopCommand);
            OpenEventLogCommand = new DelegateCommand(ExecuteOpenEventLogCommand);
            OpenGroupChatCommand = new DelegateCommand(ExecuteOpenGroupChatCommand);
            ChangeStyleCommand = new DelegateCommand(ExecuteChangeStyleCommand);
            LeaveGroupCommand = new DelegateCommand(ExecuteLeaveGroupCommand);

            _chatController.ConnectionStateChanged += HandleConnectionStateChanged;
        }

        #endregion Constructors

        #region Methods

        private void OnTimerEvent(object sender, ElapsedEventArgs e)
        {
            _chatController?.Send(_chatController.Login, String.Empty, counter++.ToString());
        }

        private void ExecuteSendCommand()
        {
            if (CurrentTarget == "General")
            {
                _chatController?.Send(_chatController.Login, String.Empty, CurrentMessage);
            }
            else
            {
                if (ClientsList.Contains(CurrentTarget))
                {
                    _chatController?.Send(_chatController.Login, CurrentTarget, CurrentMessage);
                }
                else
                {
                    _chatController?.Send(_chatController.Login, CurrentTarget, CurrentMessage, CurrentTarget);
                }
            }

            CurrentMessage = String.Empty;
        }

        private void ExecuteStopCommand()
        {
            if (_chatController != null)
            {
                /*_timer.Stop();
                _timer.Enabled = false;*/
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ClientsList.Clear();
                    OnlineClients.Clear();
                    OfflineClients.Clear();
                    GroupsList.Clear();
                });
                ChatMessages = String.Empty;
                Chats.Clear();
                ChatVisibility = Visibility.Collapsed;

                _eventAggregator.GetEvent<CloseWindowsEventArgs>().Publish();

                _chatController.ConnectionReceived -= HandleConnectionReceived;
                _chatController.MessageReceived -= HandleMessageReceived;
                _chatController.ChatHistoryReceived -= HandleChatHistoryReceived;
                _chatController.FilteredMessagesReceived -= HandleFilteredMessagesReceived;
                _chatController.ClientsListReceived -= HandleClientsListReceived;
                _chatController.GroupsReceived -= HandleGroupsReceived;

                _chatController.Disconnect();
            }
        }

        private void ExecuteOpenEventLogCommand()
        {
            _eventAggregator.GetEvent<OpenEventLogEventArgs>().Publish();
            ChatVisibility = Visibility.Collapsed;
        }

        private void ExecuteOpenGroupChatCommand()
        {
            ObservableCollection<string> clients = new ObservableCollection<string>(_clientsList.Where(item => item != _chatController.Login));
            _eventAggregator.GetEvent<OpenGroupChatEventArgs>().Publish(clients);
            ChatVisibility = Visibility.Collapsed;
        }

        private void ExecuteChangeStyleCommand()
        {
            _eventAggregator.GetEvent<ChangeStyleEventArgs>().Publish(IsDarkTheme);
        }

        private void ExecuteLeaveGroupCommand()
        {
            _chatController.LeaveGroup(_chatController.Login, CurrentTarget);
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                if (!String.IsNullOrEmpty(e.Client))
                {
                    _chatController.ConnectionReceived += HandleConnectionReceived;
                    _chatController.MessageReceived += HandleMessageReceived;
                    _chatController.ChatHistoryReceived += HandleChatHistoryReceived;
                    _chatController.FilteredMessagesReceived += HandleFilteredMessagesReceived;
                    _chatController.ClientsListReceived += HandleClientsListReceived;
                    _chatController.GroupsReceived += HandleGroupsReceived;

                    /*_timer.Enabled = true;
                    _timer.Start();*/

                    ChatVisibility = Visibility.Visible;

                    ConnectionParametres = $"Login: {_chatController.Login}";

                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        OnlineClients.Add("General");
                        OnlineClients.Add("Event Log");
                        if (e.OnlineClients != null)
                        {
                            OnlineClients.AddRange(e.OnlineClients.ToList());
                        }
                    });
                    CurrentTarget = "General";
                }
            }
            else
            {
                ExecuteStopCommand();
            }
        }

        private void HandleConnectionReceived(object sender, ConnectionReceivedEventArgs e)
        {
            if (!Chats.ContainsKey(e.Login))
            {
                Chats.Add(e.Login, String.Empty);
            }
            if (e.IsConnected)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    OnlineClients.Add(e.Login);
                    OfflineClients.Remove(e.Login);
                });

            }
            else
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    OnlineClients.Remove(e.Login);
                    OfflineClients.Add(e.Login);
                });
            }
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Target) || e.Target == "General")
            {
                Chats["General"] += $"{e.Date} {e.Source} : {e.Message}\n";
                if (CurrentTarget == "General" || CurrentTarget == String.Empty)
                {
                    ChatMessages = Chats["General"];
                }
            }
            else
            {
                if (String.IsNullOrEmpty(e.GroupName))
                {
                    if (_chatController.Login == e.Source)
                    {
                        Chats[e.Target] += $"{e.Date} {e.Source} : {e.Message}\n";
                        if (CurrentTarget == e.Target)
                        {
                            ChatMessages = Chats[e.Target];
                        }
                    }
                    else
                    {
                        Chats[e.Source] += $"{e.Date} {e.Source} : {e.Message}\n";
                        if (CurrentTarget == e.Source)
                        {
                            ChatMessages = Chats[e.Source];
                        }
                    }
                }
                else
                {
                    if (GroupsList.Contains(e.GroupName))
                    {
                        Chats[e.GroupName] += $"{e.Date} {e.Source} : {e.Message}\n";
                        if(CurrentTarget == e.GroupName)
                        {
                            ChatMessages = Chats[e.GroupName];
                        }
                    }
                }
            }
        }

        private void HandleChatHistoryReceived(object sender, ChatHistoryReceivedEventArgs e)
        {
            Chats = e.ClientMessages;
            ChatMessages = Chats["General"];
        }

        private void HandleFilteredMessagesReceived(object sender, FilteredMessagesReceivedEventArgs e)
        {
            Chats["Event Log"] = e.FilteredMessages;
            ChatMessages = Chats["Event Log"];
            CurrentTarget = "Event Log";
        }

        private void HandleClientsListReceived(object sender, ClientsListReceivedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                ClientsList.AddRange(e.ClientsList.Where(item => !ClientsList.Contains(item)));
                OfflineClients.AddRange(e.ClientsList.Where(item => !OnlineClients.Contains(item)));
            });
        }

        private void HandleGroupsReceived(object sender, GroupsReceivedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach(var group in e.Groups)
                {
                    GroupsList.Add(group.Key);
                }
            });
        }

        private void HandleOpenChat()
        {
            ChatVisibility = Visibility.Visible;
        }

        #endregion Methods
    }
}
