using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace DoAnTotNghiep.ViewModels
{
    public class ZaloViewModel{
        //public int appid { get; set; } = 554;
        //public string appuser { get; set; } = "hieugc";
        //public long apptime { get; set; } = DateTime.UtcNow.Ticks;
        //public int amount { get; set; } = 50000;
        //public string apptransid { get; set; } = string.Empty;
        //public string embeddata { get; set; } = string.Empty;
        //public string item { get; set; } = string.Empty;
        //public string mac { get; set; } = string.Empty;
        //public string bankcode { get; set; } = string.Empty;
        //public string description { get; set; } = string.Empty;
        //public string phone { get; set; } = string.Empty;
        //public string email { get; set; } = string.Empty;
        //public string address { get; set; } = string.Empty;
        //public string subappid { get; set; } = string.Empty;

        public Dictionary<string, string> CreateOrder(User user, HistoryTransaction transaction, string host, string key1, string AppId = "554")
        {
            Dictionary<string, string> model = new Dictionary<string, string>();
            var tranId = user.Id + "-transaction-" + transaction.Id;
            var embeddata = new ZaloEmbedData() { 
                RedirectUrl = host + "/Transaction/CallBack",
                zlppaymentid = transaction.Id.ToString() 
            };
            string item = JsonConvert.SerializeObject(transaction);

            model.Add("appid", AppId);
            model.Add("appuser", user.FirstName + " " + user.LastName);
            model.Add("apptime", ((long)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds).ToString());
            model.Add("amount", transaction.Amount.ToString());
            model.Add("apptransid", transaction.CreatedDate.ToString("yyMMdd") + "_" + tranId); // mã giao dich có định dạng yyMMdd_xxxx
            model.Add("embeddata", JsonConvert.SerializeObject(embeddata));
            model.Add("item", item);
            model.Add("description", "Tích điểm VExchange ZaloPay");
            model.Add("bankcode", "zalopayapp");


            var data = AppId.ToString() + "|" + model["apptransid"] + "|" + model["appuser"] + "|" + model["amount"] + "|"
               + model["apptime"] + "|" + model["embeddata"] + "|" + model["item"];
            model.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));


            return model;
        }

        public Dictionary<string, string> CheckOrder(string appid, string key1, HistoryTransaction transaction)
        {
            string apptransid = transaction.CreatedDate.ToString("yyMMdd") + "_" + transaction.IdUser + "-transaction-" + transaction.Id;
            var param = new Dictionary<string, string>();
            param.Add("appid", appid);
            param.Add("apptransid", apptransid);
            var data = appid + "|" + apptransid + "|" + key1;
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            return param;
        }
    }
    public class ZaloEmbedData
    {
        public string RedirectUrl { get; set; } = string.Empty;
        //public string? columninfo { get; set; } = string.Empty;
        //public string? promotioninfo { get; set; } = string.Empty;
        public string zlppaymentid { get; set; } = string.Empty;
    }
    public class ZaloResponseCreateOrder
    {
        public string zptranstoken { get; set; } = string.Empty;
        public string orderurl { get; set; } = string.Empty;
        public int returncode { get; set; } = 1;
        public string returnmessage { get; set; } = string.Empty;
    }

    public class ZaloResponseCheckOrder
    {
        public int returncode { get; set; } = 0;
        public string returnmessage { get; set; } = string.Empty;
        public bool isprocessing { get; set; } = false;
        public long amount { get; set; } = 0;
        public long discountamount { get; set; } = 0;
        public string zptransid { get; set; } = string.Empty;
    }

    public class CreateTransaction
    {
        [Required(ErrorMessage = "Hãy điền mệnh giá bạn muốn nạp")]
        [Range(1000, Double.MaxValue, ErrorMessage = "Gía trị nạp ít nhất 1000đ")]
        public int Price { get; set; } = 1000;
    }
}
