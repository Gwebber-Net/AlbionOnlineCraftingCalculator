using System.Collections.Generic;

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
        public string Location { get; set; }

        public double ReturnRate { get; set; }

        public int NutritionFee { get; set; }

        public int MainTreeSpec { get; set; }


        //public List<double> Specialisation { get; set; } = new List<double>();
    }



    public class AlbionCraftingInformation
    {

        public double NutritionCost { get; set; } = 0;

        public double SellorderCost { get; set; } = 0;

        public Resources Resources { get; set; } = new Resources();

        public Fame Fame { get; set; } = new Fame();

        public Journal Journal { get; set; } = new Journal();

       // public Focus Focus { get; set; } = new Focus();

    }

    public class Focus
    {

    }


    public class Resources
    {

        //double resourceCount = 0;

        //double resourceSilverInvested = 0;
        //double resourceAmountReturned = 0;


        //double resourceSilverReturn = 0;

        //double resourceSilverCost = 0;



        // ResourceCount = total amount of resources invested. (artefact included i think)
        public double ResourceCount { get; set; } = 0;
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
        public double JournalFilledPercentage { get; set; }
        public double JournalPrice { get; set; }
        public double JournalFilledPercentageValue { get; set; }
    }




    public class ResourceReturnModel
    {
        public string UniqueName { get; set; }
        public int Count { get; set; }
    }



}
