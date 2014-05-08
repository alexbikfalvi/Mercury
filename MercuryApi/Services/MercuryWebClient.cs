using Mercury.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Mercury.Services
{
    public class MercuryWebClient
    {


        //public static void Main(string[] args)
        //{
        //    //User user = GetUser();
        //    //Console.WriteLine("User name is: "+user.Name );


        //    //My Info
        //    MyInfo myInfo = GetMyInfo();
        //    Console.WriteLine("My as is " + myInfo.As + " and my AS name is: " + myInfo.ASName );

        //    //AS Relationships
        //    ASRelationship asRelationship = GetASRelationship();
        //    Console.WriteLine("AS Relationship between AS0 " + asRelationship.As0 + " and AS1 " + asRelationship.As1 + " is "+asRelationship.Relationship);

        //    Console.WriteLine("\nAS Relationships:");
        //    List<ASRelationship> asRelationships = GetASRelationships();
        //    foreach(ASRelationship asRel in asRelationships){
        //        Console.WriteLine("AS rels between " + asRel.As0 + " and : " + asRel.As1 + " is " + asRel.Relationship);
        //    }


        //    //IP-2-AS Mappings
        //    Console.WriteLine("\nIP-2-AS Mappings:");
        //    List<List<Ip2AsnMapping>> auxMappings = GetIp2AsnMappings();
        //    foreach (List<Ip2AsnMapping> ip2AsnMappings in auxMappings)
        //    {
        //        foreach (Ip2AsnMapping ip2AsnMapping in ip2AsnMappings)
        //        {
        //            Console.WriteLine("IP-2-AS for ip: " + ip2AsnMapping.Ip + " is AS " + ip2AsnMapping.As + " - " + ip2AsnMapping.AsName);
        //        }
        //    }

        //    //IP-2-Geo mappings
        //    Console.WriteLine("\nIP-2-GEO Mappings:");
        //    List<Ip2GeoMapping> ip2GeoMappings = GetIp2GeoMappings();
        //    foreach (Ip2GeoMapping ip2GeoMapping in ip2GeoMappings)
        //    {
        //        Console.WriteLine("IP-2-GEO for ip " + ip2GeoMapping.Ip + " is country : " + ip2GeoMapping.CountryName + " and city: " + ip2GeoMapping.City);
        //    }


        //    //Getting TracerouteASes by destination
        //    Console.WriteLine("\nGet TracerouteASes by Destination:");
        //    List<TracerouteAS> tracerouteASes = GetTracerouteASesByDst();
        //    foreach (TracerouteAS tracerouteAS in tracerouteASes)
        //    {
        //        Console.WriteLine("Traceroute AS DomainDst: " + tracerouteAS.dst + " from sourceAS: " + tracerouteAS.srcAS + " and destAS: " + tracerouteAS.dstAS + " and asHops: " +tracerouteAS.tracerouteASStats.asHops);
        //        foreach (TracerouteASRelationship trRel in tracerouteAS.tracerouteASRelationships)
        //        {
        //            Console.WriteLine("Hop " + trRel.hop + " has a relationship " + trRel.relationship);
        //        }
        //        foreach (String attemptId in tracerouteAS.tracerouteIpAttemptIds)
        //        {
        //            Console.WriteLine("Attempt id: "+attemptId);
        //        }
        //        foreach(TracerouteASHop trHop in tracerouteAS.tracerouteASHops){
        //            Console.WriteLine("Hop id: " + trHop.hop+" of AS "+trHop.asName+" of type "+trHop.type+" has been inferred "+trHop.inferred);
        //        }
        //    }

        //    //Adding TracerouteSettings
        //    Console.WriteLine("Adding one Traceroute Settings: " + addTracerouteSettings());
        //    Console.WriteLine("Adding another Traceroute Settings: " + addTracerouteSettings());


        //    Console.Write("\nPress any key to quit!");
        //    ConsoleKeyInfo cki = Console.ReadKey(); 
        //}

        public static User GetUser()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("http://coderwall.com/mdeiters.json");
                var user = JsonConvert.DeserializeObject<User>(json);

                return user;
            }
        }

        public static MyInfo GetMyInfo()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("http://mercury.upf.edu/mercury/api/services/myInfo");
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var myInfo = JsonConvert.DeserializeObject<MyInfo>(json);

                return myInfo;
            }
        }

        public static ASRelationship GetASRelationship()
        {
            using (WebClient wc = new WebClient())
            {
                int as0 = 2;
                int as1 = 3;
                var json = wc.DownloadString("http://mercury.upf.edu/mercury/api/services/getASRelationship/"+as0+"/"+as1);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var asRelationship = JsonConvert.DeserializeObject<ASRelationship>(json);

                return asRelationship;
            }
        }

        public static List<ASRelationship> GetASRelationships()
        {
            using (WebClient wc = new WebClient())
            {
                string myParameters = "pairs=2-3,766-3356,2589-6985";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var json = wc.UploadString("http://mercury.upf.edu/mercury/api/services/getASRelationshipsPOST", myParameters);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var asRelationships = JsonConvert.DeserializeObject<List<ASRelationship>>(json);

                return asRelationships;
            }
        }


        public static List<List<Ip2AsnMapping>> GetIp2AsnMappings()
        {
            using (WebClient wc = new WebClient())
            {
                string myParameters = "ips=193.145.48.3,8.8.8.85";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var json = wc.UploadString("http://mercury.upf.edu/mercury/api/services/getIp2AsnMappingsByIpsPOST", myParameters);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var ip2AsnMappings = JsonConvert.DeserializeObject<List<List<Ip2AsnMapping>>>(json);

                return ip2AsnMappings;
            }
        }


        public static List<Ip2GeoMapping> GetIp2GeoMappings()
        {
            using (WebClient wc = new WebClient())
            {
                string myParameters = "ips=193.145.48.3,8.8.8.85";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var json = wc.UploadString("http://mercury.upf.edu/mercury/api/services/getIps2GeoPOST", myParameters);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var ip2GeoMappings = JsonConvert.DeserializeObject<List<Ip2GeoMapping>>(json);

                return ip2GeoMappings;
            }
        }

        public static List<TracerouteAS> GetTracerouteASesByDst()
        {
            using (WebClient wc = new WebClient())
            {
                String dst = "yimg.com";
                var json = wc.DownloadString("http://mercury.upf.edu/mercury/api/services/getTracerouteASesByDst/" + dst );
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var tracerouteASes = JsonConvert.DeserializeObject<List<TracerouteAS>>(json);

                return tracerouteASes;
            }
        }

        public static String addTracerouteSettings()
        {
            using (WebClient wc = new WebClient())
            {
                string myData = generateTracerouteSettings();
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = wc.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteSettingsPOST", myData);
                //Console.WriteLine("**************\n"+result+"\n*********************");

                return result;
            }
        }


        public static String generateTracerouteSettings()
        {
            String UUID = Guid.NewGuid().ToString();
            Console.WriteLine("UUID generated: " + UUID);
            TracerouteSettings ts = new TracerouteSettings(
                UUID, 3, 3, 0, 32, 500, 1000, 35000, 36000, 20);
            string json = JsonConvert.SerializeObject(ts);
            return json;
            
        }


        public static String generateTracerouteIp()
        {
            //First we create 4 Ip hops
            //TracerouteIpHop thop1 = new TracerouteIpHop(TracerouteIpHop.State.REQ_SENT, TracerouteIpHop.Type.TIME_EXCEEDED, "192.168.1.2", 1);
            //      )

            return null;
        }
    }
}
