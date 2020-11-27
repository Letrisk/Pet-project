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

    using Common.Network;
    using Common.Network.Messages;

    public class ChatViewModel : BindableBase
    {
        #region Fields

        private IChatController _chatController;

        private ObservableCollection<string> _clientsList = new ObservableCollection<string>(){ "General" };

        private Dictionary<string, string> _chats = new Dictionary<string, string>() { { "General", String.Empty } };

        private string _currentMessage, _chatMessages, _currentTarget = "General", _connectionParametres; //если currentTarget отключается, то происходит краш

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

        public Dictionary<string, string> Chats
        {
            get => _chats;
            set => SetProperty(ref _chats, value);
        }

        public string ChatMessages
        {
            get => _chats[_currentTarget];
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
                
                SetProperty(ref _currentTarget, value);

                if (_currentTarget == null)
                {
                    _currentTarget = "General";
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

        public DelegateCommand SendPrivateCommand { get; }

        #endregion Properties

        #region Constructors

        public ChatViewModel(IChatController chatController)
        {
            _chatController = chatController;

            SendCommand = new DelegateCommand(ExecuteSendCommand);
            StopCommand = new DelegateCommand(ExecuteStopCommand);

            _chatController.ConnectionStateChanged += HandleConnectionStateChanged;
            _chatController.ConnectionReceived += HandleConnectionReceived;
            _chatController.MessageReceived += HandleMessageReceived;
        }

        #endregion Constructors

        #region Methods

        private void ExecuteSendCommand()
        {
            if (CurrentTarget == "General")
            {
                _chatController?.Send(_chatController.Login, null, CurrentMessage);
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
                ClientsList.Add("General");
            }
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                if (string.IsNullOrEmpty(e.Client))
                {
                    Chats["General"] += $"Авторизуйтесь, чтобы отправлять сообщения.\n";
                    ChatMessages = Chats["General"];
                }
                else
                {
                    ChatVisibility = Visibility.Visible;

                    Chats["General"] += $"Авторизация выполнена успешно.\n";
                    ChatMessages = Chats["General"];

                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        foreach (object client in e.OnlineClients)
                        {
                            Chats.Add((string)client, String.Empty);
                            ClientsList.Add((string)client);
                        }

                    });
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
                if (!Chats.ContainsKey(e.Login))
                {
                    Chats.Add(e.Login, String.Empty);
                }

                if (!ClientsList.Contains(e.Login))
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        ClientsList.Add(e.Login);
                    });
                }
            }
            else
            {
                Chats.Remove(e.Login);

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ClientsList.Remove(e.Login);
                });
            }
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Target) || e.Target == "General")
            {
                Chats["General"] += $"{e.Date} {e.Source} : {e.Message}\n";
                ChatMessages = Chats["General"];
            }
            else
            {
                Chats[e.Source] += $"{e.Date} {e.Source} : {e.Message}\n";
                ChatMessages = Chats[e.Source];
            }
        }

        #endregion Methods
    }
}
