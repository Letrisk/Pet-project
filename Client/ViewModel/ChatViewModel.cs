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
        #region Constants

        private const string GeneralChat = "General";
        private const string EventLog = "Event Log";

        #endregion Constants

        #region Fields

        private readonly Timer _timer;

        private readonly IChatController _chatController;
        private readonly IEventAggregator _eventAggregator;

        private ObservableCollection<Client> _clientsCollection;
        private Client _selectedClient;

        private ObservableCollection<Client> _groupsList;
        private ObservableCollection<string> _chatMessages;

        private Dictionary<string, List<string>> _chats;

        private int _counter;

        private string _currentMessage;
        private string _connectionParametres;
        private string _login;

        private bool _isDarkTheme;
        private bool _isGroupMessage;

        private Visibility _chatVisibility;

        #endregion Fields

        #region Properties

        public Visibility ChatVisibility
        {
            get => _chatVisibility;
            set => SetProperty(ref _chatVisibility, value);
        }

        public ObservableCollection<Client> GroupsList
        {
            get => _groupsList;
            set => SetProperty(ref _groupsList, value);
        }

        public Dictionary<string, List<string>> Chats
        {
            get => _chats;
            set => SetProperty(ref _chats, value);
        }

        public ObservableCollection<string> ChatMessages
        {
            get => _chatMessages;
            set => SetProperty(ref _chatMessages, value);
        }

        public ObservableCollection<Client> ClientsCollection
        {
            get => _clientsCollection;
            set => SetProperty(ref _clientsCollection, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (value == null)
                {
                    SetProperty(ref _selectedClient, _clientsCollection.FirstOrDefault(item => item.Login == GeneralChat));
                    return;
                }
                else
                {
                    SetProperty(ref _selectedClient, value);
                }

                if (!_chats.ContainsKey(_selectedClient.Login))
                {
                    _chats.Add(_selectedClient.Login, new List<string>());
                }

                if (_groupsList.FirstOrDefault(item => item.Login == value.Login) != default)
                {
                    _isGroupMessage = true;
                }
                else
                {
                    _isGroupMessage = false;
                }

                ChatMessages = new ObservableCollection<string>(_chats[_selectedClient.Login]);
            }
        }

        public string CurrentMessage
        {
            get => _currentMessage;
            set => SetProperty(ref _currentMessage, value);
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

        public bool IsGroupMessage
        {
            get => _isGroupMessage;
            set => SetProperty(ref _isGroupMessage, value);
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

            _groupsList = new ObservableCollection<Client>();
            _chatMessages = new ObservableCollection<string>();
            _clientsCollection = new ObservableCollection<Client>();

            _chatVisibility = Visibility.Collapsed;

            _chats = new Dictionary<string, List<string>>();

            _counter = 0;

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
            _chatController.ConnectionReceived += HandleConnectionReceived;
            _chatController.MessageReceived += HandleMessageReceived;
            _chatController.ClientsListReceived += HandleClientsListReceived;
            _chatController.ChatHistoryReceived += HandleChatHistoryReceived;
            _chatController.FilteredMessagesReceived += HandleFilteredMessagesReceived;
            _chatController.GroupsReceived += HandleGroupsReceived;
        }

        #endregion Constructors

      #region Methods

        private void OnTimerEvent(object sender, ElapsedEventArgs e)
        {
            _chatController?.Send(String.Empty, _counter++.ToString(), String.Empty);
        }

        private void ExecuteSendCommand()
        {
            if (String.IsNullOrEmpty(CurrentMessage) || SelectedClient.Login == EventLog)
            {
                ChatMessages.Add($"Something go wrong!");
            }
            else
            {
                if (IsGroupMessage)
                {
                    _chatController?.Send(SelectedClient.Login, CurrentMessage, SelectedClient.Login);
                }
                else
                {
                    _chatController?.Send(SelectedClient.Login, CurrentMessage, String.Empty);
                }
            }

            CurrentMessage = String.Empty;
        }

        private void ExecuteStopCommand()
        {
                /*_timer.Stop();
                _timer.Enabled = false;*/
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    GroupsList.Clear();
                    ChatMessages.Clear();
                    ClientsCollection.Clear();
                });

                Chats.Clear();
                ChatVisibility = Visibility.Collapsed;

                _eventAggregator.GetEvent<CloseWindowsEventArgs>().Publish();

                _chatController.Disconnect();
        }

        private void ExecuteOpenEventLogCommand()
        {
            _eventAggregator.GetEvent<OpenEventLogEventArgs>().Publish();
            ChatVisibility = Visibility.Collapsed;
        }

        private void ExecuteOpenGroupChatCommand()
        {
            ObservableCollection<string> clients = new ObservableCollection<string>(ClientsCollection.Select(item => item.Login).Where(item => 
                                                                                    item != GeneralChat && item != EventLog && item != _login));
            _eventAggregator.GetEvent<OpenGroupChatEventArgs>().Publish(clients);
            ChatVisibility = Visibility.Collapsed;
        }

        private void ExecuteChangeStyleCommand()
        {
            _eventAggregator.GetEvent<ChangeStyleEventArgs>().Publish(IsDarkTheme);
        }

        private void ExecuteLeaveGroupCommand()
        {
            _chatController.LeaveGroup(_login, SelectedClient.Login);
            GroupsList.Remove(SelectedClient);
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                _login = e.Client;
                ChatVisibility = Visibility.Visible;

                ConnectionParametres = $"Login: {_login}";

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ClientsCollection.Add(new Client(GeneralChat, true));
                    ClientsCollection.Add(new Client(EventLog, true));
                    if (e.OnlineClients != null)
                    {
                        foreach (var client in e.OnlineClients)
                        {
                            ClientsCollection.Add(new Client(client, true));
                        }
                    }
                });
                SelectedClient = ClientsCollection.FirstOrDefault(item => item.Login == GeneralChat);
            }
            else
            {
                ExecuteStopCommand();
            }
        }

        private void HandleConnectionReceived(object sender, ConnectionReceivedEventArgs e)
        {
            Client focusedClient = SelectedClient;

            if (!Chats.ContainsKey(e.Login))
            {
                Chats.Add(e.Login, new List<string>());
            }
            if (e.IsConnected)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ClientsCollection.FirstOrDefault(item => item.Login == e.Login).IsOnline = true;
                    ClientsCollection = new ObservableCollection<Client>(ClientsCollection.OrderByDescending(item => item.IsOnline));

                    SelectedClient = focusedClient;                       
                });

            }
            else
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ClientsCollection.FirstOrDefault(item => item.Login == e.Login).IsOnline = false;
                    ClientsCollection = new ObservableCollection<Client>(ClientsCollection.OrderByDescending(item => item.IsOnline));

                    SelectedClient = focusedClient;
                });
            }
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.GroupName))
            {
                if (String.IsNullOrEmpty(e.Target) || e.Target == GeneralChat)
                {
                    Chats[GeneralChat].Add($"{e.Date} {e.Source} : {e.Message}\n");
                    if (SelectedClient?.Login == GeneralChat)
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            ChatMessages = new ObservableCollection<string>(Chats[GeneralChat]);
                        });
                    }
                }
                else
                {
                    if (_login == e.Source)
                    {
                        Chats[e.Target].Add($"{e.Date} {e.Source} : {e.Message}\n");
                        if (SelectedClient.Login == e.Target)
                        {
                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                ChatMessages = new ObservableCollection<string>(Chats[e.Target]);
                            });
                        }
                    }
                    else
                    {
                        Chats[e.Source].Add($"{e.Date} {e.Source} : {e.Message}\n");
                        if (SelectedClient.Login == e.Source)
                        {
                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                ChatMessages = new ObservableCollection<string>(Chats[e.Source]);
                            });
                        }
                    }
                }
            }
            else
            {
                if (GroupsList.FirstOrDefault(item => item.Login == e.GroupName) != default)
                {
                    Chats[e.GroupName].Add($"{e.Date} {e.Source} : {e.Message}\n");
                    if (SelectedClient.Login == e.GroupName)
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            ChatMessages = new ObservableCollection<string>(Chats[e.GroupName]);
                        });
                    }
                }
            }
        }

        private void HandleChatHistoryReceived(object sender, ChatHistoryReceivedEventArgs e)
        {
            Chats = e.ClientMessages;
            ChatMessages = new ObservableCollection<string>(Chats[GeneralChat]);
        }

        private void HandleFilteredMessagesReceived(object sender, FilteredMessagesReceivedEventArgs e)
        {
            Chats[EventLog] = e.FilteredMessages;
            ChatMessages = new ObservableCollection<string>(Chats[EventLog]);
            SelectedClient = ClientsCollection.FirstOrDefault(item => item.Login == EventLog);
        }

        private void HandleClientsListReceived(object sender, ClientsListReceivedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach(var client in e.ClientsList)
                {
                    if (ClientsCollection.FirstOrDefault(item => item.Login == client) == default)
                    {
                        ClientsCollection.Add(new Client(client, false)); 
                    }
                    if (!Chats.ContainsKey(client))
                    {
                        Chats.Add(client, new List<string>());
                    }
                }
            });
        }

        private void HandleGroupsReceived(object sender, GroupsReceivedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach(var group in e.Groups)
                {
                    GroupsList.Add(new Client(group.Key, true));
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
