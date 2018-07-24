using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Search;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using OSIsoft.AF.Time;

namespace EventFrames
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkCredential credential = new NetworkCredential(connectionInfo.user, connectionInfo.password);
            var piSystem = (new PISystems())[connectionInfo.AFServerName];
            Console.WriteLine($"connecting to : {connectionInfo.AFServerName} - {connectionInfo.AFDatabaseName}");
            piSystem.Connect(credential);
            var afdb = piSystem.Databases[connectionInfo.AFDatabaseName];
            Console.WriteLine("connected");

            var query = "Template:'Antimatter Relay' Name:T001";
            var search = new AFElementSearch(afdb, "Relay Search", query);
            var relay = search.FindElements(0, true, 1).FirstOrDefault();

            if (relay != null)
            {
                var now = DateTime.Now;
                
                //this will not work, the kind propery is not set and it treated as UTC
                //var end = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

                //the correct way to do it
                var end = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Local);
                var start = end.AddDays(-1);

                AFTimeRange range = new AFTimeRange(start, end);
                AFTimeSpan span = AFTimeSpan.Parse("1h");
                var values = relay.Attributes["Ion Charge"].Data.InterpolatedValues(range, span, null, "", false);
                foreach (var value in values)
                {
                    Console.WriteLine($"value: {value.ValueAsDouble()} time: {value.Timestamp.ToString()}");
                }
            }
            Console.WriteLine("completed execution");
            Console.ReadKey();
        }
    }
}
