﻿namespace Pet_Project.ViewModel
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
    using Common.Network._Enums_;
    using Common.Network._EventArgs_;
    using View;
    using Common.Network.Messages;

    public class ConnectionViewModel : BindableBase
    {
        #region Constants

        const TransportType CurrentTransport = TransportType.WebSocket;

        #endregion Constants

        #region Fields

        private IController _currentController;

        private string _currentAddress = "192.168.37.147", _currentPort = "65000", _currentLogin, _guideText = "Введите адрес и порт";

        private bool _isLoginEnable = false;

        #endregion Fields

        #region Properties

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

        public ConnectionViewModel()
        {
            StartCommand = new DelegateCommand(ExecuteStartCommand);
            LoginCommand = new DelegateCommand(ExecuteLoginCommand);
        }

        #endregion Constructors

        #region Methods

        private void ExecuteStartCommand()
        {
            try
            {
                _currentController = ControllerFactory.Create(CurrentTransport);
                _currentController.ConnectionStateChanged += HandleConnectionStateChanged;
                _currentController.Connect(CurrentAddress, CurrentPort);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void ExecuteLoginCommand()
        {
            _currentController?.Login(CurrentLogin);
            NavigationViewModel navMod = new NavigationViewModel { SelectedViewModel = new ChatViewModel()};
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
                GuideText = $"Клиент отключен от сервера.\n";
            }
        }

        #endregion Methods
    }
}