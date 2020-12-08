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

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Events;

    using Common.Network;
    using Common.Network.Messages;

    public class ChatViewModel : BindableBase
    {
        #region Fields

        private IChatController _chatController;
        private IEventAggregator _eventAggregator;

        private ObservableCollection<string> _clientsList = new ObservableCollection<string>();
        private ObservableCollection<string> _onlineClients = new ObservableCollection<string>();
        private ObservableCollection<string> _offlineClients = new ObservableCollection<string>();
 
        private Dictionary<string, string> _chats = new Dictionary<string, string>();

        private string _currentMessage, _chatMessages, _currentTarget, _connectionParametres;

        private Visibility _chatVisibility = Visibility.Collapsed;

        #endregion Fields

        #region Properties

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

        public DelegateCommand SendCommand { get; }

        public DelegateCommand StopCommand { get; }

        public DelegateCommand OpenEventLogCommand { get; }

        #endregion Properties

        #region Constructors

        public ChatViewModel(IChatController chatController, IEventAggregator eventAggregator)
        {
            _chatController = chatController;
            _eventAggregator = eventAggregator;

            eventAggregator.GetEvent<OpenChatEventArgs>().Subscribe(HandleOpenChat);

            SendCommand = new DelegateCommand(ExecuteSendCommand);
            StopCommand = new DelegateCommand(ExecuteStopCommand);
            OpenEventLogCommand = new DelegateCommand(ExecuteOpenEventLogCommand);

            _chatController.ConnectionStateChanged += HandleConnectionStateChanged;
            _chatController.ConnectionReceived += HandleConnectionReceived;
            _chatController.MessageReceived += HandleMessageReceived;
            _chatController.ChatHistoryReceived += HandleChatHistoryReceived;
            _chatController.FilteredMessagesReceived += HandleFilteredMessagesReceived;
            _chatController.ClientsListReceived += HandleClientsListReceived;
        }

        #endregion Constructors

        #region Methods

        private void ExecuteSendCommand()
        {
            if (CurrentTarget == "General")
            {
                _chatController?.Send(_chatController.Login, String.Empty, CurrentMessage);
            }
            else
            {
                _chatController?.Send(_chatController.Login, CurrentTarget, CurrentMessage);
            }

            CurrentMessage = String.Empty;
        }

        private void ExecuteStopCommand()
        {
            if (_chatController != null)
            {
                _chatController.Disconnect();
                ClientsList.Clear();
                OnlineClients.Clear();
                OfflineClients.Clear();
                ChatMessages = String.Empty;
                Chats.Clear();
            }
        }

        private void ExecuteOpenEventLogCommand()
        {
            _eventAggregator.GetEvent<OpenEventLogEventArgs>().Publish();
            ChatVisibility = Visibility.Collapsed;
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                if (!ClientsList.Contains("General"))
                {
                    //OnlineClients.Add("General");
                    //Chats.Add("General", String.Empty);
                    //Chats.Add("Event Log", String.Empty);
                }

                if (String.IsNullOrEmpty(e.Client))
                {
                    //Chats["General"] += $"Авторизуйтесь, чтобы отправлять сообщения.\n";
                    //ChatMessages = Chats["General"];
                }
                else
                {
                    ChatVisibility = Visibility.Visible;

                    /*Chats["General"] += $"Авторизация выполнена успешно.\n";
                    ChatMessages = Chats["General"];*/
                    ConnectionParametres = $"Login: {_chatController.Login}";

                    /*App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        foreach (object client in e.OnlineClients)
                        {
                            if (!Chats.ContainsKey((string)client))
                            {
                                Chats.Add((string)client, String.Empty);
                            }
                            
                            ClientsList.Add((string)client);
                        }

                    });*/
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        OnlineClients.Add("General");
                        OnlineClients.Add("Event Log");
                        OnlineClients.AddRange(e.OnlineClients.ToList());
                    });
                    CurrentTarget = "General";
                    
                }
            }
            else
            {
                ChatVisibility = Visibility.Collapsed;
            }
        }

        private void HandleConnectionReceived(object sender, ConnectionReceivedEventArgs e)
        {
            if (e.IsConnected)
            {
                /*if (!ClientsList.Contains(e.Login))
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        ClientsList.Add(e.Login);
                    });
                }*/
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
        }

        private void HandleChatHistoryReceived(object sender, ChatHistoryReceivedEventArgs e)
        {
            Chats = e.ClientMessages;
            ChatMessages = Chats["General"];
        }

        private void HandleFilteredMessagesReceived(object sender, FilteredMessagesReceivedEventArgs e)
        {
            /*if (!ClientsList.Contains("Event Log"))
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ClientsList.Add("Event Log");
                });
            }*/

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

        private void HandleOpenChat()
        {
            ChatVisibility = Visibility.Visible;
        }

        #endregion Methods
    }
}
