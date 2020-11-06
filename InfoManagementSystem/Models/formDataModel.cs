using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Globalization;
namespace InfoManagementSystem.Models
{
    public class formDataModel
    {
        private const String conString = "Data Source=DESKTOP-GDIP1KV\\SQLEXPRESS;Initial Catalog=InfoMgmtSystem;Integrated Security=True";
        private static String qry_cust = @"select * from customers";

        public int countUS { get; set; }
        public int countDE { get; set; }
        public int countSG { get; set; }
        public List<formFieldsModel> lstformData = new List<formFieldsModel>();
        
        public List<Models.formFieldsModel> callJotFormAPI()
        {

            try
            {
                SqlDataAdapter adap = new SqlDataAdapter(String.Format(qry_cust), conString);
                DataTable dt = new DataTable();
                adap.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    int x = Int32.Parse(dr["Customerid"].ToString());
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }



            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            WebRequest request = WebRequest.Create("https://api.jotform.com/form/203060722539046/submissions?apiKey=4ce97364480710d75dc4ba42c9e6658f&limit=1000"); // formid || APIKEY || limit=1000
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            JObject c = JObject.Parse(responseFromServer);
            JArray items = (JArray)c["content"];
            int count = items.Count;

            for (int i = 0; i < count; i++)
            {
                var userSubmission = new Models.formFieldsModel();
                // Fetching only required data from Jot form API

                if (c["content"][i]["created_at"] != null)
                {
                   userSubmission.createdOn = DateTime.Parse(c["content"][i]["created_at"].ToString());
                }
                userSubmission.status = c["content"][i]["status"].ToString();
                userSubmission.custId = Int32.Parse(c["content"][i]["answers"]["6"]["answer"] != null ? c["content"][i]["answers"]["6"]["answer"].ToString() : "");
                userSubmission.firstName = c["content"][i]["answers"]["3"]["answer"] != null ? c["content"][i]["answers"]["3"]["answer"]["first"].ToString() : "";
                userSubmission.lastName = c["content"][i]["answers"]["3"]["answer"] != null ? c["content"][i]["answers"]["3"]["answer"]["last"].ToString() : "";
                userSubmission.country = c["content"][i]["answers"]["8"]["answer"] != null ? c["content"][i]["answers"]["8"]["answer"].ToString() : "";
                userSubmission.favProduct = c["content"][i]["answers"]["7"]["answer"] != null ? c["content"][i]["answers"]["7"]["answer"].ToString() : "";
                userSubmission.email = c["content"][i]["answers"]["4"]["answer"] != null ? c["content"][i]["answers"]["4"]["answer"].ToString() : "";
                lstformData.Add(userSubmission);
            }
            lstformData.RemoveAll(x => x.status != "ACTIVE");

            lstformData = lstformData.GroupBy(g => new { g.custId, g.favProduct })
                         .Select(g => g.First())
                         .ToList();

            /*var customerInquiry = lstformData.GroupBy(s => new { s.custId, s.favProduct }).Select(g =>
                new
                {
                    custID = g.Key.custId,
                    favProd = g.Key.favProduct,
                    noOfInquiries = g.Count()
                }).ToList();*/            

            countUS = lstformData.Where(x => x.country == "United States").Count();
            countDE = lstformData.Where(x => x.country == "Germany").Count();
            countSG = lstformData.Where(x => x.country == "Singapore").Count();
            return lstformData;
        }
    }
}