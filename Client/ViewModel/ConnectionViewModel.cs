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
    using View;
    using Common.Network.Messages;

    public class ConnectionViewModel : BindableBase
    {
        #region Constants

        const TransportType CurrentTransport = TransportType.WebSocket;

        #endregion Constants

        #region Fields

        private IConnectionController _connectionController;

        private Visibility _connectionVisibility = Visibility.Visible;

        private string _currentAddress = "192.168.37.147", _currentPort = "65000", _currentLogin, _guideText = "Введите адрес и порт";

        private bool _isLoginEnable = false;

        #endregion Fields

        #region Properties

        public Visibility ConnectionVisibility
        {
            get => _connectionVisibility;
            set => SetProperty(ref _connectionVisibility, value);
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

        public string CurrentLogin
        {
            get => _currentLogin;
            set => SetProperty(ref _currentLogin, value);
        }

        public string GuideText
        {
            get => _guideText;
            set => SetProperty(ref _guideText, value);
        }

        public bool IsLoginEnable
        {
            get => _isLoginEnable;
            set => SetProperty(ref _isLoginEnable, value);
        }

        public DelegateCommand StartCommand { get; }

        public DelegateCommand LoginCommand { get; }

        #endregion Properties

        #region Constructors

        public ConnectionViewModel(IConnectionController connectionController)
        {
            _connectionController = connectionController;
            StartCommand = new DelegateCommand(ExecuteStartCommand);
            LoginCommand = new DelegateCommand(ExecuteLoginCommand);
        }

        #endregion Constructors

        #region Methods

        private void ExecuteStartCommand()
        {
            _connectionController.ConnectionStateChanged -= HandleConnectionStateChanged;
            _connectionController?.Disconnect();

            try
            {
                _connectionController.ConnectionStateChanged += HandleConnectionStateChanged;
                _connectionController.ErrorReceived += HandleErrorReceived;
                _connectionController.Connect(CurrentAddress, CurrentPort);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void ExecuteLoginCommand()
        {
            _connectionController?.Login(CurrentLogin);
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                if (string.IsNullOrEmpty(e.Client))
                {
                    //GuideText += $"{e.Date} Клиент подключен к серверу\n";
                    GuideText = $"Авторизуйтесь, чтобы отправлять сообщения.\n";
                    IsLoginEnable = true;
                }
                else
                {
                    GuideText = $"Авторизация выполнена успешно.\n";

                    ConnectionVisibility = Visibility.Collapsed;
                    /*App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        foreach (object client in e.OnlineClients)
                        {
                            ClientsList.Add((string)client);
                        }

                    });*/
                }
            }
            else
            {
                //GuideText = "Введите адрес и порт";
                CurrentLogin = String.Empty;
                IsLoginEnable = false;
                _connectionController.ConnectionStateChanged -= HandleConnectionStateChanged;

                ConnectionVisibility = Visibility.Visible;
            }
        }

        private void HandleErrorReceived(object sender, ErrorReceivedEventArgs e)
        {
            CurrentLogin = e.Reason;
        }

        #endregion Methods
    }
}
