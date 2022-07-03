using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Media.Imaging;


namespace AlbionOnlineCraftingCalculator
{
    public static class Methods
    {

        public static void SaveSettings(Setting settings)
        {
            string fullPath = $@".\settings.json";
            File.WriteAllText(fullPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }


        public static Setting UpdateSpecialisations(Setting setting, string specName, int index, int specValue)
        {
            for (int i = 0; i < setting.UserSpecInput.Count; i++)
            {
                if (setting.UserSpecInput[i].Name == specName)
                {
                    setting.UserSpecInput[i].Spec[index] = specValue;
                    return setting;
                }
            }
            return setting;
        }

        public static Setting UpdateMainTree(Setting setting, string specname, int mainTree)
        {
            for (int i = 0; i < setting.UserSpecInput.Count; i++)
            {
                if (setting.UserSpecInput[i].Name == specname)
                {
                    setting.UserSpecInput[i].MainTree = mainTree;
                    return setting;
                }
            }
            return setting;
        }
        public static UserSpecInput FindSetting(string settingName, List<UserSpecInput> settings)
        {

            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].Name == settingName)
                {
                    return settings[i];
                }
            }
            return new UserSpecInput() { Name = "NOTFOUND" };
        }


        public static List<SimplifiedItemV2> OpenSimplifiedItems()
        {
            string fullPath = $@".\simplifieditems.json";
            List<SimplifiedItemV2> simplifiedItemsV2 = new List<SimplifiedItemV2>();
            if (File.Exists(fullPath))
            {
                return OpenLocalFile();
            }
            else
            {
                return CreateListFromSeperateListsV2(fullPath);
            }




        }

        public static Setting OpenSettings(List<Shopcategory> shopcategories, List<string> shopcategoriesToBeIgnored, List<string> shopsubcategoriesToBeIgnored)
        {
            string fullPath = $@".\settings.json";
            Setting setting = new Setting();
            if (File.Exists(fullPath))
            {
                using (StreamReader r = new StreamReader(@".\settings.json"))
                {
                    string json = r.ReadToEnd();
                    setting = JsonConvert.DeserializeObject<Setting>(json);
                }
            }
            else
            {
                //File.Create(fullPath);
                setting = GenerateSettingsFile(shopcategories, shopcategoriesToBeIgnored, shopsubcategoriesToBeIgnored);
                File.WriteAllText(fullPath, JsonConvert.SerializeObject(setting, Formatting.Indented));

                return setting;
            }



            return setting;
        }

        public static Setting GenerateSettingsFile(List<Shopcategory> shopcategories, List<string> shopcategoriesToBeIgnored, List<string> shopsubcategoriesToBeIgnored)
        {





            Setting sRoot = new Setting();
            List<UserSpecInput> s = new List<UserSpecInput>();

            for (int i = 0; i < shopcategories.Count; i++)
            {
                if (!shopcategoriesToBeIgnored.Contains(shopcategories[i].Id))
                {
                    for (int j = 0; j < shopcategories[i].Shopsubcategory.Count; j++)
                    {

                        if (!shopsubcategoriesToBeIgnored.Contains(shopcategories[i].Shopsubcategory[j].id))
                        {
                            UserSpecInput setting = new UserSpecInput();
                            setting.Name = $"SPEC_{shopcategories[i].Id}_{shopcategories[i].Shopsubcategory[j].id}";


                            if (setting.Name == "SPEC_consumables_cooked")
                            {
                                for (int k = 0; k < 8; k++)
                                {
                                    setting.Spec.Add(0);
                                }
                            }
                            else
                            {

                                for (int k = 0; k < 7; k++)
                                {
                                    setting.Spec.Add(0);
                                }
                            }


                            s.Add(setting);
                        }

                    }


                }
            }

            sRoot.UserSpecInput = s;


            return sRoot;
        }



        public static AlbionCraftingInformation CalculateProfitV2(SimplifiedItemV2 simplifiedItemV2, int feePerHundredNutrition, double SellorderTax, string location, List<SimplifiedItemV2> simplifiedItemsV2, Journalitem journalitem, double returnRate)
        {
            const double nutritionUsedPerItemValue = 0.001125;
            List<double> fameMultiplier = new List<double> { 0, 1.5, 7.5, 22.5, 90, 270, 645, 1395 };




            List<ResourceReturnModel> resourceAmountReturnedPerResource = new List<ResourceReturnModel>();


            //Debug.WriteLine(returnRate);

            // Resource
            double resourceCount = 0;
            double resourceCountTotal = 0;
            double resourceSilverInvested = 0;
            double resourceSilverReturn = 0;
            double resourceSilverCost = 0;
            double resourceAmountReturned = 0;


            // Fame
            double baseFameGained = 0;
            double artefactFameGained = 0;
            double enchantmentFameGained = 0;


            // Nutrition
            double nutritionCost = 0;

            // AlbionCraftingInformationModel for returning to the MainThread
            AlbionCraftingInformation albionCraftingInformation = new AlbionCraftingInformation();


            for (int i = 0; i < simplifiedItemV2.Craftingrequirements[0].Craftresources.Count; i++)
            {
                ////////////////////////////////////////////////////
                ///   Calculate the total amount of resources needed.
                ///   Calculate the value of the investment and return
                ///   
                ////////////////////////////////////////////////////
                double _resourceCount = simplifiedItemV2.Craftingrequirements[0].Craftresources[i].Count;
                resourceCountTotal += _resourceCount;
                // si = the resource being used for ze crafting.
                SimplifiedItemV2 si = FindSimplifiedItem(simplifiedItemV2.Craftingrequirements[0].Craftresources[i].Uniquename, simplifiedItemsV2);
                // price is the price of that resource.
                int resourcePrice = GetPriceFromAlbionDataModel(si, location);
                // resourceMoney is the variable containing the money that went into buying the collection of resources.
                double _resourceSilverInvested = _resourceCount * resourcePrice;
                double _resourceReturnCount = Math.Floor(_resourceCount * returnRate);
                double _resourceSilverReturn = resourcePrice * _resourceReturnCount;
                double _resourceSilverCost = _resourceSilverInvested - _resourceSilverReturn;

                resourceAmountReturnedPerResource.Add(new ResourceReturnModel() { UniqueName = simplifiedItemV2.Craftingrequirements[0].Craftresources[i].Uniquename, Count = (int)_resourceReturnCount });


                ////////////////////////////////////////////////////
                ///
                ///   If there is an artefact, or royal token involved,
                ///
                ////////////////////////////////////////////////////
                if (simplifiedItemV2.Craftingrequirements[0].Craftresources[i].Uniquename.Contains("ARTEFACT") || simplifiedItemV2.Craftingrequirements[0].Craftresources[i].Uniquename.Contains("ROYALs"))
                {
                    // No aditional nutritionCosts needed
                    if (simplifiedItemV2.Craftingrequirements[0].Craftresources[i].Uniquename.Contains("ARTEFACT"))
                    {
                        artefactFameGained += 500;
                    }
                    else if (simplifiedItemV2.Craftingrequirements[0].Craftresources[i].Uniquename.Contains("ROYAL"))
                    {
                        if (simplifiedItemV2.Tier < 6)
                        {
                            artefactFameGained += (2.5 * resourceCountTotal * (simplifiedItemV2.Tier - 3));
                        }
                        else
                        {
                            artefactFameGained += (2.5 * resourceCountTotal * 4);
                        }
                    }
                }
                else
                {
                    double itemValue = FindSimplifiedItem(simplifiedItemV2.Craftingrequirements[0].Craftresources[0].Uniquename, simplifiedItemsV2).Itemvalue;
                    int amount = simplifiedItemV2.Craftingrequirements[0].Craftresources[0].Count;
                    nutritionCost += (double)(itemValue * amount * nutritionUsedPerItemValue * feePerHundredNutrition);
                    //baseFameGained += _resourceCount * fameMultiplier[simplifiedItemV2.Tier - 1];
                }
                ////////////////////////////////////////////////////
                ///
                ///   If there is an enchantment involved,
                ///
                ////////////////////////////////////////////////////

                // This might be removed, since it doesnt matter that much...
                resourceAmountReturned += _resourceReturnCount;
                resourceCount += _resourceCount;
                resourceSilverInvested = _resourceSilverInvested;
                resourceSilverReturn += _resourceSilverReturn;
                resourceSilverCost += _resourceSilverCost;
            }

            ////////////////////////////////////////////////////
            ///
            ///   Calculate the value, for putting up the sell order for the endproduct.
            ///
            ////////////////////////////////////////////////////
            ///\
            ///
            int endProductPrice = GetPriceFromAlbionDataModel(simplifiedItemV2, location);

            if (simplifiedItemV2.Craftingrequirements[0].Amountcrafted > 0)
            {
                endProductPrice = endProductPrice * simplifiedItemV2.Craftingrequirements[0].Amountcrafted;
            }
           
            double sellOrderPrice = Math.Floor(endProductPrice * SellorderTax);

           
                Debug.WriteLine($"Sellorder: {endProductPrice} * {SellorderTax} = {sellOrderPrice}");
            

            ////////////////////////////////////////////////////
            ///
            ///   Calculate the totalFameGained,
            ///   wich is being used for the journal calculation below
            ////////////////////////////////////////////////////

            baseFameGained += resourceCountTotal * fameMultiplier[simplifiedItemV2.Tier - 1];

            if (simplifiedItemV2.Enchantment != 0)
            {
                enchantmentFameGained += (simplifiedItemV2.Enchantment * (baseFameGained - (7.5 * resourceCountTotal)));
            }

            double totalFameGained = artefactFameGained + enchantmentFameGained + baseFameGained;



            ////////////////////////////////////////////////////
            ///
            ///   Calculate the amount of silver generated with filling the journal.
            ///
            ////////////////////////////////////////////////////
            ///
            if (journalitem != null)
            {
                double maxFame = journalitem.Maxfame;
                double percentageFilled = (totalFameGained / maxFame);
                double journalPrice = GetPriceFromItemsList(simplifiedItemsV2, journalitem.Uniquename, location);
                //Debug.WriteLine($"Price for {journalitem.Uniquename} in calculation method:{journalPrice}");
                double journalFilledPercentageValue = journalPrice * percentageFilled;

                albionCraftingInformation.Journal.JournalPrice = journalPrice;
                albionCraftingInformation.Journal.JournalFilledPercentage = percentageFilled;
                albionCraftingInformation.Journal.JournalFilledPercentageValue = journalFilledPercentageValue;
            }


            ////////////////////////////////////////////////////
            ///
            ///   Storing all calculated values inside the "AlbionCraftingInformation" model.
            ///
            ////////////////////////////////////////////////////
            albionCraftingInformation.NutritionCost = nutritionCost;
            albionCraftingInformation.SellorderCost = sellOrderPrice;

            albionCraftingInformation.Resources.ResourceCount = resourceCountTotal;
            albionCraftingInformation.Resources.ResourceSilverReturn = resourceSilverReturn;
            albionCraftingInformation.Resources.ResourceSilverInvested = resourceSilverInvested;
            albionCraftingInformation.Resources.ResourceSilverCost = resourceSilverCost;
            albionCraftingInformation.Resources.ResourceReturnModels = resourceAmountReturnedPerResource;

            albionCraftingInformation.Fame.TotalFameGained = totalFameGained;
            albionCraftingInformation.Fame.BaseFameGained = baseFameGained;
            albionCraftingInformation.Fame.ArtefactFameGained = artefactFameGained;
            albionCraftingInformation.Fame.EnchantmentFameGained = enchantmentFameGained;




            return albionCraftingInformation;

        }

        public static void CalculateProfit(SimplifiedItemV2 simplifiedItemV2, int feePerHundredNutrition, double SellorderTax, string location, List<SimplifiedItemV2> simplifiedItemsV2, Journalitem journalitem, double returnRate)
        {

            //Debug.WriteLine($"Calculating profit for {simplifiedItemV2.Uniquename} with enchantment {simplifiedItemV2.Enchantment}");


            const double nutritionUsedPerItemValue = 0.001125;

            List<Craftresource> craftresources = simplifiedItemV2.Craftingrequirements[0].Craftresources;


            if (craftresources.Count > 0)
            {
                int resourceCount = 0;

                int endProductPrice = Methods.GetPriceFromAlbionDataModel(simplifiedItemV2, location);
                double sellOrderCost = endProductPrice * SellorderTax;
                double nutritionCost = 0;

                switch (craftresources.Count)
                {
                    case 1:
                        {
                            resourceCount += craftresources[0].Count;
                            break;
                        }
                    case 2:
                        {
                            resourceCount += craftresources[0].Count;

                            if (craftresources[1].Uniquename.Contains("ARTEFACT") || craftresources[1].Uniquename.Contains("ROYAL"))
                            {
                                // NO RESOURCE COUNT ADDED
                            }
                            else
                            {
                                resourceCount += craftresources[1].Count;
                            }
                            break;
                        }
                    case 3:
                        {
                            resourceCount += craftresources[0].Count;
                            resourceCount += craftresources[1].Count;
                            break;
                        }
                    default:
                        break;
                }



                for (int i = 0; i < craftresources.Count; i++)
                {
                    int price = Methods.GetPriceFromItemsList(simplifiedItemsV2, simplifiedItemV2.Craftingrequirements[0].Craftresources[i].Uniquename, location);









                }




                // sellingReturn is what we receive back from the investment of crafting the item.
                double sellingReturn = endProductPrice - sellOrderCost - nutritionCost;

                //                                               1  2   3   4   5   6   7   8
                List<double> fameMultiplier = new List<double> { 0, 1.5, 7.5, 22.5, 90, 270, 645, 1395 };

                double baseFame = resourceCount * fameMultiplier[simplifiedItemV2.Tier - 1];



                double totalCraftingFame = baseFame;
                int enchantment = simplifiedItemV2.Enchantment;



                int amount = simplifiedItemV2.Craftingrequirements[0].Craftresources[0].Count;
                double itemValue = FindSimplifiedItem(simplifiedItemV2.Craftingrequirements[0].Craftresources[0].Uniquename, simplifiedItemsV2).Itemvalue;
                nutritionCost += (double)(itemValue * amount * nutritionUsedPerItemValue * feePerHundredNutrition);



                // No need for IF == 1, since we only have the materials, and the basefame we have calculated...


                if (simplifiedItemV2.Craftingrequirements[0].Craftresources.Count == 2)
                {
                    // basefame is known.
                    // Now we calculate the aditional fame for using OR "artefact", or "royal".


                    // Can be an artifact , or royal.
                    if (simplifiedItemV2.Craftingrequirements[0].Craftresources[1].Uniquename.Contains("ARTEFACT"))
                    {
                        totalCraftingFame += 500;
                    }
                    else if (simplifiedItemV2.Craftingrequirements[0].Craftresources[1].Uniquename.Contains("ROYAL"))
                    {
                        if (simplifiedItemV2.Tier < 6)
                        {
                            totalCraftingFame += (int)(2.5 * resourceCount * (simplifiedItemV2.Tier - 3));
                        }
                        else
                        {
                            totalCraftingFame += (int)(2.5 * resourceCount * 4);
                            // Calculate what the enchantment will add to the totalFame.
                            if (enchantment != 0)
                            {
                                // Debug.WriteLine("Calculating enchantment addition");

                                double fameGainedByEnchantment = enchantment * (baseFame - (7.5 * resourceCount));
                                totalCraftingFame += fameGainedByEnchantment;
                            }
                        }
                    }
                    else // its just materials.
                    {
                        amount = simplifiedItemV2.Craftingrequirements[0].Craftresources[0].Count;
                        itemValue = FindSimplifiedItem(simplifiedItemV2.Craftingrequirements[0].Craftresources[1].Uniquename, simplifiedItemsV2).Itemvalue;
                        nutritionCost += (double)(itemValue * amount * nutritionUsedPerItemValue * feePerHundredNutrition);
                    }
                }

                if (simplifiedItemV2.Craftingrequirements[0].Craftresources.Count == 3)
                {
                    // Can be an artifact , or royal.
                    // 0 and 1 are normal resources.
                    if (simplifiedItemV2.Craftingrequirements[0].Craftresources[2].Uniquename.Contains("ARTEFACT"))
                    {
                        totalCraftingFame += 500;
                    }
                    else if (simplifiedItemV2.Craftingrequirements[0].Craftresources[2].Uniquename.Contains("ROYAL"))
                    {
                        if (simplifiedItemV2.Tier < 6)
                        {
                            totalCraftingFame += (int)(2.5 * resourceCount * (simplifiedItemV2.Tier - 3));
                        }
                        else
                        {
                            totalCraftingFame += (int)(2.5 * resourceCount * 4);


                            // Calculate what the enchantment will add to the totalFame.
                            if (enchantment != 0)
                            {
                                //Debug.WriteLine("Calculating enchantment addition");
                                double fameGainedByEnchantment = enchantment * (baseFame - (7.5 * resourceCount));
                                totalCraftingFame += fameGainedByEnchantment;
                            }

                        }
                    }
                }

                double percentageFilled = 0;

                // This request needs to be combined with all other journals.
                double profitFromFillingBooks = 0;
                if (journalitem != null)
                {
                    int journalPrice = Methods.GetPriceFromItemsList(simplifiedItemsV2, journalitem.Uniquename, location);

                    percentageFilled = totalCraftingFame / (double)journalitem.Maxfame;
                    profitFromFillingBooks = (double)journalPrice * percentageFilled;
                    Debug.WriteLine($"Profit from filling book: {profitFromFillingBooks}");

                }







                // Need costs of the resources....

                double resourcePrice = 0;


                for (int i = 0; i < simplifiedItemV2.Craftingrequirements[0].Craftresources.Count; i++)
                {
                    SimplifiedItemV2 item = FindSimplifiedItem(simplifiedItemV2.Craftingrequirements[0].Craftresources[i].Uniquename, simplifiedItemsV2);

                    int price = GetPriceFromItemsList(simplifiedItemsV2, item.Uniquename, location);

                    resourcePrice += price * simplifiedItemV2.Craftingrequirements[0].Craftresources[i].Count;


                }
            }






            // Now we calculate based on the return rate, how much resources we will receive back.
            // Return rate only applies to resource....

            // It needs to be applied to each of the different resources independantly. afaik.







            //Debug.WriteLine($"Resources Cost:{resourcePrice}");
            //Debug.WriteLine($"Filling books made:{profitFromFillingBooks}");
            //Debug.WriteLine($"Selling returns:{sellingReturn}");


            // We know what the item will give us back in case of money....

            // We know what filling the book will give us.

        }


        public static SimplifiedItemV2 FindSimplifiedItem(string uniqueName, List<SimplifiedItemV2> simplifiedItemsV2)
        {


            for (int i = 0; i < simplifiedItemsV2.Count; i++)
            {
                if (simplifiedItemsV2[i].Uniquename == uniqueName)
                {
                    // We have a Match
                    return simplifiedItemsV2[i];
                }
            }

            return null;


        }






        public static Journalitem FindJournalForEndproduct(List<Journalitem> journalitems, string craftableUniqueName)
        {
            for (int i = 0; i < journalitems.Count; i++)
            {
                if (journalitems[i].Famefillingmissions.Craftitemfame != null)
                {
                    for (int j = 0; j < journalitems[i].Famefillingmissions.Craftitemfame.Validitem.Count; j++)
                    {



                        if (journalitems[i].Famefillingmissions.Craftitemfame.Validitem[j].Id == craftableUniqueName)
                        {
                            return journalitems[i];
                        }
                    }
                }




            }
            return null;
        }




        public static int GetPriceFromItemsList(List<SimplifiedItemV2> simplifiedItemsV2, string itemName, string location)
        {
            int price = 0;
            for (int i = 0; i < simplifiedItemsV2.Count; i++)
            {
                if (simplifiedItemsV2[i].Uniquename == itemName)
                {
                    //price = simplifiedItemsV2[i].AlbionDataPriceModels.SellPriceMin;
                    if (!(simplifiedItemsV2[i].AlbionDataPriceModelsV2.Count == 0))
                    {
                        for (int j = 0; j < simplifiedItemsV2[i].AlbionDataPriceModelsV2.Count; j++)
                        {
                            if (simplifiedItemsV2[i].AlbionDataPriceModelsV2[j].City == location)
                            {
                                // We found the match.
                                price = simplifiedItemsV2[i].AlbionDataPriceModelsV2[j].SellPriceMin;
                            }
                        }
                    }
                    else
                    {
                        // There are no prices to be found, since there are no prices stored for this item.
                        // price will remain 0
                    }

                }
            }
            return price;
        }





        public static int GetPriceFromAlbionDataModel(SimplifiedItemV2 simplifiedItemV2, string location)
        {
            for (int i = 0; i < simplifiedItemV2.AlbionDataPriceModelsV2.Count; i++)
            {
                if (simplifiedItemV2.AlbionDataPriceModelsV2[i].City == location)
                {
                    // we found a match
                    return simplifiedItemV2.AlbionDataPriceModelsV2[i].SellPriceMin;
                }
            }

            return 0;
        }


        public static BitmapImage DownloadImage(string url)
        {
            //Image image = new Image();
            var fullFilePath = url;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.EndInit();

            //image = (Image)bitmap;








            //image.Source = bitmap;
            //wrapPanel1.Children.Add(image);
            return bitmap;
        }



        //public static List<Simpleitem> ConvertItemsToResourcesOnly()
        //{
        //    string FileName = @"C:\Users\bart\Downloads\items.json";
        //    string FileNameOutput = @"C:\Users\bart\Downloads\items_simpleitems.json";



        //    AlbionItemDocument myDeserializedClass = JsonConvert.DeserializeObject<AlbionItemDocument>(File.ReadAllText(FileName));

        //    List<Simpleitem> simpleitems = myDeserializedClass.Items.Simpleitem;


        //    List<Simpleitem> NewList = new List<Simpleitem>();

        //    foreach (Simpleitem item in simpleitems)
        //    {
        //        if (item.Shopcategory == "resources")
        //        {

        //            if (item.Tier > 3)
        //            {
        //                switch (item.Shopsubcategory1)
        //                {
        //                    case "wood":
        //                        {
        //                            NewList.Add(item);
        //                            break;
        //                        }
        //                    case "rock":
        //                        {
        //                            NewList.Add(item);
        //                            break;
        //                        }
        //                    case "ore":
        //                        {
        //                            NewList.Add(item);
        //                            break;
        //                        }
        //                    case "fiber":
        //                        {
        //                            NewList.Add(item);
        //                            break;
        //                        }
        //                    case "hide":
        //                        {
        //                            NewList.Add(item);
        //                            break;
        //                        }


        //                    case "planks":
        //                        {
        //                            NewList.Add(item);
        //                            break;
        //                        }

        //                    case "stoneblock":
        //                        {
        //                            NewList.Add(item);
        //                            break;
        //                        }
        //                    case "metalbar":
        //                        {
        //                            NewList.Add(item);
        //                            break;
        //                        }
        //                    case "cloth":
        //                        {
        //                            NewList.Add(item);
        //                            break;
        //                        }
        //                    case "leather":
        //                        {
        //                            NewList.Add(item);
        //                            break;
        //                        }


        //                    case
        //                null:
        //                        break;
        //                }
        //            }
        //        }
        //        else
        //        {

        //        }
        //    }

        //    return NewList;


        //    //string json = JsonConvert.SerializeObject(NewList, Formatting.Indented);

        //    //File.WriteAllText(FileNameOutput, json);
        //}


        public static List<Journalitem> ConvertItemsJournalitems()
        {
            //string FileName = @"C:\Users\bart\Downloads\items.json";
            //AlbionItemDocument myDeserializedClass = JsonConvert.DeserializeObject<AlbionItemDocument>(File.ReadAllText(FileName));
            //string fileName = @".\simplifiedItemList.json";

            //AlbionItemDocument myDeserializedClass = JsonConvert.DeserializeObject<AlbionItemDocument>(File.ReadAllText(fileName));


            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "AlbionOnlineCraftingCalculator.Files.items.json";
            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            AlbionItemDocument myDeserializedClass = JsonConvert.DeserializeObject<AlbionItemDocument>(result);


            List<Journalitem> journals = new List<Journalitem>();

            List<Journalitem> journalitems = myDeserializedClass.Items.Journalitem;

            for (int i = 0; i < journalitems.Count; i++)
            {
                Journalitem item1 = journalitems[i];
                Journalitem item2 = journalitems[i];

                item1.EmptyName = item1.Uniquename + "_EMPTY";
                item1.Uniquename += "_FULL";


                journals.Add(item1);







            }






            //for (int i = 0; i < myDeserializedClass.Items.Journalitem.Count; i++)
            //{
            //    Journalitem item2 = myDeserializedClass.Items.Journalitem[i];
            //    item2.Uniquename += "_EMPTY";
            //    journals.Add(item2);

            //}

            return journals;

        }

        public static List<Shopcategory> ConvertItemsToCategories()
        {
            //string FileName = @"C:\Users\bart\Downloads\items.json";
            //string FileNameOutput = @"C:\Users\bart\Downloads\items_shopcategories.json";
            //string testlocation = @"C:\Users\bart\Downloads\items_equipmentitems_precheck.json";


            //AlbionItemDocument myDeserializedClass = JsonConvert.DeserializeObject<AlbionItemDocument>(File.ReadAllText(FileName));
            //string fileName = @".\simplifiedItemList.json";

            //AlbionItemDocument myDeserializedClass = JsonConvert.DeserializeObject<AlbionItemDocument>(File.ReadAllText(fileName));

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "AlbionOnlineCraftingCalculator.Files.items.json";
            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            AlbionItemDocument myDeserializedClass = JsonConvert.DeserializeObject<AlbionItemDocument>(result);


            List<Shopcategory> shopcategories = myDeserializedClass.Items.Shopcategories.Shopcategory;



            //File.WriteAllText(FileNameOutput, JsonConvert.SerializeObject(shopcategories, Formatting.Indented));

            return shopcategories;
        }


        public static List<string> CreateItemListForAlbionDataApiV2(List<string> existingList, string itemName)
        {

            //string names = existingList;



            if (!(existingList.Contains(itemName)))
            {
                existingList.Add(itemName);
            }

            return existingList;



        }

        public static string CreateAlbionDataUrl(AlbionDataWebRequestModel albionDataWebRequestModel)
        {

            string baseUrl = "https://www.albion-online-data.com/api/v2/stats/prices/";


            string items = "";

            for (int i = 0; i < albionDataWebRequestModel.Items.Count; i++)
            {
                items = items + albionDataWebRequestModel.Items[i] + ",";


            }

            string qualities = albionDataWebRequestModel.Qualities;
            string locations = albionDataWebRequestModel.Locations;


            // Compose URL

            string url = baseUrl + items + "?" + "locations=" + locations + "&" + "qualities=" + qualities;



            return url;
        }


        public static List<SimplifiedItemV2> UpdatePrices(List<AlbionDataPriceModel> albionDataPriceModels, List<SimplifiedItemV2> simplifiedItemsV2)
        {

            for (int j = 0; j < simplifiedItemsV2.Count; j++)
            {
                for (int i = 0; i < albionDataPriceModels.Count; i++)
                {

                    string name = albionDataPriceModels[i].ItemId;

                    if (name.Contains("LEVEL1@1")) { name = name.Replace("@1", ""); }
                    if (name.Contains("LEVEL2@2")) { name = name.Replace("@2", ""); }
                    if (name.Contains("LEVEL3@3")) { name = name.Replace("@3", ""); }




                    if (simplifiedItemsV2[j].Uniquename == name)
                    {
                        //Debug.WriteLine($"Updating price for{simplifiedItemsV2[j].Uniquename}");
                        // We found a match.
                        // Now we can determine, whether we update the price into this FILE.



                        simplifiedItemsV2[j] = UpdatePriceWhenValidV2(albionDataPriceModels[i], simplifiedItemsV2[j]);


                    }







                    // Exception for Ze Journals






                }
            }
            return simplifiedItemsV2;
        }


        //private static SimplifiedItemV2 UpdatePriceWhenValid(AlbionDataPriceModel albionDataPriceModel, SimplifiedItemV2 simplifiedItemV2)
        //{


        //    if (!(simplifiedItemV2.AlbionDataPriceModels.SellPriceMin == 0))
        //    {
        //        // First we check if the new value is NOT 0.
        //        if (!(albionDataPriceModel.SellPriceMin == 0))
        //        {

        //            int oldPrice = simplifiedItemV2.AlbionDataPriceModels.SellPriceMin;
        //            int NewPrice = albionDataPriceModel.SellPriceMin;
        //            // Now we check if the difference between the 2 items is not too big. 
        //            // Sometimes people spam the API with fake values.

        //            double maximumDeviation = 0.2 * simplifiedItemV2.AlbionDataPriceModels.SellPriceMin;

        //            if(!(NewPrice < oldPrice - maximumDeviation) && !(NewPrice > oldPrice + maximumDeviation))
        //            {
        //                // The new value sits in between the maximum allowed deviation.
        //                simplifiedItemV2.AlbionDataPriceModels.SellPriceMin = albionDataPriceModel.SellPriceMin;
        //                Debug.WriteLine($"UpdatingPrices Match : {simplifiedItemV2.Uniquename} {albionDataPriceModel.SellPriceMin}");
        //            }



        //        }
        //        else
        //        {
        //            // The retreived value was 0, so no sense in updating it.
        //        }
        //    }
        //    else
        //    {
        //        // We have no value as of now for the current price.
        //        // So we might aswell update the value that we have......
        //        // Since there is no benchmark to determine whether the value we receiving is valid.
        //        simplifiedItemV2.AlbionDataPriceModels = albionDataPriceModel;
        //        Debug.WriteLine($"UpdatingPrices Match : {simplifiedItemV2.Uniquename} {albionDataPriceModel.SellPriceMin}");
        //    }
        //    return simplifiedItemV2;
        //}



        private static SimplifiedItemV2 UpdatePriceWhenValidV2(AlbionDataPriceModel albionDataPriceModel, SimplifiedItemV2 simplifiedItemV2)
        {


            if (simplifiedItemV2.AlbionDataPriceModelsV2.Count > 0)
            {
                //Debug.WriteLine($"{simplifiedItemV2.Uniquename} has {simplifiedItemV2.AlbionDataPriceModelsV2.Count} known locations.");

                // Now we check if the difference between the 2 items is not too big. 
                // Sometimes people spam the API with fake values.


                // Look for the corresponding city
                bool priceModelExisted = false;
                for (int i = 0; i < simplifiedItemV2.AlbionDataPriceModelsV2.Count; i++)
                {
                    if (simplifiedItemV2.AlbionDataPriceModelsV2[i].City == albionDataPriceModel.City)
                    {
                        double maximumDeviation = simplifiedItemV2.AlbionDataPriceModelsV2[i].SellPriceMin * 0.2;
                        int oldPrice = simplifiedItemV2.AlbionDataPriceModelsV2[i].SellPriceMin;
                        int NewPrice = albionDataPriceModel.SellPriceMin;


                        if (!(NewPrice < oldPrice - maximumDeviation) && !(NewPrice > oldPrice + maximumDeviation))
                        {
                            simplifiedItemV2.AlbionDataPriceModelsV2[i] = albionDataPriceModel;
                        }
                        else
                        {
                            // Value received it outside of the maximum deviation.
                            // Meaning there is something wrong with the data received.
                        }
                        priceModelExisted = true;
                    }
                }

                if (!priceModelExisted)
                {
                    //Debug.WriteLine($"{simplifiedItemV2.Uniquename} had known locations, but a new one came in");

                    if (!(albionDataPriceModel.SellPriceMin == 0))
                    {
                        simplifiedItemV2.AlbionDataPriceModelsV2.Add(albionDataPriceModel);

                    }


                }
            }
            else
            {
                // We dont have any pricemodels for this item.
                if (!(albionDataPriceModel.SellPriceMin == 0))
                {
                    simplifiedItemV2.AlbionDataPriceModelsV2.Add(albionDataPriceModel);
                    //Debug.WriteLine($"New datapricemodel: {simplifiedItemV2.Uniquename} {albionDataPriceModel.SellPriceMin}");

                }

            }
            return simplifiedItemV2;
        }


        public static string SaveImage()
        {

            string fullPath = $@".\img\T4_OFF_SHIELD.png";

            string url = @"https://render.albiononline.com/v1/item/T4_OFF_SHIELD.png";
            Bitmap bitmap;


            if (File.Exists(fullPath))
            {
                // its already there.
                bitmap = new Bitmap(fullPath);
            }
            else
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(url);
                bitmap = new Bitmap(stream);

                if (bitmap != null)
                {
                    bitmap.Save(fullPath, ImageFormat.Png);
                }

                stream.Flush();
                stream.Close();
                client.Dispose();
            }


            return fullPath;
        }


        public static List<AlbionDataPriceModel> GetMarketPrices(string url)
        {


            //https://www.albion-online-data.com/api/v2/stats/prices/?locations=Bridgewatch&qualities=1}


            //string url_ = "https://www.albion-online-data.com/api/v2/stats/prices/T4_BAG,T5_BAG?locations=Caerleon,Bridgewatch&qualities=2";

            //Debug.WriteLine(url);


            var request = WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream webStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(webStream);
            var data = streamReader.ReadToEnd();

            List<AlbionDataPriceModel> albionDataPriceModels = JsonConvert.DeserializeObject<List<AlbionDataPriceModel>>(data);

            //Debug.WriteLine(data);



            return albionDataPriceModels;
        }










        public static BitmapImage DownloadIfNotExistsLocally(string itemName)
        {
            string fullPath = $@".\img\{itemName}.png";
            if (File.Exists(fullPath))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.UriSource = new Uri(fullPath);
                return bitmapImage;
            }
            else
            {
                string url = $"https://render.albiononline.com/v1/item/{itemName}.png";


                BitmapImage image = new BitmapImage();
                image = DownloadImage(url);




                return image;
            }


        }


        public static string DownloadIfNotExistsLocallyV2(string itemName)
        {
            string fullPath = $@"{Environment.CurrentDirectory}\img\{itemName}.png";
            if (!File.Exists(fullPath))
            {
                string url = $"https://render.albiononline.com/v1/item/{itemName}.png";
                BitmapImage bitmapImage = new BitmapImage();

                Debug.WriteLine($"Downloading {itemName}");
                using (WebClient client = new WebClient())
                {
                    //client.DownloadFile(new Uri(url), fullPath);
                    DownloadFile(url, fullPath);
                }



            }
            return fullPath;
        }


        public static int DownloadFile(String remoteFilename,
                               String localFilename)
        {
            // Function will return the number of bytes processed
            // to the caller. Initialize to 0 here.
            int bytesProcessed = 0;

            // Assign values to these objects here so that they can
            // be referenced in the finally block
            Stream remoteStream = null;
            Stream localStream = null;
            WebResponse response = null;

            // Use a try/catch/finally block as both the WebRequest and Stream
            // classes throw exceptions upon error
            try
            {
                // Create a request for the specified remote file name
                WebRequest request = WebRequest.Create(remoteFilename);
                if (request != null)
                {
                    // Send the request to the server and retrieve the
                    // WebResponse object 
                    response = request.GetResponse();
                    if (response != null)
                    {
                        // Once the WebResponse object has been retrieved,
                        // get the stream object associated with the response's data
                        remoteStream = response.GetResponseStream();

                        // Create the local file
                        localStream = File.Create(localFilename);

                        // Allocate a 1k buffer
                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        // Simple do/while loop to read from stream until
                        // no bytes are returned
                        do
                        {
                            // Read data (up to 1k) from the stream
                            bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                            // Write the data to the local file
                            localStream.Write(buffer, 0, bytesRead);

                            // Increment total bytes processed
                            bytesProcessed += bytesRead;
                        } while (bytesRead > 0);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // Close the response and streams objects here
                // to make sure they're closed even if an exception
                // is thrown at some point
                if (response != null) response.Close();
                if (remoteStream != null) remoteStream.Close();
                if (localStream != null) localStream.Close();
            }

            // Return total bytes processed to caller.
            return bytesProcessed;
        }


        public static double CalculateFocusCosts(string uniqueName, int mainTreeLevel, List<FocusCostCalculcationModel> focusCostCalculcationModels)
        {

            double totalFocusCostEfficiency = 0;
            double baseFocusCost = 0;
            double newFocusCost = 0;

            //Debug.WriteLine($"received {focusCostCalculcationModels.Count} focusCostCalculationModels ");

            // uniqueName is the item we are going to calculate it for.


            // THe list contains all the items we are also going to look at.
            // Because those specialisation counts aswell for the item in question.
            for (int i = 0; i < focusCostCalculcationModels.Count; i++)
            {


                if (focusCostCalculcationModels[i].UniqueName == uniqueName)
                {
                    //Debug.WriteLine($"Found uniqueItem{uniqueName}");


                    baseFocusCost = focusCostCalculcationModels[i].FocusCost;
                    // Add up the maintreelevel. wich gives 30 for all items.
                    totalFocusCostEfficiency += (mainTreeLevel * 30);


                    //Debug.WriteLine($"totalFocusCostEfficiency: {totalFocusCostEfficiency}");


                    // We found the item we want to calculate it for.
                    for (int j = 0; j < focusCostCalculcationModels.Count; j++)
                    {
                        if (focusCostCalculcationModels[j].UniqueName == uniqueName)
                        {
                            totalFocusCostEfficiency += (focusCostCalculcationModels[j].UserSpecInput * 250);
                            totalFocusCostEfficiency += (focusCostCalculcationModels[j].UserSpecInput * 30);


                        }
                        else
                        {
                            // Now we are going to add up all the item spec.
                            if (focusCostCalculcationModels[j].ArtefactItem)
                            {
                                totalFocusCostEfficiency += (focusCostCalculcationModels[j].UserSpecInput * 15);
                            }
                            else
                            {
                                totalFocusCostEfficiency += (focusCostCalculcationModels[j].UserSpecInput * 30);
                            }
                        }

                    }
                    //Debug.WriteLine($"totalFocusCostEfficiency: {totalFocusCostEfficiency}");

                    // Now we have the total focusCostefficiency points.

                    // Lets calculate the new focus cost value.
                    //Debug.WriteLine($"FocusCostEfficiency:{totalFocusCostEfficiency}");
                    //Debug.WriteLine($"baseCost:{baseFocusCost}");


                    newFocusCost = baseFocusCost * Math.Pow(0.5, totalFocusCostEfficiency / 10000);
                    //Debug.WriteLine($"newFocusCost{newFocusCost}");
                }
            }




            return Math.Ceiling(newFocusCost);
        }







        public static void SaveToFile(List<SimplifiedItemV2> simplifiedItems)
        {
            string FileNameOutput = @".\simplifiedItemList.json";





            File.WriteAllText(FileNameOutput, JsonConvert.SerializeObject(simplifiedItems, Formatting.Indented));

        }
        public static List<SimplifiedItemV2> OpenLocalFile()
        {
            string fileName = @".\simplifiedItemList.json";

            return JsonConvert.DeserializeObject<List<SimplifiedItemV2>>(File.ReadAllText(fileName));

        }



        public static List<SimplifiedItemV2> CreateListFromSeperateListsV2(string absoluteURL)
        {


            List<SimplifiedItemV2> simplifiedItemsV2 = new List<SimplifiedItemV2>();



            // Working demonstration of Items.JSON inside the project.
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "AlbionOnlineCraftingCalculator.Files.items.json";
            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            AlbionItemDocument myDeserializedClass = JsonConvert.DeserializeObject<AlbionItemDocument>(result);






            for (int i = 0; i < myDeserializedClass.Items.Mount.Count; i++)
            {
                var item = myDeserializedClass.Items.Mount[i];





                if (!item.Uniquename.Contains("DEBUG"))
                {
                    simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Itemvalue = item.Itemvalue });
                }




            }

            for (int i = 0; i < myDeserializedClass.Items.Simpleitem.Count; i++)
            {
                var item = myDeserializedClass.Items.Simpleitem[i];




                if (!item.Uniquename.Contains("DEBUG"))
                {
                    simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Itemvalue = item.Itemvalue, Enchantment = item.Enchantmentlevel });
                }
            }





            for (int i = 0; i < myDeserializedClass.Items.Equipmentitem.Count; i++)
            {




                var item = myDeserializedClass.Items.Equipmentitem[i];

                if (!item.Uniquename.Contains("DEBUG"))
                {

                    if (item.Tier > 3 && item.Enchantments != null)
                    {

                        if (item.Shopcategory == "gatherergear")
                        {
                            if (item.Shopsubcategory1 == "fibergatherer_helmet" || item.Shopsubcategory1 == "fibergatherer_armor" || item.Shopsubcategory1 == "fibergatherer_shoes")// fiber
                            {
                                simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "fibergatherer", Enchantment = 0 });

                                for (int j = 0; j < item.Enchantments.Enchantment.Count; j++)
                                {
                                    if (!item.Uniquename.Contains("DEBUG"))
                                    {

                                        simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename + $"@{(j + 1).ToString()}", Tier = item.Tier, Craftingrequirements = item.Enchantments.Enchantment[j].Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "fibergatherer", Enchantment = j + 1 });




                                    }
                                }
                            }
                            if (item.Shopsubcategory1 == "hidegatherer_helmet" || item.Shopsubcategory1 == "hidegatherer_armor" || item.Shopsubcategory1 == "hidegatherer_shoes") // hide                        {
                            {

                                simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "hidegatherer", Enchantment = 0 });

                                for (int j = 0; j < item.Enchantments.Enchantment.Count; j++)
                                {
                                    if (!item.Uniquename.Contains("DEBUG"))
                                    {

                                        simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename + $"@{(j + 1).ToString()}", Tier = item.Tier, Craftingrequirements = item.Enchantments.Enchantment[j].Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "hidegatherer", Enchantment = j + 1 });




                                    }
                                }

                            }
                            if (item.Shopsubcategory1 == "rockgatherer_helmet" || item.Shopsubcategory1 == "rockgatherer_armor" || item.Shopsubcategory1 == "rockgatherer_shoes")// rock
                            {

                                simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "rockgatherer", Enchantment = 0 });

                                for (int j = 0; j < item.Enchantments.Enchantment.Count; j++)
                                {
                                    if (!item.Uniquename.Contains("DEBUG"))
                                    {

                                        simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename + $"@{(j + 1).ToString()}", Tier = item.Tier, Craftingrequirements = item.Enchantments.Enchantment[j].Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "rockgatherer", Enchantment = j + 1 });




                                    }
                                }

                            }
                            if (item.Shopsubcategory1 == "woodgatherer_helmet" || item.Shopsubcategory1 == "woodgatherer_armor" || item.Shopsubcategory1 == "woodgatherer_shoes")// wood
                            {

                                simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "woodgatherer", Enchantment = 0 });

                                for (int j = 0; j < item.Enchantments.Enchantment.Count; j++)
                                {
                                    if (!item.Uniquename.Contains("DEBUG"))
                                    {

                                        simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename + $"@{(j + 1).ToString()}", Tier = item.Tier, Craftingrequirements = item.Enchantments.Enchantment[j].Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "woodgatherer", Enchantment = j + 1 });




                                    }
                                }

                            }
                            else if (item.Shopsubcategory1 == "oregatherer_helmet" || item.Shopsubcategory1 == "oregatherer_armor" || item.Shopsubcategory1 == "oregatherer_shoes")// ore
                            {

                                simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "oregatherer", Enchantment = 0 });

                                for (int j = 0; j < item.Enchantments.Enchantment.Count; j++)
                                {
                                    if (!item.Uniquename.Contains("DEBUG"))
                                    {

                                        simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename + $"@{(j + 1).ToString()}", Tier = item.Tier, Craftingrequirements = item.Enchantments.Enchantment[j].Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "oregatherer", Enchantment = j + 1 });




                                    }
                                }

                            }
                            else if (item.Shopsubcategory1 == "fishgatherer_helmet" || item.Shopsubcategory1 == "fishgatherer_armor" || item.Shopsubcategory1 == "fishgatherer_shoes")// fish
                            {

                                simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "fishgatherer", Enchantment = 0 });

                                for (int j = 0; j < item.Enchantments.Enchantment.Count; j++)
                                {
                                    if (!item.Uniquename.Contains("DEBUG"))
                                    {

                                        simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename + $"@{(j + 1).ToString()}", Tier = item.Tier, Craftingrequirements = item.Enchantments.Enchantment[j].Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = "fishgatherer", Enchantment = j + 1 });




                                    }
                                }

                            }
                        }
                        else
                        {
                            simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Enchantment = 0 });

                            for (int j = 0; j < item.Enchantments.Enchantment.Count; j++)
                            {
                                if (!item.Uniquename.Contains("DEBUG"))
                                {

                                    simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename + $"@{(j + 1).ToString()}", Tier = item.Tier, Craftingrequirements = item.Enchantments.Enchantment[j].Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Enchantment = j + 1 });




                                }
                            }
                        }







                    }
                }

            }


            for (int i = 0; i < myDeserializedClass.Items.Consumableitem.Count; i++)
            {
                var item = myDeserializedClass.Items.Consumableitem[i];

                if (item.Tier > 1)
                {
                    simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Enchantment = 0 });


                    if (item.Enchantments != null)
                    {
                        for (int j = 0; j < item.Enchantments.Enchantment.Count; j++)
                        {
                            if (!item.Uniquename.Contains("DEBUG"))
                            {

                                simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename + $"@{(j + 1).ToString()}", Tier = item.Tier, Craftingrequirements = item.Enchantments.Enchantment[j].Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Enchantment = j + 1 });




                            }
                        }
                    }
                    else
                    {

                    }

                }
            }

            for (int i = 0; i < myDeserializedClass.Items.Weapon.Count; i++)
            {
                var item = myDeserializedClass.Items.Weapon[i];
                if (item.Tier > 3 && item.Enchantments != null)
                {
                    simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Enchantment = 0 });

                    for (int j = 0; j < item.Enchantments.Enchantment.Count; j++)
                    {
                        if (!item.Uniquename.Contains("DEBUG"))
                        {


                            //if (item.Uniquename.Contains("BOW")) { Debug.WriteLine($"{item.Uniquename} requires {item.Enchantments.Enchantment[j].Craftingrequirements[0].Craftresources.Count} resources to craft"); }

                            simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename + $"@{(j + 1).ToString()}", Tier = item.Tier, Craftingrequirements = item.Enchantments.Enchantment[j].Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Enchantment = j + 1 });




                        }
                    }
                }
            }


            for (int i = 0; i < myDeserializedClass.Items.Furnitureitem.Count; i++)
            {
                var item = myDeserializedClass.Items.Furnitureitem[i];

                if (!item.Uniquename.Contains("DEBUG"))
                {



                    simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Itemvalue = item.Itemvalue });
                }
            }

            for (int i = 0; i < myDeserializedClass.Items.Farmableitem.Count; i++)
            {
                var item = myDeserializedClass.Items.Farmableitem[i];
                if (!item.Uniquename.Contains("DEBUG"))
                {

                    simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Itemvalue = item.Itemvalue });
                }
            }

            for (int i = 0; i < myDeserializedClass.Items.Consumablefrominventoryitem.Count; i++)
            {
                var item = myDeserializedClass.Items.Consumablefrominventoryitem[i];

                if (!item.Uniquename.Contains("DEBUG"))
                {
                    simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename, Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Itemvalue = item.Itemvalue });

                }
            }


            for (int i = 0; i < myDeserializedClass.Items.Journalitem.Count; i++)
            {
                var item = myDeserializedClass.Items.Journalitem[i];
                if (!item.Uniquename.Contains("DEBUG"))
                {
                    simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename + "_FULL", Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Famefillingmissions = item.Famefillingmissions });
                    simplifiedItemsV2.Add(new SimplifiedItemV2() { Uniquename = item.Uniquename + "_EMPTY", Tier = item.Tier, Craftingrequirements = item.Craftingrequirements, Shopcategory = item.Shopcategory, Shopsubcategory = item.Shopsubcategory1, Famefillingmissions = item.Famefillingmissions });

                }

            }

            for (int i = 0; i < simplifiedItemsV2.Count; i++)
            {
                if (simplifiedItemsV2[i].Uniquename.Contains("IRONGAUNTLETS"))
                {
                    simplifiedItemsV2.RemoveAt(i);
                }
            }




            return simplifiedItemsV2;

        }


    }


    class FileDownloader
    {
        private readonly string _url;
        private readonly string _fullPathWhereToSave;
        private bool _result = false;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        public FileDownloader(string url, string fullPathWhereToSave)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(fullPathWhereToSave)) throw new ArgumentNullException("fullPathWhereToSave");

            this._url = url;
            this._fullPathWhereToSave = fullPathWhereToSave;
        }

        public bool StartDownload(int timeout)
        {
            try
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(_fullPathWhereToSave));

                if (File.Exists(_fullPathWhereToSave))
                {
                    //File.Delete(_fullPathWhereToSave);
                    return true;
                }
                else
                {
                    using (WebClient client = new WebClient())
                    {
                        var ur = new Uri(_url);
                        // client.Credentials = new NetworkCredential("username", "password");
                        client.DownloadProgressChanged += WebClientDownloadProgressChanged;
                        client.DownloadFileCompleted += WebClientDownloadCompleted;
                        Console.WriteLine(@"Downloading file:");
                        client.DownloadFileAsync(ur, _fullPathWhereToSave);
                        _semaphore.Wait(timeout);
                        return _result && File.Exists(_fullPathWhereToSave);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Was not able to download file!");
                Console.Write(e);
                return false;
            }
            finally
            {
                this._semaphore.Dispose();
            }
        }

        private void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Write("\r     -->    {0}%.", e.ProgressPercentage);
        }

        private void WebClientDownloadCompleted(object sender, AsyncCompletedEventArgs args)
        {
            _result = !args.Cancelled;
            if (!_result)
            {
                Console.Write(args.Error.ToString());
            }
            Console.WriteLine(Environment.NewLine + "Download finished!");
            _semaphore.Release();
        }

        public static bool DownloadFile(string url, string fullPathWhereToSave, int timeoutInMilliSec)
        {
            return new FileDownloader(url, fullPathWhereToSave).StartDownload(timeoutInMilliSec);
        }
    }



}
