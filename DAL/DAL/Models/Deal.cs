using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class Deal
    {
        public string SellerName { get; set; }

        public string SellerInn { get; set; }

        public string BuyerName { get; set; }

        public string BuyerInn { get; set; }

        public double WoodVolumeBuyer { get; set; }

        public double WoodVolumeSeller { get; set; }

        public DateTime DealDate { get; set; }

        public string DealNumber { get; set; }

        public Deal(string sellerName, string sellerInn, string buyerName, string buyerInn, double woodVolumeBuyer, double woodVolumeSeller, DateTime dealDate, string dealNumber)
        {
            SellerName = sellerName;
            SellerInn = sellerInn;
            BuyerName = buyerName;
            BuyerInn = buyerInn;
            WoodVolumeBuyer = woodVolumeBuyer;
            WoodVolumeSeller = woodVolumeSeller;
            DealDate = dealDate;
            DealNumber = dealNumber;
        }

        public static explicit operator Deal(Content content)
        {
            // like yyy-mm-dd
            var dateString = content.DealDate;
            var date = dateString.Split('-');
            var dateOfDeal = new DateTime(
                Int32.Parse(date[0]), 
                Int32.Parse(date[1]), 
                Int32.Parse(date[2]));

            return new Deal(
                content.SellerName,
                content.SellerInn,
                content.BuyerName,
                content.BuyerInn,
                content.WoodVolumeBuyer,
                content.WoodVolumeSeller,
                dateOfDeal,
                content.DealNumber);
        }

        public static IList<Deal> ContentListToDealList(IList<Content> contents)
        {
            var deals = new List<Deal>();

            foreach (var content in contents)
            {
                deals.Add((Deal) content);
            }

            return deals;
        }
    }
}