using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using Newtonsoft.Json;
using System.Management;

namespace HubEventFeeder
{
    class Program
    {

        //Replace the connection String with your connection and the event hub with your event hub name.
        const string  connectionString = "Endpoint=sb://maheshbus.servicebus.windows.net/;SharedAccessKeyName=mahesh;SharedAccessKey=n75sRkARkVx9vda9PaA6q7n+rFBY83asM5zIj3DIQNI=";
        const string eventHubName = "maheshhub";


        static void Main(string[] args)

        {


            //You can fetch your data from weather API or any Device. Fetching Data is not the key concern here. 
            //This is just a fake Data. The Weather API donot return data every minute or second. So we can not get relative large variety and  
            //and volume in the data. So I have tried to randomize the weather. 
            //While Randomizing the weather, I looked at hourly forcast at any weather forcast site and randomize values around that value, 
            //by adding a slight fluctuation to the value by generating a random number. 
            data mydata = new data();

           while (true)

            {


                ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                foreach (ManagementObject obj in searcher.Get())
                {
                    Double temp = Convert.ToDouble(obj["CurrentTemperature"].ToString());
                    mydata.tempertature = (temp - 2732) / 10.0;
                    mydata.location = obj["InstanceName"].ToString();
                    //float fluctuation = (rnd.Next(1, 10));
                    //fluctuation = fluctuation / 10;
                    //float nowtemperature = weatherDictionary[Convert.ToInt16(DateTime.Now.Hour)] + fluctuation 
                    mydata.ID = Guid.NewGuid().ToString();
                    mydata.when = DateTime.Now;
                    mydata.hour = DateTime.Now.Hour;
                    var eventHub = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);
                    eventHub.SendAsync(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mydata))));
                    Console.WriteLine(JsonConvert.SerializeObject(mydata));
                    Thread.Sleep(300);
                }
            }



         
               
        }

        public class data {
            public int hour { get; set; }
            public string ID { get; set; }
            public double tempertature { get; set; }
             public DateTime when{ get; set; }
            public string location{ get; set; }
        }

    }
}
