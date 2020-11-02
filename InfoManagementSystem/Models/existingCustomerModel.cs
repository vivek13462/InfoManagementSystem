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
        private enum specialMonths { October = 15, November = 20, December = 25 };
        private const String conString = "Data Source=1009-sql-vivekt; Initital Catalog=UserMgmt; Integrated Security=False; UserID=***;Password=*** Connect Timeout=15; Encrypt=False; Packet Size=4096";

        private static String qry_custDetails = @"Select c.customerid,sum(total) as totalSale from customers c inner join orders o
                                                on c.customer = o.customerid where year(o.orderdate) = 2020 and c.customerid in ({0}) group by c.customerid";

        public List<fromFields> lstOnlineCustomers { get; set; }
        public List<fromFields> lstCustomerSales { get; set; }
        public Dictionary<string, int> prodDisc { get; set; }

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
                lstCustomerSales = new List<fromFields>();
                foreach(DataRow dr in dt.Rows)
                {
                    fromFields fdData = new fromFields();
                    fdData.custId = Int32.Parse(dr["customerid"].ToString());
                    fdData.totalSale = decimal.Parse(dr["totalSale"].ToString());
                    lstCustomerSales.Add(fdData);
                }
                lstCustomerSales.RemoveAll(v => v.totalSale < 1000);
                for(int i = 0; i < lstCustomerSales.Count; i++)
                {
                    lstCustomerSales[i].favProduct = lstOnlineCustomers.Where(c => c.custId == lstCustomerSales[i].custId).Select(x => x.favProduct).ToString();
                    lstCustomerSales[i].createdOn = DateTime.Parse(lstOnlineCustomers.Where(c => c.custId == lstCustomerSales[i].custId).Select(x => x.createdOn).ToString());
                    int percentagedisc = calcDisc(lstCustomerSales[i].favProduct);
                    lstCustomerSales[i].perDiscQualified = percentagedisc;
                    switch (lstCustomerSales[i].createdOn.Month)
                    {
                        case 10:
                            lstCustomerSales[i].perDiscQualified += (int)specialMonths.October;
                            break;
                        case 11:
                            lstCustomerSales[i].perDiscQualified += (int)specialMonths.November;
                            break;
                        case 12:
                            lstCustomerSales[i].perDiscQualified += (int)specialMonths.December;
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }
        
        public int calcDisc(string favProduct)
        {
            prodDisc.Add("iPhone", 10);
            prodDisc.Add("iWatch", 8);
            prodDisc.Add("iPad", 11);
            prodDisc.Add("AirPod", 5);

            int percentage = 0;
            int result;
            if(prodDisc.TryGetValue(favProduct, out result))
            {
                percentage = result;
            }
            else
            {
                percentage = 0;
            }
            return percentage;
        }
    }
}
