using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace InfoManagementSystem.Models
{
    public class existingCustomerModel
    {
        private const String conString = "Data Source=1009-sql-vivekt; Initital Catalog=UserMgmt; Integrated Security=False; UserID=***;Password=*** Connect Timeout=15; Encrypt=False; Packet Size=4096";

        private static String qry_custDetails = @"Select c.customerid,c.maincountry,cd.car from customers c inner join cardetails cd
                                                on c.customer = cd.customerid where c.customerid in ({0})";

        public List<fromFields> lstOnlineCustomers { get; set; }
        public List<fromFields> lstCustomerCarData { get; set; }

        public int getCustDetails()
        {
            formData fd = new formData();
            lstOnlineCustomers = new List<fromFields>();
            lstOnlineCustomers = fd.callJotAPI("");
            string custIDs = string.Join(",", lstOnlineCustomers.Select(e => e.custId).Distinct());
            try
            {
                SqlDataAdapter adap = new SqlDataAdapter(String.Format(qry_custDetails, custIDs),conString);
                DataTable dt = new DataTable();
                adap.Fill(dt);
                lstCustomerCarData = new List<fromFields>();
                foreach(DataRow dr in dt.Rows)
                {
                    fromFields fdData = new fromFields();
                    fdData.custId = Int32.Parse(dr["customerid"].ToString());
                    fdData.country = dr["maincountry"].ToString();
                    fdData.existingCar = dr["car"].ToString();
                    lstCustomerCarData.Add(fdData);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }        
    }
}