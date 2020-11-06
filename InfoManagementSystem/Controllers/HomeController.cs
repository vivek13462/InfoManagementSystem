using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.IO;

namespace InfoManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public List<Models.formFieldsModel> excelDataList { get; set; }
        public ActionResult Index()
        {
            Models.formDataModel fdObj = new Models.formDataModel();
            fdObj.callJotFormAPI();
            return View("Index", fdObj);
        }

        public ActionResult CheckDiscount()
        {
            Models.existingCustomerModel emObj = new Models.existingCustomerModel();
            emObj.getCustDetails();
            return View("DiscountView", emObj);
        }

        public void ExcelFile()
       {
           Models.formDataModel fdData = new Models.formDataModel();
           List<Models.formFieldsModel> LstData;
           excelDataList = new List<Models.formFieldsModel>();
           LstData = fdData.callJotFormAPI();

           for (int i = 0; i < LstData.Count; i++)
           {
               Models.formFieldsModel formFields = new Models.formFieldsModel();
               formFields.custId = Int32.Parse(LstData[i].custId.ToString());
               formFields.firstName = LstData[i].firstName;
               formFields.lastName = LstData[i].lastName;
               formFields.country = LstData[i].country;
               formFields.email = LstData[i].email;
               formFields.favProduct = LstData[i].favProduct;
               excelDataList.Add(formFields);
           }
           ExcelPackage excel = new ExcelPackage();
           var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
           workSheet.Cells[1, 1].LoadFromCollection(excelDataList, true);
           using (var rng = workSheet.Cells["A1:AE" + excelDataList.Count + 1])
           {
               rng.AutoFitColumns();
           }
           using (var memoryStream = new MemoryStream())
           {
               Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
               Response.AddHeader("content-disposition", "attachment;  filename=" + "ExcelReport _" + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Year + ".xlsx");
               excel.SaveAs(memoryStream);
               memoryStream.WriteTo(Response.OutputStream);
               Response.Flush();
               Response.End();
           }

       }
    }
}
