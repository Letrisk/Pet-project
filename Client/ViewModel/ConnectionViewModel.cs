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

    public class ConnectionViewModel : BindableBase
    {
        #region Constants

        const TransportType CurrentTransport = TransportType.WebSocket;

        #endregion Constants

        #region Fields

        private readonly IConnectionController _connectionController;
        private readonly IEventAggregator _eventAggregator;

        private Visibility _connectionVisibility;

        private string _address;
        private string _port;
        private string _login;
        private string _guideText;

        private bool _isAuthorized;
        private bool _isDarkTheme;

        #endregion Fields

        #region Properties

        public Visibility ConnectionVisibility
        {
            get => _connectionVisibility;
            set => SetProperty(ref _connectionVisibility, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        public string GuideText
        {
            get => _guideText;
            set => SetProperty(ref _guideText, value);
        }

        public bool IsAuthorized
        {
            get => _isAuthorized;
            set => SetProperty(ref _isAuthorized, value);
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set => SetProperty(ref _isDarkTheme, value);
        }

        public DelegateCommand StartCommand { get; }

        public DelegateCommand LoginCommand { get; }

        #endregion Properties

        #region Constructors

        public ConnectionViewModel(IEventAggregator eventAggregator, IConnectionController connectionController)
        {
            _eventAggregator = eventAggregator;
            _connectionController = connectionController;

            _connectionVisibility = Visibility.Visible;
            _address = "192.168.37.147";
            _port = "65000";
            _guideText = "Введите адрес и порт";
            _isAuthorized = false;

            eventAggregator.GetEvent<ChangeStyleEventArgs>().Subscribe(HandleChangeStyle);
            eventAggregator.GetEvent<CloseWindowsEventArgs>().Subscribe(HandleDisconnection);

            StartCommand = new DelegateCommand(ExecuteStartCommand);
            LoginCommand = new DelegateCommand(ExecuteLoginCommand);

            _connectionController.ConnectionStateChanged += HandleConnectionStateChanged;
            _connectionController.ErrorReceived += HandleErrorReceived;
        }

        #endregion Constructors

        #region Methods

        private void ExecuteStartCommand()
        {
            try
            {
                _connectionController.Connect(Address, Port);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void ExecuteLoginCommand()
        {
            _connectionController.Login(Login);
            ConnectionVisibility = Visibility.Collapsed;
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                if (string.IsNullOrEmpty(e.Client))
                {
                    GuideText = $"Авторизуйтесь, чтобы отправлять сообщения.\n";
                    IsAuthorized = true;
                }
                else
                {
                    GuideText = $"Авторизация выполнена успешно.\n";
                    _eventAggregator.GetEvent<OpenChatEventArgs>().Publish();
                }
            }
        }

        private void HandleErrorReceived(object sender, ErrorReceivedEventArgs e)
        {
            MessageBox.Show(e.Reason);
        }

        private void HandleChangeStyle(bool isDarkTheme)
        {
            IsDarkTheme = isDarkTheme;
        }

        private void HandleDisconnection()
        {
            Login = String.Empty;
            IsAuthorized = false;

            ConnectionVisibility = Visibility.Visible;
        }

        #endregion Methods
    }
}
