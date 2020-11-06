using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfoManagementSystem.Models
{
    public class formFieldsModel
    {
        public int custId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime createdOn { get; set; }
        public string country { get; set; }
        public string email { get; set; }
        public string favProduct { get; set; }
        public double prodPrice { get; set; }
        public double pastPurchaseAmt { get; set; }
        public double finalAmt { get; set; }
        public int perDiscQualified { get; set; }
        public string status { get; set; }
    }
}