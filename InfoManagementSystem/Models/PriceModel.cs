using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfoManagementSystem.Models
{
    public class PriceModel
    {
        public int iPhone { get; set; }
        public int iWatch { get; set; }
        public int iPad { get; set; }
        public int AirPod { get; set; }
        public PriceModel()
        {
            this.iPhone = 799;
            this.iPhone = 350;
            this.iPad = 750;
            this.AirPod = 250;
        }
        public void setPrice()
        {
            /*Dictionary<string, int> dictPrice = new Dictionary<string, int>();
            dictPrice.Add("iPhone",799);
            dictPrice.Add("iWatch", 350);
            dictPrice.Add("iPad", 750);
            dictPrice.Add("AirPod", 250);*/
        }         

        
        
    }
}