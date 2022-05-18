using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Prism.Commands;
using Prism.Mvvm;

namespace Chat
{
    public class MainViewModel : BindableBase
    {
        #region Fields

        const string ChatUrl = "https://localhost:5001/hub";
        private HubConnection client = null!;

        #endregion

        public MainViewModel()
        {
            Initialize();
        }

        #region Properties

        #region property UserName

        public string UserName
        {
            get => _userName;
            set
            {
                SetProperty(ref _userName, value);
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        private string _userName = "User1";

        #endregion

        #region property MessageList

        public ObservableCollection<string> MessageList
        {
            get => _messageList;
            set => SetProperty(ref _messageList, value);
        }

        private ObservableCollection<string> _messageList = new ObservableCollection<string>();

        #endregion

        #region property MessageText

        public string? MessageText
        {
            get => _messageText;
            set
            {
                SetProperty(ref _messageText, value);
                SendCommand!.RaiseCanExecuteChanged();
            }
        }

        private string? _messageText;

        #endregion

        #endregion

        #region Commands

        #region ConnectCommand

        /// <summary>
        /// The Connect DelegateCommand
        /// </summary>
        public DelegateCommand ConnectCommand { get; private set; } = null!;

        private bool ConnectCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Execute method for ConnectCommand
        /// </summary>
        private async void ConnectCommandExecute()
        {
            await ConnectToChatAsync();
        }

        #endregion // ConnectCommand region

        #region SendCommand

        public DelegateCommand? SendCommand { get; private set; }

        private bool SendCommandCanExecute()
        {
            return true;
        }

        private async void SendCommandExecute()
        {
            await SendMessageAsync(UserName, MessageText);
            MessageText = string.Empty;
        }

        #endregion // SendCommand region

        #endregion

        #region privates

        private void Initialize()
        {
            ConnectCommand = new DelegateCommand(ConnectCommandExecute, ConnectCommandCanExecute);
            SendCommand = new DelegateCommand(SendCommandExecute, SendCommandCanExecute);
        }


        private async Task SendMessageAsync(string userName, string? message)
        {
            
            client.On("newMessage", (string x, string y) =>
            {
                var chat = ($"{DateTime.Now} From {y}: {x}");
                MessageList.Add(chat);
            });

            await client.InvokeAsync("NewMessage", "myGroup", message, userName);



        }

        private async Task ConnectToChatAsync()
        {
            client = new HubConnectionBuilder()
                .WithUrl(ChatUrl)
                .WithAutomaticReconnect()
                .Build();

            try
            {
                await client.StartAsync();
                await client.InvokeAsync("JoinGroup", "myGroup");
            }
            catch (Exception exception)
            {
                MessageList.Add(exception.Message);
            }
        }
        #endregion
    }
}