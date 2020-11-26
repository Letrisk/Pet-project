namespace Pet_Project.ViewModel
{
    using System;
    using System.Windows;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Threading;

    using Prism.Commands;
    using Prism.Mvvm;

    using Common.Network;
    using Common.Network._Enums_;
    using Common.Network._EventArgs_;
    using Common.Network.Messages;

    public class ChatViewModel : BindableBase
    {
        #region Fields

        private IController _currentController;

        private ObservableCollection<string> _clientsList = new ObservableCollection<string>();
        private string _currentMessage, _chatMessages, _currentTarget;

        #endregion Fields

        #region Properties

        public ObservableCollection<string> ClientsList
        {
            get => _clientsList;
            set => SetProperty(ref _clientsList, value);
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
            set => SetProperty(ref _currentTarget, value);
        }

        public DelegateCommand SendCommand { get; }

        public DelegateCommand StopCommand { get; }

        public DelegateCommand SendPrivateCommand { get; }

        #endregion Properties

        #region Constructors

        public ChatViewModel()
        {

        }

        #endregion Constructors
    }
}
