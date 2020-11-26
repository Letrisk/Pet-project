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
    using Common.Network._Enums_;
    using Common.Network._EventArgs_;
    using View;
    using Common.Network.Messages;

    public class NavigationViewModel : BindableBase
    {
        #region Fields

        private object _selectedViewModel;

        private ConnectionViewModel _connectionControl = new ConnectionViewModel();
        private ChatViewModel _chatControl = new ChatViewModel();

        #endregion Fields

        #region Properties

        public object SelectedViewModel
        {
            get => _selectedViewModel;
            set => SetProperty(ref _selectedViewModel, value);
        }

        public ConnectionViewModel ConnectionControl
        {
            get => _connectionControl;
            set => SetProperty(ref _connectionControl, value);
        }

        public ChatViewModel ChatControl
        {
            get => _chatControl;
            set => SetProperty(ref _chatControl, value);
        }

        public DelegateCommand EnterChatCommand { get; }

        #endregion Properties

        #region Constructors

        public NavigationViewModel()
        {
            SelectedViewModel = ConnectionControl;

            EnterChatCommand = new DelegateCommand(ExecuteEnterChatCommand);
        }

        #endregion Constructors

        #region Methods

        private void ExecuteEnterChatCommand()
        {
            SelectedViewModel = ChatControl;
        }

        #endregion Methods
    }
}
