namespace Common.Network
{
    using System.Collections.ObjectModel;

    using Prism.Events;

    public class OpenGroupChatEventArgs : PubSubEvent<ObservableCollection<string>>
    {
    }
}
