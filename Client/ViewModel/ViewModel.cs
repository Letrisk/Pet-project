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

    using Common.Network;
    using Common.Network.Messages;

    public class ViewModel : BindableBase
    {

        #region Fields

        private IController _currentController;

        private List<TransportType> _transportTypes = Enum.GetValues(typeof(TransportType)).OfType<TransportType>().ToList();
        private List<MessageType> _messageTypes = Enum.GetValues(typeof(MessageType)).OfType<MessageType>().ToList();
        private ObservableCollection<string> _clientsList = new ObservableCollection<string>();
        private string _currentLogin, _currentMessage, _currentAddress, _currentPort, _chatMessage, _currentTarget;
        private TransportType _currentTransport = TransportType.WebSocket;
        //private ObservableCollection<TabItem> _chats = new ObservableCollection<TabItem>();

        #endregion Fields

        #region Properties

        public List<TransportType> TransportTypes
        {
            get => _transportTypes;
            set => SetProperty(ref _transportTypes, value);
        }

        public List<MessageType> MessageTypes
        {
            get => _messageTypes;
            set => SetProperty(ref _messageTypes, value);
        }

        public ObservableCollection<string> ClientsList
        {
            get => _clientsList;
            set => SetProperty(ref _clientsList, value);
        }

        public string ChatMessages
        {
            get => _chatMessage;
            set => SetProperty(ref _chatMessage, value);
        }

        public string CurrentLogin
        {
            get => _currentLogin;
            set => SetProperty(ref _currentLogin, value);
        }

        public string CurrentMessage
        {
            get => _currentMessage;
            set => SetProperty(ref _currentMessage, value);
        }

        public string CurrentTarget
        {
            get => _currentTarget;
            set => SetProperty(ref _currentTarget, value);
        }

        public string CurrentAddress
        {
            get => _currentAddress;
            set => SetProperty(ref _currentAddress, value);
        }

        public string CurrentPort
        {
            get => _currentPort;
            set => SetProperty(ref _currentPort, value);
        }

        public TransportType CurrentTransport
        {
            get => _currentTransport;
            set => SetProperty(ref _currentTransport, value);
        }

        /*public ObservableCollection<TabItem> Chats
        {
            get => _chats;
            set => SetProperty(ref _chats, value);
        }*/

        public DelegateCommand StartCommand { get; }

        public DelegateCommand SendCommand { get; }

        public DelegateCommand LoginCommand { get; }

        public DelegateCommand StopCommand { get; }

        public DelegateCommand SendPrivateCommand { get; }

        #endregion Properties

        #region Constructors

        public ViewModel()
        {
            StartCommand = new DelegateCommand(ExecuteStartCommand);
            SendCommand = new DelegateCommand(ExecuteSendCommand);
            LoginCommand = new DelegateCommand(ExecuteLoginCommand);
            StopCommand = new DelegateCommand(ExecuteStopCommand);
            SendPrivateCommand = new DelegateCommand(ExecuteSendPrivateCommand);
        }

        #endregion Constructors

        #region Methods

        private void ExecuteStartCommand()
        {
            try
            {
                _currentController = ControllerFactory.Create(CurrentTransport);
                _currentController.ConnectionStateChanged += HandleConnectionStateChanged;
                _currentController.MessageReceived += HandleMessageReceived;
                _currentController.ConnectionReceived += HandleConnectionReceived;
                _currentController.Connect(CurrentAddress, CurrentPort);
            }

            catch (Exception ex)
            {
                ChatMessages += $"{ex.Message}\n";
            }
        }

        private void ExecuteLoginCommand()
        {
            _currentController?.Login(CurrentLogin);
        }

        private void ExecuteSendCommand()
        {
            _currentController?.Send(CurrentLogin, CurrentTarget, CurrentMessage);
        }

        private void ExecuteStopCommand()
        {
            if (_currentController != null)
            {
                _currentController.ConnectionStateChanged -= HandleConnectionStateChanged;
                _currentController.MessageReceived -= HandleMessageReceived;
                _currentController.Disconnect();
                ClientsList.Clear();
            }
        }

        private void ExecuteSendPrivateCommand()
        {
            /*TabItem newTabItem = new TabItem
            {
                Header = "Test",
                Content = "dsfsdfsdf"
            };
            Chats.Add(newTabItem);*/
            
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                if (string.IsNullOrEmpty(e.Client))
                {
                    ChatMessages += $"{e.Date} Клиент подключен к серверу\n";
                    ChatMessages += $"{e.Date} Авторизуйтесь, чтобы отправлять сообщения.\n";
                }
                else
                {
                    ChatMessages += $"{e.Date} Авторизация выполнена успешно.\n";

                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        foreach (object client in e.OnlineClients)
                        {
                            ClientsList.Add((string)client);
                        }

                    });
                }
            }
            else
            {
                ChatMessages += $"{e.Date} Клиент отключен от сервера.\n";
            }
        }


        private void HandleConnectionReceived(object sender, ConnectionReceivedEventArgs e)
        {
            if (e.IsConnected)
            {
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
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ClientsList.Remove(e.Login);
                });
            }
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            ChatMessages += $"{e.Date} {e.Source} : {e.Message}\n";
        }

        #endregion Methods
    }
}
