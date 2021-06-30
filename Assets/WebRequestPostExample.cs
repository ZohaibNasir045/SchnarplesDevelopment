using System;
using System.IO;
using System.Net;
using System.Text;
// Token authentication example in C# using the RTM user events RESTful API
namespace Examples.System.Net
{
    public class WebRequestPostExample
    {
        public static void Main()
        {

            // RTM Token
            string token = "Your RTM Token";
            // User ID used to generate the RTM token
            string uid = "userA";
            // Add the x-agora-token field to the header
            string tokenHeader = "x-agora-token: " + token;
            // Add the x-agora-uid field to the header
            string uidHeader = "x-agora-uid: " + uid;

            WebRequest request = WebRequest.Create("https://api.agora.io/dev/v2/project/<Your App ID>/rtm/vendor/user_events");
            request.Method = "GET";

            // Add header to the request
            request.Headers.Add(tokenHeader);
            request.Headers.Add(uidHeader);

            request.ContentType = "application/json";

            // Get response
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);


            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
            }

            response.Close();
        }
    }
}