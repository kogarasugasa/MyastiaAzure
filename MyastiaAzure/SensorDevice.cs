using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyastiaAzure
{
    public class SensorDevice
    {
        private readonly DeviceClient deviceClient;
        private readonly KeyValuePair<string, string> deviceProperty;
        private StringBuilder cnString = new StringBuilder();
        private const string HostName = "MyastiaIot.azure-devices.net";
        private const string DeviceId = "Rasp01;";
        private const string SharedAccessKey = "9gbtY1yoTNxNME3RwxighF3u8xqc5j/Tcd49EW7saNc=";

        public SensorDevice()
        {
            cnString.Append("HostName=" + HostName + ";");
            cnString.Append("DeviceId=" + DeviceId + ";");
            cnString.Append("SharedAccessKey=" + SharedAccessKey);
            deviceProperty = new KeyValuePair<string, string>("DeviceType", "RasspberryPi");


            deviceClient = DeviceClient.CreateFromConnectionString(cnString.ToString(), TransportType.Http1);
        }
        ~SensorDevice()
        {
            deviceClient.Dispose();
        }
        //********************************************************************************
        //プロパティ
        //********************************************************************************
        public string ReceivedMessage { get; set; } = string.Empty;
        public List<string> Error { get; } = new List<string>();

        //********************************************************************************
        //メソッド
        //********************************************************************************
        public async void Run()
        {
            SendAzureMessage();
            ReceiveAzureMessage();
        }

        private async Task ThrowError()
        {
            await Task.Delay(1000);
            throw new Exception();
        }

        private async Task SendAzureMessage()
        {
            try
            {
                var data = new SensorData();
                data.DeviceId = DeviceId;
                data.SensorText = "Use json";

                var dataString = JsonConvert.SerializeObject(data);
                using (var message = new Message(Encoding.UTF8.GetBytes(dataString)))
                {
                    message.Properties.Add(deviceProperty);
                    await deviceClient.SendEventAsync(message);
                }
            }
            catch (Exception e)
            {
                this.Error.Add(JsonConvert.SerializeObject(e));
            }
        }

        private async Task ReceiveAzureMessage()
        {
            try
            {
                while (true)
                {
                    using (var message = await deviceClient.ReceiveAsync())
                    {
                        if (message == null)
                        {
                            continue;
                        }
                        var messageText = Encoding.UTF8.GetString(message.GetBytes());
                        await deviceClient.CompleteAsync(message);
                        this.ReceivedMessage = messageText;
                    }
                }
            }
            catch (Exception e)
            {
                this.Error.Add(JsonConvert.SerializeObject(e));
            }
        }
    }

    class SensorData
    {
        public string DeviceId { get; set; } = string.Empty;
        public string SensorText { get; set; } = string.Empty;
    }
}
