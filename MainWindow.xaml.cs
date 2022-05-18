using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Prism.Commands;
using Prism.Mvvm;

namespace Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }


        static async Task Main(string name, string msg)
        {
            string chat;
            var hubConnectionBuilder = new HubConnectionBuilder();
            var client = hubConnectionBuilder.WithUrl("https://localhost:5001/hub").WithAutomaticReconnect().Build();

            await client.StartAsync();
            await client.InvokeAsync("JoinGroup", "myGroup");


            client.On("newMessage", (string x) =>
            {
                var chat = ($"{DateTime.Now} From {name}: {x}");
            });

            string message = msg;
            await client.InvokeAsync("NewMessage", "myGroup", message);
            await client.StopAsync();

        }



    }
}
