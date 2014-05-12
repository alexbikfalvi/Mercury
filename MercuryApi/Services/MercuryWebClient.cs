using Mercury.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Mercury.Services
{
    class MercuryWebClient
    {


        //public static void Main(string[] args)
        //{
        //    //User user = GetUser();
        //    //Console.WriteLine("User name is: "+user.Name );


        //    //My Info
        //    MyInfo myInfo = GetMyInfo();
        //    Console.WriteLine("My as is " + myInfo.As + " and my AS name is: " + myInfo.ASName);

        //    //AS Relationships
        //    ASRelationship asRelationship = GetASRelationship();
        //    Console.WriteLine("AS Relationship between AS0 " + asRelationship.As0 + " and AS1 " + asRelationship.As1 + " is " + asRelationship.Relationship);

        //    Console.WriteLine("\nAS Relationships:");
        //    List<ASRelationship> asRelationships = GetASRelationships();
        //    foreach (ASRelationship asRel in asRelationships)
        //    {
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
        //        Console.WriteLine("Traceroute AS DomainDst: " + tracerouteAS.dst + " from sourceAS: " + tracerouteAS.srcAS + " and destAS: " + tracerouteAS.dstAS + " and asHops: " + tracerouteAS.tracerouteASStats.asHops);
        //        foreach (TracerouteASRelationship trRel in tracerouteAS.tracerouteASRelationships)
        //        {
        //            Console.WriteLine("Hop " + trRel.hop + " has a relationship " + trRel.relationship);
        //        }
        //        foreach (String attemptId in tracerouteAS.tracerouteIpAttemptIds)
        //        {
        //            Console.WriteLine("Attempt id: " + attemptId);
        //        }
        //        foreach (TracerouteASHop trHop in tracerouteAS.tracerouteASHops)
        //        {
        //            Console.WriteLine("Hop id: " + trHop.hop + " of AS " + trHop.asName + " of type " + trHop.type + " has been inferred " + trHop.inferred);
        //        }
        //    }

        //    //Adding TracerouteSettings
        //    Console.WriteLine("Adding one Traceroute Settings... obtained id: " + addTracerouteSettings());
        //    Console.WriteLine("Adding another Traceroute Settings... obtained id: " + addTracerouteSettings());

        //    //Adding TracerouteIp
        //    Console.WriteLine("Adding one TracerouteIp... the response is:\n" + addTracerouteIp());

        //    //Adding TracerouteAS
        //    Console.WriteLine("Adding one TracerouteAS... the response is:\n" + addTracerouteAS());

        //    //Adding TracerouteASes
        //    Console.WriteLine("Adding one TracerouteASes... the response is:\n" + addTracerouteASes());

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

        public static ASRelationship GetASRelationship(int as0, int as1)
        {
            using (WebClient wc = new WebClient())
            {
                //int as0 = 2;
                //int as1 = 3;
                var json = wc.DownloadString("http://mercury.upf.edu/mercury/api/services/getASRelationship/" + as0 + "/" + as1);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var asRelationship = JsonConvert.DeserializeObject<ASRelationship>(json);

                return asRelationship;
            }
        }

        public static List<ASRelationship> GetASRelationships(string myParameters)
        {
            using (WebClient wc = new WebClient())
            {
                //string myParameters = "pairs=2-3,766-3356,2589-6985";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var json = wc.UploadString("http://mercury.upf.edu/mercury/api/services/getASRelationshipsPOST", myParameters);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var asRelationships = JsonConvert.DeserializeObject<List<ASRelationship>>(json);

                return asRelationships;
            }
        }


        public static List<List<Ip2AsnMapping>> GetIp2AsnMappings(string myParameters)
        {
            using (WebClient wc = new WebClient())
            {
                //string myParameters = "ips=193.145.48.3,8.8.8.85";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var json = wc.UploadString("http://mercury.upf.edu/mercury/api/services/getIp2AsnMappingsByIpsPOST", myParameters);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var ip2AsnMappings = JsonConvert.DeserializeObject<List<List<Ip2AsnMapping>>>(json);

                return ip2AsnMappings;
            }
        }


        public static List<Ip2GeoMapping> GetIp2GeoMappings(string myParameters)
        {
            using (WebClient wc = new WebClient())
            {
                //string myParameters = "ips=193.145.48.3,8.8.8.85";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var json = wc.UploadString("http://mercury.upf.edu/mercury/api/services/getIps2GeoPOST", myParameters);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var ip2GeoMappings = JsonConvert.DeserializeObject<List<Ip2GeoMapping>>(json);

                return ip2GeoMappings;
            }
        }

        public static List<TracerouteAS> GetTracerouteASesByDst(String dst)
        {
            using (WebClient wc = new WebClient())
            {
                //String dst = "yimg.com";
                var json = wc.DownloadString("http://mercury.upf.edu/mercury/api/services/getTracerouteASesByDst/" + dst);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var tracerouteASes = JsonConvert.DeserializeObject<List<TracerouteAS>>(json);

                return tracerouteASes;
            }
        }



        public static TracerouteAS generateTracerouteAS()
        {

            //First we create 3 AS hops. Notice that we hace two HOPs number 1. 
            //     This is because we use the inferrence algorithm "true" to select the best hop
            TracerouteASHop hop0 = new TracerouteASHop(0, 3352, "Telefonica de Espana", "", TracerouteASHop.Type.AS, false);
            TracerouteASHop hop1 = new TracerouteASHop(1, 12956, "Telefonica Backbone", "", TracerouteASHop.Type.AS, true);
            TracerouteASHop hop2 = new TracerouteASHop(1, 3356, "Level-3", "", TracerouteASHop.Type.AS, false);
            TracerouteASHop hop3 = new TracerouteASHop(2, 10310, "Yahoo-3", "", TracerouteASHop.Type.AS, false);


            //Then we create the 2 AS Relationships
            TracerouteASRelationship trel0 = new TracerouteASRelationship(TracerouteASRelationship.Relationship.S2S, 3352, 12956, 0);
            TracerouteASRelationship trel1 = new TracerouteASRelationship(TracerouteASRelationship.Relationship.P2P, 12956, 10310, 1);

            //Now we create the TracerouteStats. Notice the flags value set to 2 because we have used the inference algorithm. 
            //  We have to decide which are the binary flags (int32bit)
            TracerouteASStats tstats = new TracerouteASStats(2, 0, 1, 0, 1, 0, 0, true, 2);

            //Finally we generate the TracerouteAS
            TracerouteAS tas = new TracerouteAS(3352, "Telefonica de Espana", "192.168.1.2",
                "80.33.0.24", "Barcelona", "Spain", 10310,
                "Yahoo-3", "98.139.102.145", "yimg.com", "Sunny Valley", "United States", tstats);

            tas.tracerouteASHops.Add(hop0);
            tas.tracerouteASHops.Add(hop1);
            tas.tracerouteASHops.Add(hop2);
            tas.tracerouteASHops.Add(hop3);

            tas.tracerouteASRelationships.Add(trel0);
            tas.tracerouteASRelationships.Add(trel1);

            return tas;

        }

        public static String addTracerouteAS(TracerouteAS tracerouteAS)
        {
            using (WebClient wc = new WebClient())
            {
                //We serialize
                //string myData = JsonConvert.SerializeObject(generateTracerouteAS());
                string myData = JsonConvert.SerializeObject(tracerouteAS);
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = wc.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteASPOST", myData);
                //Console.WriteLine("**************\n"+result+"\n*********************");

                return result;
            }
        }

        public static String addTracerouteASes(List<TracerouteAS> tases)
        {
            using (WebClient wc = new WebClient())
            {
                //We add an array with 3 TracerouteAS(es)
                //List<TracerouteAS> tases = new List<TracerouteAS>();
                //tases.Add(generateTracerouteAS());
                //tases.Add(generateTracerouteAS());
                //tases.Add(generateTracerouteAS());

                string myData = JsonConvert.SerializeObject(tases);
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = wc.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteASesPOST", myData);
                //Console.WriteLine("**************\n"+result+"\n*********************");

                return result;
            }
        }


        public static TracerouteSettings generateTracerouteSettings()
        {
            String UUID = Guid.NewGuid().ToString();
            Console.WriteLine("UUID generated: " + UUID);
            TracerouteSettings ts = new TracerouteSettings(
                UUID, 3, 3, 0, 32, 500, 1000, 35000, 36000, 20);
            return ts;

        }

        public static String addTracerouteSettings(TracerouteSettings tracerouteSettings)
        {
            using (WebClient wc = new WebClient())
            {

                //string myData = JsonConvert.SerializeObject(generateTracerouteSettings());
                string myData = JsonConvert.SerializeObject(tracerouteSettings);
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = wc.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteSettingsPOST", myData);
                //Console.WriteLine("**************\n"+result+"\n*********************");

                return result;
            }
        }


        public static TracerouteIp generateTracerouteIp()
        {
            //First we create 4 Ip hops
            TracerouteIpHop thop0 = new TracerouteIpHop(TracerouteIpHop.State.REQ_SENT, TracerouteIpHop.Type.ECHO_REPLY, "192.168.1.1", 1);
            TracerouteIpHop thop1 = new TracerouteIpHop(TracerouteIpHop.State.REQ_SENT, TracerouteIpHop.Type.ECHO_REPLY, "194.224.58.10", 2);
            TracerouteIpHop thop2 = new TracerouteIpHop(TracerouteIpHop.State.REQ_SENT, TracerouteIpHop.Type.ECHO_REPLY, "84.16.0.1", 3);
            TracerouteIpHop thop3 = new TracerouteIpHop(TracerouteIpHop.State.REQ_SENT, TracerouteIpHop.Type.ECHO_REPLY, "98.139.102.145", 4);

            //We create 1 attempt
            TracerouteIpAttempt tatt = new TracerouteIpAttempt(TracerouteIpAttempt.State.COMPLETED, 4, Guid.NewGuid().ToString());

            tatt.tracerouteIpHops.Add(thop0);
            tatt.tracerouteIpHops.Add(thop1);
            tatt.tracerouteIpHops.Add(thop2);
            tatt.tracerouteIpHops.Add(thop3);

            //We create 1 flow
            TracerouteIpFlow tflow = new TracerouteIpFlow(TracerouteIpFlow.Algorithm.ICMP, Guid.NewGuid().ToString());
            tflow.tracerouteIpAttemps.Add(tatt);

            //We create 1 traceroute
            TracerouteIp tip = new TracerouteIp("192.168.1.2", "80.33.0.24", "98.139.102.145", "yimg.com", addTracerouteSettings(generateTracerouteSettings()) );
            tip.tracerouteIpFlows.Add(tflow);

            return tip;
        }


        public static String addTracerouteIp(TracerouteIp tracerouteIp)
        {
            using (WebClient wc = new WebClient())
            {
                //string myData = JsonConvert.SerializeObject(generateTracerouteIp());
                string myData = JsonConvert.SerializeObject(tracerouteIp);
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = wc.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteIpPOST", myData);
                //Console.WriteLine("**************\n"+result+"\n*********************");

                return result;
            }
        }


        /*Not yet implemented in Mercury Server*/
        //public static String addTracerouteIps()
        //{
        //    using (WebClient wc = new WebClient())
        //    {
        //        List<TracerouteIp> tips = new List<TracerouteIp>();
        //        tips.Add(generateTracerouteIp());
        //        tips.Add(generateTracerouteIp());
        //        tips.Add(generateTracerouteIp());

        //        string myData = JsonConvert.SerializeObject(tips);
        //        wc.Headers[HttpRequestHeader.ContentType] = "application/json";
        //        var result = wc.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteIpPOST", myData);
        //        //Console.WriteLine("**************\n"+result+"\n*********************");

        //        return result;
        //    }
        //}

    }
}
