using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace AlbionOnlineCraftingCalculator
{
    public class UIModel
    {

        public InputParameters InputParameters { get; set; }


        public List<SimplifiedItem> simplifiedItems { get; set; }




    }

    public class UIModelV2
    {

        public InputParameters InputParameters { get; set; }


        public List<SimplifiedItemV2> simplifiedItems { get; set; }


        public List<AlbionDataPriceModel> albionDataPriceModels { get; set; }


        public Journalitem Journal { get; set; }







    }

    public class FocusCostCalculcationModel
    {
        public string UniqueName { get; set; } = "";

        public int FocusCost { get; set; } = 0;

        public int UserSpecInput { get; set; } = 0;

        public bool ArtefactItem { get; set; } = false;
    }


    public class InputParameters
    {
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public int Tier { get; set; }
        public int Enchantment { get; set; }
        public string SellingLocation { get; set; }
        public string BuyingLocation { get; set; }

        public double ReturnRate { get; set; }

        public int NutritionFee { get; set; }

        public int MainTreeSpec { get; set; }


    }



    public class AlbionCraftingInformation
    {
        public double EndproductSellingPrice { get; set; }
        public double NutritionCost { get; set; } = 0;

        public double SellorderCost { get; set; } = 0;

        public Resources Resources { get; set; } = new Resources();

        public Fame Fame { get; set; } = new Fame();

        public Journal Journal { get; set; } = new Journal();

        public Profit Profit { get; set; } = new Profit();

        public void CalculateProfit(bool hasJournal)
        {
            double investment = Resources.ResourceSilverInvested;
            double returned = Resources.ResourceSilverReturn;
            double resourceCost = investment - returned;
            //Debug.WriteLine($"ResourceCost: {Resources.ResourceSilverCost}");

            double journalProfit = 0;
            double nutritionCost = 0;
            double sellOrderTax = 0;
            if (hasJournal)
            {
                // Journal profit is a bit hard, since if u craft 2 items, and you buy 1 journal,   the journal value is not going double. (empty)
                journalProfit = (Journal.JournalFullPrice - Journal.JournalEmptyPrice) * Journal.JournalFilledPercentage;
                //Debug.WriteLine($"JournalProfit:{journalProfit}");
                
            }
            else
            {
                Journal.JournalEmptyPrice = 0;
                Journal.JournalFilledPercentageValue = 0;
            }

            nutritionCost = NutritionCost;
            sellOrderTax = SellorderCost;


            
            double costs = resourceCost + Journal.JournalEmptyPrice + nutritionCost + sellOrderTax;
            Debug.WriteLine($"Costs: {costs}");
            double sales = EndproductSellingPrice + Journal.JournalFilledPercentageValue;
            Debug.WriteLine($"Sales: {sales}");
            Profit.Investment = costs;
            Profit.Returned = sales;

            if (costs < sales) { /*Debug.WriteLine($"Profitable");*/ Profit.IsProfitable = true; }
            else { /*Debug.WriteLine($"Not Profitable");*/ Profit.IsProfitable = false; }
        }
    }

    public class Profit
    {
        public double Investment { get; set; }


        public double Returned { get; set; }

        public bool IsProfitable { get; set; }




    }

    public class ProfitUI
    {
        public string Investment { get; set; }


        public string Returned { get; set; }

        public bool IsProfitable { get; set; }

        public BitmapImage ProfitImage { get; set; }

        public string unknown { get; set; }


    }



    public class Resources
    {

        //double resourceCount = 0;

        //double resourceSilverInvested = 0;
        //double resourceAmountReturned = 0;


        //double resourceSilverReturn = 0;

        //double resourceSilverCost = 0;



        // ResourceCount = total amount of resources invested. (artefact included i think)
        //public double ResourceCount { get; set; } = 0;
        // ResourceSilverInvested = total amount of silver invested into crafting the item. (artefact included)
        public double ResourceSilverInvested { get; set; } = 0;

        //double ResourceReturnCount { get; set; } = 0;

        // ResourceSilverReturn = total amount of silver returned through the return rate of the craftingstation.
        public double ResourceSilverReturn { get; set; } = 0;


        // ResourceSilverCost = total amount of silver that went into the amount of materials that were not returned by the crafting station.
        public double ResourceSilverCost { get; set; } = 0;

        // ResourceReturnModels = List per resource how many of that particular resource were returned through crafting the item.
        public List<ResourceReturnModel> ResourceReturnModels = new List<ResourceReturnModel>();
    }

    public class Fame
    {
        public double TotalFameGained { get; set; }
        public double BaseFameGained { get; set; }
        public double ArtefactFameGained { get; set; }
        public double EnchantmentFameGained { get; set; }
    }

    public class Journal
    {
        public double JournalFilledPercentage { get; set; } = 0;
        public double JournalFullPrice { get; set; } = 0;

        public double JournalEmptyPrice { get; set; } = 0;

        public double JournalFilledPercentageValue { get; set; } = 0;
        public BitmapImage JournalImage { get; set; } = null;
    }




    public class ResourceReturnModel
    {
        public string UniqueName { get; set; }
        public int Count { get; set; }
    }



}
