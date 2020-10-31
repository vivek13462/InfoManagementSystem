using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace InfoManagementSystem.Models
{
    public class formData
    {
        public List<Models.fromFields> lstformData = new List<fromFields>();
        
        public void callJotAPI()
        {
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            WebRequest request = WebRequest.Create("https://api.jotform.com/form/203045808099055/submissions?apiKey=4ce97364480710d75dc4ba42c9e6658f&limit=1000"); // formid || APIKEY || limit=1000
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            JObject c = JObject.Parse(responseFromServer);
            JArray items = (JArray)c["content"];
            int count = items.Count;
            //lstFormData = new List<DreamTeam5Info>();

            for (int i = 0; i < count; i++)
            {
                var userSubmission = new Models.fromFields();
                // Fetching only required data from Jot form API and hence commented extra stuff.

                if (c["content"][i]["created_at"] != null)
                {
                   userSubmission.createdOn = DateTime.Parse(c["content"][i]["created_at"].ToString());
                }
                userSubmission.status = c["content"][i]["status"].ToString();
                userSubmission.firstName = c["content"][i]["answers"]["3"]["answer"] != null ? c["content"][i]["answers"]["3"]["answer"]["first"].ToString() : "";
                userSubmission.lastName = c["content"][i]["answers"]["3"]["answer"] != null ? c["content"][i]["answers"]["3"]["answer"]["last"].ToString() : "";
                userSubmission.country = c["content"][i]["answers"]["8"]["answer"] != null ? c["content"][i]["answers"]["8"]["answer"].ToString() : "";
                userSubmission.favCar = c["content"][i]["answers"]["9"]["answer"] != null ? c["content"][i]["answers"]["9"]["answer"].ToString() : "";
                userSubmission.address = c["content"][i]["answers"]["5"]["answer"] != null ? c["content"][i]["answers"]["5"]["answer"]["addr_line1"].ToString() : "";
                userSubmission.city = c["content"][i]["answers"]["5"]["answer"] != null ? c["content"][i]["answers"]["5"]["answer"]["city"].ToString() : "";
                userSubmission.state = c["content"][i]["answers"]["5"]["answer"] != null ? c["content"][i]["answers"]["5"]["answer"]["state"].ToString() : "";
                userSubmission.zip = c["content"][i]["answers"]["5"]["answer"] != null ? c["content"][i]["answers"]["5"]["answer"]["postal"].ToString() : "";
                lstformData.Add(userSubmission);
            }
            lstformData.RemoveAll(x => x.status != "ACTIVE");
        }
    }
}
