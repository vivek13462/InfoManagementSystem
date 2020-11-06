using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace InfoManagementSystem.Models
{
    public class existingCustomerModel
    {
        private enum specialMonths { October = 15, November = 20, December = 25 }; // Extra Discounts

        private const String conString = "Data Source=DESKTOP-GDIP1KV\\SQLEXPRESS;Initial Catalog=InfoMgmtSystem;Integrated Security=True";        
        public List<formFieldsModel> lstOnlineCustomers { get; set;}
        public List<formFieldsModel> lstCustomerSales { get; set; }
        public Dictionary<string, int> prodDisc { get; set; }

        public existingCustomerModel()
        {
            // Product and discount % on them
            prodDisc = new Dictionary<string, int>();
            prodDisc.Add("iPhone", 10);
            prodDisc.Add("iWatch", 8);
            prodDisc.Add("iPad", 11);
            prodDisc.Add("AirPod", 5);
        }

        public List<formFieldsModel> getCustDetails()
        {            
            formDataModel fdObj = new formDataModel();
            PriceModel pmObj = new PriceModel();
            lstOnlineCustomers = new List<formFieldsModel>();
            lstOnlineCustomers = fdObj.callJotFormAPI();
            string custIDs = string.Join(",", lstOnlineCustomers.Select(e => e.custId));
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    String strAppend = "";
                    String strIds = "";
                    int index = 1;
                    String paramName = "";
                    String[] strArrayNames;
                    strIds = custIDs;
                    strArrayNames = strIds.Split(',');
                    foreach (String item in strArrayNames)
                    {
                        paramName = "@custID" + index;
                        command.Parameters.AddWithValue(paramName, item); //Making individual parameters for every name  
                        strAppend += paramName + ",";
                        index += 1;
                    }
                    strAppend = strAppend.ToString().Remove(strAppend.LastIndexOf(","), 1); //Remove the last comma 
                    command.CommandText = @"select o.Customerid,sum(o.Total) as totalSale from Orders o inner join OrderDetails od
                                            on o.OrderId = od.OrderId where o.IsDelivered = 'YES' and od.PyamentReceived = 'YES' and 
                                            OrderDate >= DATEADD(Month, -3, GetDate()) and o.Customerid in (" + strAppend + ") group by o.Customerid";

                    SqlDataAdapter adapter = new SqlDataAdapter(command);                  

                    adapter.Fill(dt);
                }
                lstCustomerSales = new List<formFieldsModel>();
                foreach (DataRow dr in dt.Rows)
                {
                    formFieldsModel fdData = new formFieldsModel();
                    fdData.custId = Int32.Parse(dr["customerid"].ToString());
                    fdData.pastPurchaseAmt = double.Parse(dr["totalSale"].ToString());
                    lstCustomerSales.Add(fdData);
                }
                for(int i = 0; i < lstOnlineCustomers.Count; i++)
                {
                    var tempLst = lstCustomerSales.Where(c => c.custId == lstOnlineCustomers[i].custId).ToList();
                    double pastOrdersAmt = tempLst.Select(c => c.pastPurchaseAmt).FirstOrDefault();
                    int percentagedisc = calcDisc(lstOnlineCustomers[i].favProduct, pastOrdersAmt);
                    lstOnlineCustomers[i].perDiscQualified = percentagedisc;
                    switch (lstOnlineCustomers[i].createdOn.Month)
                    {
                        case 10:
                            lstOnlineCustomers[i].perDiscQualified += (int)specialMonths.October;
                            break;
                        case 11:
                            lstOnlineCustomers[i].perDiscQualified += (int)specialMonths.November;
                            break;
                        case 12:
                            lstOnlineCustomers[i].perDiscQualified += (int)specialMonths.December;
                            break;
                    }
                    if(lstOnlineCustomers[i].favProduct == "iPhone")
                    {
                        lstOnlineCustomers[i].prodPrice = pmObj.iPhone;
                    }
                    else if(lstOnlineCustomers[i].favProduct == "iWatch")
                    {
                        lstOnlineCustomers[i].prodPrice = pmObj.iWatch;
                    }
                    else if(lstOnlineCustomers[i].favProduct == "iPad")
                    {
                        lstOnlineCustomers[i].prodPrice = pmObj.iPad;
                    }
                    else
                    {
                        lstOnlineCustomers[i].prodPrice = pmObj.AirPod;
                    }
                    double perQual = lstOnlineCustomers[i].perDiscQualified;
                    double prodActualPrice = lstOnlineCustomers[i].prodPrice;
                    double DiscAmt = (perQual / 100) * prodActualPrice;
                    lstOnlineCustomers[i].finalAmt = lstOnlineCustomers[i].prodPrice - DiscAmt;
                    DiscAmt = Math.Round(DiscAmt, 2);
                    double discounted_cost = lstOnlineCustomers[i].prodPrice - DiscAmt;
                    discounted_cost = Math.Round(discounted_cost, 2);
                    lstOnlineCustomers[i].finalAmt = discounted_cost;
                }
                return lstOnlineCustomers;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        
        public int calcDisc(string favProduct, double pastPurchase)
        {       
            int percentage = 0;
            int result;
            if(prodDisc.TryGetValue(favProduct, out result) && pastPurchase >= 100)
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
