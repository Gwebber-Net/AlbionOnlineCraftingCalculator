using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlbionOnlineCraftingCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public bool debug = false;


        List<Shopcategory> shopcategories = new List<Shopcategory>();


        List<SimplifiedItemV2> SimplifiedItemsV2 = new List<SimplifiedItemV2>();


        List<Journalitem> journalitems = new List<Journalitem>();


        Setting Settings = new Setting();



        List<string> shopcategoriesToBeIgnored = new List<string>() {"accessories", "mounts", "skillbooks", "resources", "token", "materials", "artefacts", "cityresources", "labourers", "furniture", "other", "products", "luxurygoods", "trophies", "farmables" };
        List<string> shopsubcategoriesToBeIgnored = new List<string>() { "vanity", "maps", "other", "fish", "fishingbait" };



        public string OldCategory = "", NewCategory = "";
        public string OldSubCategory = "", NewSubCategory = "";



        public MainWindow()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("nl-NL");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("nl-NL");


            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            SimplifiedItemsV2 = Methods.OpenSimplifiedItems();
            //Methods.SaveToFile(SimplifiedItemsV2);
            shopcategories = Methods.ConvertItemsToCategories();

            journalitems = Methods.ConvertItemsJournalitems();

            Settings = Methods.OpenSettings(shopcategories, shopcategoriesToBeIgnored, shopsubcategoriesToBeIgnored);



            PrepareUI();
        }


        private void PrepareUI()
        {
            cmb_tier.ItemsSource = new List<string> { "4", "5", "6", "7", "8" };
            cmb_enchantment.ItemsSource = new List<string> { "0", "1", "2", "3" };
            cmb_location.ItemsSource = new List<string> { "Bridgewatch", "Martlock", "Thetford", "Lymhurst", "Fort Sterling", "Caerleon" };
            cmb_returnrate.ItemsSource = new List<string> { "43,5", "47,9" };

            // Populate combobox categories.
            List<String> list = new List<string>();
            for (int i = 0; i < shopcategories.Count; i++)
            {
                if (!shopcategoriesToBeIgnored.Contains(shopcategories[i].Id))
                {
                    list.Add(shopcategories[i].Id.ToString());
                }
            }
            string[] vs = list.ToArray();





            cmb_cat.ItemsSource = vs;


            cmb_cat.SelectedIndex = 0;
            cmb_subcat.SelectedIndex = 0;
            cmb_tier.SelectedIndex = 0;
            cmb_enchantment.SelectedIndex = 0;
            cmb_location.SelectedIndex = 1;
            cmb_returnrate.SelectedIndex = 0;
        }

        private void cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            switch (comboBox.Name)
            {
                case "cmb_cat":
                    {
                        string item = comboBox.SelectedItem.ToString();


                        for (int i = 0; i < shopcategories.Count; i++)
                        {
                            if (item == shopcategories[i].Id)
                            {
                                string[] vs;
                                List<string> list = new List<string>();



                                for (int j = 0; j < shopcategories[i].Shopsubcategory.Count; j++)
                                {
                                    if (!shopsubcategoriesToBeIgnored.Contains(shopcategories[i].Shopsubcategory[j].id))
                                    {
                                        list.Add(shopcategories[i].Shopsubcategory[j].id);
                                    }

                                }

                                vs = list.ToArray();


                                cmb_subcat.ItemsSource = vs;

                            }
                        }
                        break;
                    }
                case "cmb_subcat":
                    {
                        break;
                    }

                case "cmb_tier":
                    {

                        break;
                    }
                case "cmb_enchantment":
                    {
                        break;
                    }
            }
            UpdateRecipe(true);
        }


        private void UpdateRecipe(bool priceUpdate)
        {
            if (cmb_cat.SelectedItem != null &&
                cmb_subcat.SelectedItem != null &&
                cmb_tier.SelectedItem != null &&
                cmb_enchantment.SelectedItem != null &&
                cmb_location.SelectedItem != null &&
                cmb_returnrate.SelectedItem != null &&
                Tb_Nutrition_Fee.Text != "" //&&
                                            //Tb_Spec_MainTree.Text != ""
                )

            {
                // All comboboxes have a value.
                string category = cmb_cat.SelectedItem.ToString();
                string subcategory = cmb_subcat.SelectedItem.ToString();
                int tier = int.Parse(cmb_tier.SelectedItem.ToString());
                int enchantment = int.Parse(cmb_enchantment.SelectedItem.ToString());
                string location = cmb_location.SelectedItem.ToString();



                // Make these "TryParse Methods"
                double returnrate = double.Parse(cmb_returnrate.SelectedItem.ToString());
                int nutritionFee = int.Parse(Tb_Nutrition_Fee.Text);
                int mainTreeSpec = int.Parse(Tb_Spec_MainTree.Text);


                InputParameters inputParameters = new InputParameters { Category = category, SubCategory = subcategory, Tier = tier, Enchantment = enchantment, Location = location, ReturnRate = returnrate, NutritionFee = nutritionFee, MainTreeSpec = mainTreeSpec };


                List<SimplifiedItemV2> simplifiedItemsV2 = new List<SimplifiedItemV2>();

                Journalitem journalitem = null;

                for (int k = 0; k < SimplifiedItemsV2.Count; k++)
                {
                    if (SimplifiedItemsV2[k].Shopcategory == category && SimplifiedItemsV2[k].Shopsubcategory == subcategory && SimplifiedItemsV2[k].Tier == tier && SimplifiedItemsV2[k].Enchantment == enchantment)
                    {
                        simplifiedItemsV2.Add(SimplifiedItemsV2[k]);
                    }
                }


                



                if (priceUpdate)
                {
                    List<string> allItemNamesForPrices = new List<string>();


                    if (!(category == "resources") && !(category == "consumables"))
                    {
                        string n = simplifiedItemsV2[0].Uniquename;
                        if (enchantment != 0)
                        {
                            n = n.Replace($"@{enchantment}", "");
                        }
                        journalitem = Methods.FindJournalForEndproduct(journalitems, n);
                        allItemNamesForPrices = Methods.CreateItemListForAlbionDataApiV2(allItemNamesForPrices, Methods.FindSimplifiedItem(journalitem.Uniquename, SimplifiedItemsV2).Uniquename);
                        allItemNamesForPrices = Methods.CreateItemListForAlbionDataApiV2(allItemNamesForPrices, Methods.FindSimplifiedItem(journalitem.EmptyName, SimplifiedItemsV2).Uniquename);
                    }


                    for (int i = 0; i < simplifiedItemsV2.Count; i++)
                    {
                        string name = simplifiedItemsV2[i].Uniquename;
                        if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL1")) { name = name + "@1"; }
                        if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL2")) { name = name + "@2"; }
                        if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL3")) { name = name + "@3"; }

                        allItemNamesForPrices = Methods.CreateItemListForAlbionDataApiV2(allItemNamesForPrices, name);

                        for (int j = 0; j < simplifiedItemsV2[i].Craftingrequirements[0].Craftresources.Count; j++)
                        {

                            name = simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename;

                            if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL1")) { name = name + "@1"; }
                            if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL2")) { name = name + "@2"; }
                            if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL3")) { name = name + "@3"; }


                            allItemNamesForPrices = Methods.CreateItemListForAlbionDataApiV2(allItemNamesForPrices, name);
                        }
                    }

                    

                    List<AlbionDataPriceModel> allItemPricesForCurrentRequest = Methods.GetMarketPrices(Methods.CreateAlbionDataUrl(new AlbionDataWebRequestModel() { Items = allItemNamesForPrices, Locations = location, Qualities = "1" }));
                    SimplifiedItemsV2 = Methods.UpdatePrices(allItemPricesForCurrentRequest, SimplifiedItemsV2);

                    if (debug)
                    {
                        Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                        Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                        Debug.WriteLine("''''''''''''Íncoming Prices from AlbionDataApi''''''''''''''''");
                        Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                        Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                        for (int i = 0; i < allItemPricesForCurrentRequest.Count; i++)
                        {
                            Debug.WriteLine($"{allItemPricesForCurrentRequest[i].ItemId} : {allItemPricesForCurrentRequest[i].SellPriceMin}");
                        }


                        Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                        Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                        Debug.WriteLine("''''''''''''Íncoming Prices from AlbionDataApi''''''''''''''''");
                        Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                        Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    }
                }

                Methods.SaveToFile(SimplifiedItemsV2);
                UIModelV2 uIModelV2 = new UIModelV2();
                uIModelV2.InputParameters = inputParameters;
                uIModelV2.simplifiedItems = simplifiedItemsV2;
                uIModelV2.Journal = journalitem;
                // uIModelV2.Specialisation = specialisationList;

                UpdateUIV2(uIModelV2, priceUpdate);
            }
        }









        private void UpdateUIV2(UIModelV2 uIModel, bool priceUpdate)
        {
            Task.Run(() =>
            {
                List<FocusCostCalculcationModel> focusCostCalculcationModels = new List<FocusCostCalculcationModel>();



                for (int i = 0; i < uIModel.simplifiedItems.Count; i++)
                {

                    AlbionCraftingInformation albionCraftingInformation = new AlbionCraftingInformation();

                    // So here we are going to build the list for the focus cost calculation..
                    bool artefactItem = false;
                    if (uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources.Count > 1)
                    {
                        if (uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[1].Uniquename.Contains("ARTEFACT") ||
                        uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[1].Uniquename.Contains("ROYAL"))
                        {
                            artefactItem = true;
                        }


                        if (uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources.Count > 2)
                        {
                            if (uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[2].Uniquename.Contains("ARTEFACT") ||
                    uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[2].Uniquename.Contains("ROYAL"))
                            {
                                artefactItem = true;
                            }
                        }
                    }





                    /////////////
                    ////////////
                    ////
                    //// Putting together the information for the focus-use calculation.
                    ////
                    ////
                    NewCategory = uIModel.InputParameters.Category;
                    NewSubCategory = uIModel.InputParameters.SubCategory;



                    if (OldCategory != NewCategory || OldSubCategory != NewSubCategory)
                    {
                        // The 2 main parameters have changed.
                        // Meaning we have to reset all the spec textboxes.
                        this.Dispatcher.Invoke(() =>
                        {
                            UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Text = "0";
                            Tb_Spec_MainTree.Text = "0";
                        });
                    }


                    int specialisationValue = 0;
                    int specialisationFromUserInput = 0;
                    this.Dispatcher.Invoke(() =>
                    {
                        int.TryParse(UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Text, out specialisationFromUserInput);
                    });

                    string specName = $"SPEC_{uIModel.InputParameters.Category}_{uIModel.InputParameters.SubCategory}";
                    //Debug.WriteLine("0__" + specName);
                    UserSpecInput userSpec = Methods.FindSetting(specName, Settings.UserSpecInput);
                    int specialisationFromSettingsFile = userSpec.Spec[i];


                    if (specialisationFromUserInput != 0)
                    {
                        // We know the user has put in somthing
                        specialisationValue = specialisationFromUserInput;
                        userSpec.Spec[i] = specialisationValue;
                        Settings = Methods.UpdateSpecialisations(Settings, specName, i, specialisationValue);
                    }
                    else
                    {
                        // We know the user does not have put in something.
                        // Lets see if we have a setting from a previous session.
                        if (specialisationFromSettingsFile != 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                // Did the user put in a value other then 0 ?
                                specialisationValue = specialisationFromSettingsFile;
                                UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Text = specialisationValue.ToString();
                            });
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                specialisationValue = 0;
                                UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Text = 0.ToString();
                            });
                        }

                    }







                    if (!uIModel.simplifiedItems[i].Uniquename.Contains("ROYAL"))
                    {
                        FocusCostCalculcationModel focusCostCalculcationModel = new FocusCostCalculcationModel()
                        {
                            UniqueName = uIModel.simplifiedItems[i].Uniquename,
                            FocusCost = int.Parse(uIModel.simplifiedItems[i].Craftingrequirements[0].Craftingfocus),
                            UserSpecInput = specialisationValue,
                            ArtefactItem = artefactItem

                        };
                        focusCostCalculcationModels.Add(focusCostCalculcationModel);
                    }

                    if (priceUpdate)
                    {
                        double percentage = uIModel.InputParameters.ReturnRate / 100;
                        albionCraftingInformation = Methods.CalculateProfitV2(uIModel.simplifiedItems[i], uIModel.InputParameters.NutritionFee, 0.045, uIModel.InputParameters.Location, SimplifiedItemsV2, uIModel.Journal, percentage);

                        if (debug)
                        {
                            Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                            Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                            Debug.WriteLine("'''''''''''''''''Calculated profit results''''''''''''''''''''");
                            Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                            Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                            for (int j = 0; j < albionCraftingInformation.Resources.ResourceReturnModels.Count; j++)
                            {
                                Debug.WriteLine($"Received back:{albionCraftingInformation.Resources.ResourceReturnModels[j].UniqueName} * {albionCraftingInformation.Resources.ResourceReturnModels[j].Count}");
                            }
                            Debug.WriteLine($"Journal value:{albionCraftingInformation.Journal.JournalFilledPercentageValue}");
                            Debug.WriteLine($"Material Costs & Art:{albionCraftingInformation.Resources.ResourceSilverInvested}");
                            Debug.WriteLine($"Resource Cost after return:{albionCraftingInformation.Resources.ResourceSilverCost}");
                            Debug.WriteLine($"Nutrition Cost{albionCraftingInformation.NutritionCost}");
                            Debug.WriteLine($"Sellorder Cost{albionCraftingInformation.SellorderCost}");
                            Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                            //Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                            // Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                        }

                        if (uIModel.Journal != null)
                        {

                            if (uIModel.simplifiedItems[i].Uniquename.Contains("ROYAL"))
                            {

                                this.Dispatcher.Invoke(() =>
                                {

                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_Full_Price").Text = "";
                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_Empty_Price").Text = "";

                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_FilledPercentage").Text = "";


                                    UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Journal").Source = null;
                                    UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Visibility = Visibility.Hidden;
                                });
                            }
                            else
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    //Debug.WriteLine(uIModel.Journal.Uniquename);
                                    int journalPrice = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.Journal.Uniquename, uIModel.InputParameters.Location);
                                    int journal_empty_price = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.Journal.EmptyName, uIModel.InputParameters.Location);
                                    double percentageFilled = Math.Round(albionCraftingInformation.Journal.JournalFilledPercentage, 3) * 100;
                                    double filledPercentageValue = Math.Round(albionCraftingInformation.Journal.JournalFilledPercentageValue, 3);


                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_Full_Price").Text = "Full = " + journalPrice.ToString();
                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_Empty_Price").Text = "Empty = " + journal_empty_price.ToString();

                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_FilledPercentage").Text = percentageFilled.ToString() + "% = " + Math.Round(filledPercentageValue, 0);


                                    BitmapImage bitmapImage = new BitmapImage(new Uri(Methods.DownloadIfNotExistsLocallyV2(uIModel.Journal.Uniquename), UriKind.Absolute));
                                    UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Journal").Source = bitmapImage;
                                    UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Visibility = Visibility.Visible;
                                    UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_Nutrition_Tax").Visibility = Visibility.Visible;

                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_NutritionCost").Text = albionCraftingInformation.NutritionCost.ToString();
                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_SellOrderTax").Text = albionCraftingInformation.SellorderCost.ToString();




                                });
                            }
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_Full_Price").Text = "";
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_Empty_Price").Text = "";

                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_FilledPercentage").Text = "";
                                UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Journal").Source = null;
                            });
                        }

                        this.Dispatcher.Invoke(() =>
                        {
                            int investment = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[0].Count;
                            int returned = albionCraftingInformation.Resources.ResourceReturnModels[0].Count;
                            int cost = investment - returned;
                            int endProductPrice = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Uniquename, uIModel.InputParameters.Location);
                            int ing1Price = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[0].Uniquename, uIModel.InputParameters.Location);

                            string endProductName = uIModel.simplifiedItems[i].Uniquename;
                            string ing1Name = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[0].Uniquename;

                            if (endProductName.Contains("ROYAL"))
                            {
                                UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_Focus").Visibility = Visibility.Hidden;

                            }
                            else
                            {
                                UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_Focus").Visibility = Visibility.Visible;

                            }
                            UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Name").Text = endProductName;
                            UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Price").Text = endProductPrice.ToString();// uIModel.simplifiedItems[i].AlbionDataPriceModels.SellPriceMin.ToString();
                            UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing1").Text = ing1Name + " * (" + investment.ToString() + "-" + returned.ToString() + ") = " + cost.ToString();
                            UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing1_Price").Text = ing1Price.ToString() + " * " + investment.ToString() + "-" + returned.ToString() + " = " + (cost * ing1Price); ;

                            BitmapImage bitmapImage = new BitmapImage(new Uri(Methods.DownloadIfNotExistsLocallyV2(endProductName), UriKind.Absolute));
                            UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}").Source = bitmapImage;
                            bitmapImage = new BitmapImage(new Uri(Methods.DownloadIfNotExistsLocallyV2(ing1Name), UriKind.Absolute));
                            UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Visibility = Visibility.Visible;
                            UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Ing1").Source = bitmapImage;
                        });


                        if (uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources.Count > 1)
                        {
                            if (uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[1].Uniquename.Contains("ARTEFACT"))
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    int investment = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[1].Count;
                                    int returned = albionCraftingInformation.Resources.ResourceReturnModels[1].Count;
                                    int cost = investment - returned;
                                    int endProductPrice = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Uniquename, uIModel.InputParameters.Location);
                                    int ing2Price = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[1].Uniquename, uIModel.InputParameters.Location);

                                    string endProductName = uIModel.simplifiedItems[i].Uniquename;
                                    string ing2Name = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[1].Uniquename;

                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing2").Text = "Artefact" + " * (" + investment.ToString() + "-" + returned.ToString() + ") = " + cost;
                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing2_Price").Text = ing2Price.ToString() + " * " + investment.ToString() + "-" + returned.ToString() + " = " + (cost * ing2Price); ;
                                    BitmapImage bitmapImage = new BitmapImage(new Uri(Methods.DownloadIfNotExistsLocallyV2(ing2Name), UriKind.Absolute));
                                    UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Ing2").Source = bitmapImage;
                                    UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Visibility = Visibility.Visible;
                                });
                            }
                            else
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    int investment = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[1].Count;
                                    int returned = albionCraftingInformation.Resources.ResourceReturnModels[1].Count;
                                    int cost = investment - returned;
                                    int endProductPrice = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Uniquename, uIModel.InputParameters.Location);
                                    int ing2Price = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[1].Uniquename, uIModel.InputParameters.Location);

                                    string endProductName = uIModel.simplifiedItems[i].Uniquename;
                                    string ing2Name = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[1].Uniquename;

                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing2").Text = ing2Name + " * (" + investment.ToString() + "-" + returned.ToString() + ") = " + cost;
                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing2_Price").Text = ing2Price.ToString() + " * " + investment.ToString() + "-" + returned.ToString() + " = " + (cost * ing2Price); ;

                                    BitmapImage bitmapImage = new BitmapImage(new Uri(Methods.DownloadIfNotExistsLocallyV2(ing2Name), UriKind.Absolute));
                                    UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Ing2").Source = bitmapImage;
                                    UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Visibility = Visibility.Visible;

                                });
                            }


                            if (uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources.Count > 2)
                            {
                                if (uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[2].Uniquename.Contains("ARTEFACT"))
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        int investment = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[2].Count;
                                        int returned = albionCraftingInformation.Resources.ResourceReturnModels[2].Count;
                                        int cost = investment - returned;
                                        int endProductPrice = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Uniquename, uIModel.InputParameters.Location);
                                        int ing3Price = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[2].Uniquename, uIModel.InputParameters.Location);

                                        string endProductName = uIModel.simplifiedItems[i].Uniquename;
                                        string ing3Name = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[2].Uniquename;

                                        UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing3").Text = "Artefact" + " * (" + investment.ToString() + "-" + returned.ToString() + ") = " + cost.ToString();
                                        UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing3_Price").Text = ing3Price.ToString() + " * " + investment.ToString() + "-" + returned.ToString() + " = " + (cost * ing3Price); ;

                                        string fileLocation = Methods.DownloadIfNotExistsLocallyV2(ing3Name);
                                        BitmapImage bitmapImage = new BitmapImage(new Uri(fileLocation, UriKind.Absolute));
                                        UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Ing3").Source = bitmapImage;
                                        UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_4").Background = new SolidColorBrush(Colors.Transparent);
                                        UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Visibility = Visibility.Visible;

                                    });
                                }
                                else
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        int investment = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[2].Count;
                                        int returned = albionCraftingInformation.Resources.ResourceReturnModels[2].Count;
                                        int cost = investment - returned;
                                        int endProductPrice = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Uniquename, uIModel.InputParameters.Location);
                                        int ing3Price = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[2].Uniquename, uIModel.InputParameters.Location);

                                        string endProductName = uIModel.simplifiedItems[i].Uniquename;
                                        string ing3Name = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[2].Uniquename;

                                        UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing3").Text = ing3Name + " * (" + investment.ToString() + "-" + returned.ToString() + ") = " + cost;
                                        UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing3_Price").Text = ing3Price.ToString() + " * " + investment.ToString() + "-" + returned.ToString() + " = " + (cost * ing3Price);
                                        BitmapImage bitmapImage = new BitmapImage(new Uri(Methods.DownloadIfNotExistsLocallyV2(ing3Name), UriKind.Absolute));
                                        UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Ing3").Source = bitmapImage;
                                        UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_4").Background = new SolidColorBrush(Colors.Transparent);
                                        UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Visibility = Visibility.Visible;

                                    });
                                }
                            }
                            else
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing3").Text = "";
                                    UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing3_Price").Text = "";
                                    UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Ing3").Source = null;
                                    UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_3").Background = new SolidColorBrush(Colors.Transparent);
                                    UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_4").Background = new SolidColorBrush(Colors.Transparent);
                                });
                            }
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing2").Text = "";
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing2_Price").Text = "";
                                UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Ing2").Source = null;
                                UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_2").Background = new SolidColorBrush(Colors.Transparent);
                                UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_3").Background = new SolidColorBrush(Colors.Transparent);
                                UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_4").Background = new SolidColorBrush(Colors.Transparent);
                            });
                        }
                    }
                }

                OldCategory = NewCategory;
                OldSubCategory = NewSubCategory;

                if (priceUpdate)
                {
                    if (uIModel.simplifiedItems.Count < 8)
                    {
                        for (int i = uIModel.simplifiedItems.Count; i < 8; i++)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Name").Text = "";
                                UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}").Source = null;
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Price").Text = "";
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing1").Text = "";
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing1_Price").Text = "";
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing2").Text = "";
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing2_Price").Text = "";
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing3").Text = "";
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Ing3_Price").Text = "";
                                UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_1").Background = new SolidColorBrush(Colors.Transparent);
                                UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_2").Background = new SolidColorBrush(Colors.Transparent);
                                UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_3").Background = new SolidColorBrush(Colors.Transparent);
                                UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_4").Background = new SolidColorBrush(Colors.Transparent);
                                UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}").Source = null;
                                UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Ing1").Source = null;
                                UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Ing2").Source = null;
                                UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Ing3").Source = null;
                                UIElementExtensions.FindControl<TextBox>(this, $"Tb_Item{(i + 1).ToString()}_CraftingSpec").Visibility = Visibility.Hidden;
                                UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_Focus").Visibility = Visibility.Hidden;
                                UIElementExtensions.FindControl<Grid>(this, $"Grid_Item{(i + 1).ToString()}_Nutrition_Tax").Visibility = Visibility.Hidden;
                                UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Journal").Source = null;
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_Full_Price").Text = "";
                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_Empty_Price").Text = "";

                                UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(i + 1).ToString()}_Journal_FilledPercentage").Text = "";
                                UIElementExtensions.FindControl<System.Windows.Controls.Image>(this, $"Img_Item{(i + 1).ToString()}_Journal").Source = null;
                            });
                        }
                    }
                }

                if (priceUpdate)
                {
                    int mainTreeValue = 0;
                    int mainTreeFromUserInput = 0;
                    this.Dispatcher.Invoke(() =>
                    {

                        mainTreeFromUserInput = int.Parse(Tb_Spec_MainTree.Text); // Pull from textbox, instead of inputparamters, since input parameters is faulty, due to user sending in new request with old maintreeSpec.
                    });

                    string specName = $"SPEC_{uIModel.InputParameters.Category}_{uIModel.InputParameters.SubCategory}";
                    Debug.WriteLine("1__" + specName);
                    //string specName = $"SPEC_{uIModel.InputParameters.Category}_{uIModel.InputParameters.SubCategory}";
                    UserSpecInput userSpec = Methods.FindSetting(specName, Settings.UserSpecInput);
                    int mainTreeFromSettingsFile = userSpec.MainTree;

                    if (mainTreeFromUserInput != 0)
                    {
                        mainTreeValue = mainTreeFromUserInput;
                        userSpec.MainTree = mainTreeValue;
                        Settings = Methods.UpdateMainTree(Settings, specName, mainTreeValue);

                    }
                    else
                    {
                        if (mainTreeFromSettingsFile != 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                mainTreeValue = mainTreeFromSettingsFile;
                                this.Dispatcher.Invoke(() =>
                                {
                                    UIElementExtensions.FindControl<TextBox>(this, $"Tb_Spec_MainTree").Text = mainTreeValue.ToString();

                                });
                            });
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                UIElementExtensions.FindControl<TextBox>(this, $"Tb_Spec_MainTree").Text = 0.ToString();
                                mainTreeValue = 0;
                            });
                        }
                    }





                    for (int k = 0; k < uIModel.simplifiedItems.Count; k++)
                    {
                        double focusCost = Methods.CalculateFocusCosts(uIModel.simplifiedItems[k].Uniquename, mainTreeValue, focusCostCalculcationModels);
                        this.Dispatcher.Invoke(() =>
                        {
                            UIElementExtensions.FindControl<TextBlock>(this, $"Tb_Item{(k + 1).ToString()}_FocusCost").Text = focusCost.ToString();
                        });
                    }
                }

            });
        }



        private T FindChild<T>(DependencyObject parent, string childName)
  where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null)
            {
                //Debug.WriteLine("Returning NULL");
                return null;
            }

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);
                    var frameworkElement = child as FrameworkElement;

                    //Debug.WriteLine(frameworkElement.Name);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    //Debug.WriteLine(frameworkElement.Name);

                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        // Debug.WriteLine("Found Match");
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;



                    //foundChild = FindChild<T>(child, childName);
                    var frameworkElement = child as FrameworkElement;
                    //Debug.WriteLine(frameworkElement.Name);

                    break;
                }
            }

            return foundChild;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Methods.SaveToFile(SimplifiedItemsV2);


            Methods.SaveSettings(Settings);
        }

        private void Tb_Item_CraftingSpec_TextChanged(object sender, TextChangedEventArgs e)
        {
            // This Function will get triggered when the text is changed.
            // ComboBox comboBox = (ComboBox)sender;
            TextBox textBox = (TextBox)sender;


            if (textBox.Text != "")
            {
                //UpdateRecipe(false);

            }

            // Update the UI, but dont get new prices.
            // check if the input the user gave was valid.



        }

        private void Tb_Item_CraftingSpec_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);



            // The logic seems a bit off,    its because the e.handled wants a true, if its not right.
            // and false means its good...
            // So it works, but its weird.
            double val;
            bool parsed = !double.TryParse(fullText, out val);
            if (!parsed)
            {
                if (val > 100)
                {
                    e.Handled = true;

                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Tb_Nutrition_Fee_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);



            // The logic seems a bit off,    its because the e.handled wants a true, if its not right.
            // and false means its good...
            // So it works, but its weird.
            double val;
            bool parsed = !double.TryParse(fullText, out val);
            if (!parsed)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);



            // The logic seems a bit off,    its because the e.handled wants a true, if its not right.
            // and false means its good...
            // So it works, but its weird.
            double val;
            bool parsed = !double.TryParse(fullText, out val);
            if (!parsed)
            {
                if (val > 100)
                {
                    e.Handled = true;

                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Btn_PullDataAndCalculate_Click(object sender, RoutedEventArgs e)
        {
            UpdateRecipe(true);
        }
    }

    public static class UIElementExtensions
    {
        public static T FindControl<T>(this Visual parent, string ControlName) where T : FrameworkElement
        {
            if (parent == null)
                return null;

            if (parent.GetType() == typeof(T) && ((T)parent).Name == ControlName)
            {
                return (T)parent;
            }
            T result = null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                Visual child = (Visual)VisualTreeHelper.GetChild(parent, i);

                if (FindControl<T>(child, ControlName) != null)
                {
                    result = FindControl<T>(child, ControlName);
                    break;
                }
            }
            return result;
        }
    }


}
