using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using System.Text;
using System.Diagnostics;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using Newtonsoft.Json;


// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace MyastiaAzure
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer;
        SensorDevice myDevice;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            myDevice = new SensorDevice();
            myDevice.Run();

            this.TextBox01.Text = "完了";

            timer = new DispatcherTimer();
            timer.Tick += SetTextBoxIotHub;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void SetTextBoxIotHub(object sender, object e)
        {
            this.TextBoxHubMessage.Text = myDevice.ReceivedMessage;
            if (myDevice.Error.Count != 0)
            {
                this.TextBoxError.Text = myDevice.Error[0];
            }
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
