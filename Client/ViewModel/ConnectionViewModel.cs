namespace Pet_Project.ViewModel
{
    using System;
    using System.Windows;
    using System.Text.RegularExpressions;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Events;

    using Common.Network;

    public class ConnectionViewModel : BindableBase
    {
        #region Constants

        const string ADDRESS_FORMAT = @"\b\d{1,3}.\b\d{1,3}.\b\d{1,3}.\b\d{1,3}\b";
        const string PORT_FORMAT = @"\b\d{1,5}\b";
        const string LOGIN_FORMAT = @"^\w{1,20}$";

        #endregion Constants

        #region Fields

        private readonly IConnectionController _connectionController;
        private readonly IEventAggregator _eventAggregator;

        private Visibility _connectionVisibility;

        private string _address;
        private string _port;
        private string _login;
        private string _guideText;

        private bool _isConnected;
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
            set
            {
                SetProperty(ref _address, value);
            }
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

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
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
            _guideText = "Enter address and port";
            _isConnected = false;

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
            if (Regex.IsMatch(_address, ADDRESS_FORMAT) && Regex.IsMatch(_port, PORT_FORMAT))
            {
                try
                {
                    _connectionController.Connect(Address, Port);
                }

                catch (Exception ex)
                {
                    GuideText = ex.Message;
                } 
            }
            else
            {
                GuideText = "Incorrect IP or/and port!";
            }
        }

        private void ExecuteLoginCommand()
        {
            if (Regex.IsMatch(Login, LOGIN_FORMAT))
            {
                _connectionController.Login(Login);
                ConnectionVisibility = Visibility.Collapsed;
            }
            else
            {
                GuideText = "Incorrect login!";
            }
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                if (string.IsNullOrEmpty(e.Client))
                {
                    GuideText = $"Authorize for messaging.\n";
                    IsConnected = true;
                }
                else
                {
                    GuideText = $"Authorization complete.\n";
                    _eventAggregator.GetEvent<OpenChatEventArgs>().Publish();
                }
            }
        }

        private void HandleErrorReceived(object sender, ErrorReceivedEventArgs e)
        {
            GuideText = e.Reason;
        }

        private void HandleChangeStyle(bool isDarkTheme)
        {
            IsDarkTheme = isDarkTheme;
        }

        private void HandleDisconnection()
        {
            Login = String.Empty;
            IsConnected = false;

            ConnectionVisibility = Visibility.Visible;
        }

        #endregion Methods
    }
}
