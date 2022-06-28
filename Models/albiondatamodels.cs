using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionOnlineCraftingCalculator
{
    
    public class AlbionDataWebRequestModel
    {
        public List<string> Items { get; set; }

        public string Locations { get; set; }

        public string Qualities { get; set; }


    }



   







    public class AlbionDataPriceModel
    {
        [JsonProperty("item_id")]
        public string ItemId { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("quality")]
        public int Quality { get; set; }

        [JsonProperty("sell_price_min")]
        public int SellPriceMin { get; set; }

        [JsonProperty("sell_price_min_date")]
        public DateTime SellPriceMinDate { get; set; }

        [JsonProperty("sell_price_max")]
        public int SellPriceMax { get; set; }

        [JsonProperty("sell_price_max_date")]
        public DateTime SellPriceMaxDate { get; set; }

        [JsonProperty("buy_price_min")]
        public int BuyPriceMin { get; set; }

        [JsonProperty("buy_price_min_date")]
        public DateTime BuyPriceMinDate { get; set; }

        [JsonProperty("buy_price_max")]
        public int BuyPriceMax { get; set; }

        [JsonProperty("buy_price_max_date")]
        public DateTime BuyPriceMaxDate { get; set; }
    }
}
