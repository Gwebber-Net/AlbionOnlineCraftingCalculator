using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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
        public bool ShowGridLines = false;


        private LicenseManager LM;

        public bool debug = false;



        public ObservableCollection<ItemV2> ItemsAttachedToUIV2 = new ObservableCollection<ItemV2>();

        List<Shopcategory> shopcategories = new List<Shopcategory>();


        List<SimplifiedItemV2> SimplifiedItemsV2 = new List<SimplifiedItemV2>();


        List<Journalitem> journalitems = new List<Journalitem>();


        Setting Settings = new Setting();



        List<string> shopcategoriesToBeIgnored = new List<string>() { "accessories", "mounts", "skillbooks", "token", "materials", "artefacts", "cityresources", "labourers", "furniture", "other", "products", "luxurygoods", "trophies", "farmables" };
        List<string> shopsubcategoriesToBeIgnored = new List<string>() { "unique_helmet", "unique_armor", "unique_shoes", "vanity", "maps", "other", "fish", "fishingbait", "fiber", "hide", "ore", "rock", "wood" };





        public string Username = "";
        public string Password = "";

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



            for (int i = 0; i < shopcategories.Count; i++)
            {
                if (shopcategories[i].Id == "gatherergear")
                {
                    // we found the gatherergear
                    shopcategories[i].Shopsubcategory = new List<ShopSubCategory>();

                    shopcategories[i].Shopsubcategory.Add(new ShopSubCategory() { id = "fibergatherer" });
                    shopcategories[i].Shopsubcategory.Add(new ShopSubCategory() { id = "oregatherer" });

                    shopcategories[i].Shopsubcategory.Add(new ShopSubCategory() { id = "woodgatherer" });
                    shopcategories[i].Shopsubcategory.Add(new ShopSubCategory() { id = "rockgatherer" });

                    shopcategories[i].Shopsubcategory.Add(new ShopSubCategory() { id = "hidegatherer" });

                    shopcategories[i].Shopsubcategory.Add(new ShopSubCategory() { id = "fishgatherer" });

                }
            }



            Settings = Methods.OpenSettings(shopcategories, shopcategoriesToBeIgnored, shopsubcategoriesToBeIgnored);



            // IsLicensed();




            PrepareUI();









        }


        private bool IsLicensed()
        {

            bool validLicense = false;

            string JWT_TOKEN = "";

            if (Settings.LicenseInformation.Username != "" && Settings.LicenseInformation.Password != "")
            {

                // Make username/password publicly known.
                Username = Settings.LicenseInformation.Username;
                Password = Settings.LicenseInformation.Password;

                Methods.SaveSettings(Settings);


                string token = LicenseManager.GetToken(Username, Password);
                Debug.WriteLine("Token:" + token);
                Methods.SaveSettings(Settings); // Token is Now stored on Ze Computzer.

                LM = new LicenseManager(token);
                validLicense = LM.ValidLicense();


            }
            else
            {
                string userName = Interaction.InputBox("If you dont have one, send me an email " + Environment.NewLine +
                         "bartklumpenaar@hotmail.com or on discord : Gwebber#2042", "Please enter your username");


                string passWord = Interaction.InputBox("If you dont have one, send me an email " + Environment.NewLine +
                         "bartklumpenaar@hotmail.com or on discord : Gwebber#2042", "Please enter your password");

                if (userName != "" && passWord != "")
                {
                    string token = LicenseManager.GetToken(userName, passWord);

                    Settings.LicenseInformation.Username = userName;
                    Settings.LicenseInformation.Password = passWord;
                    Methods.SaveSettings(Settings);


                    JWT_TOKEN = token;

                    LM = new LicenseManager(JWT_TOKEN);


                    validLicense = LM.ValidLicense();
                    Debug.WriteLine("Username and password, stored.");
                }
            }

            // If we managed to get the license key from the API, and it was still valid.
            if (!validLicense)
            {
                // We have an invalid license.

                // Resetting User credentials, so when he tries to log back in, he can try his either NEW user credentials or the same ones, wich has renewed license.
                Settings.LicenseInformation.Username = "";
                Settings.LicenseInformation.Password = "";

                // Here the user can decide, if he wants to change his username and password, or shut down the app.

                MessageBoxResult messageBoxResult = MessageBox.Show("Username and password combination did not return a valid license." + Environment.NewLine +
                    "Press yes to change your input" + Environment.NewLine +
                    "Press no, if you want to shutdown the application", "errrur", MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    Methods.SaveSettings(Settings);

                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();

                }
                else if (messageBoxResult == MessageBoxResult.No)
                {

                    Methods.SaveSettings(Settings);
                    Application.Current.Shutdown();
                }
            }





            return validLicense;
        }


        private void PrepareUI()
        {
            cmb_tier.ItemsSource = new List<string> { "4", "5", "6", "7", "8" };
            cmb_enchantment.ItemsSource = new List<string> { "0", "1", "2", "3" };
            cmb_selling_location.ItemsSource = new List<string> { "Bridgewatch", "Martlock", "Thetford", "Lymhurst", "Fort Sterling", "Caerleon" };
            cmb_buying_location.ItemsSource = new List<string> { "Bridgewatch", "Martlock", "Thetford", "Lymhurst", "Fort Sterling", "Caerleon" };
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
            cmb_returnrate.SelectedIndex = 0;
            cmb_selling_location.SelectedIndex = 1;
            cmb_buying_location.SelectedIndex = 1;
        }

        private void cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            bool newListRequired = false;
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

                                if (item == "resources")
                                {
                                    cmb_returnrate.ItemsSource = new List<string> { "43,5", "47,9", "53,9" };
                                    cmb_enchantment.IsEnabled = false;

                                }
                                else
                                {
                                    cmb_returnrate.ItemsSource = new List<string> { "43,5", "47,9" };
                                    cmb_enchantment.IsEnabled = true;

                                }


                            }
                        }


                        newListRequired = true;

                        break;
                    }
                case "cmb_subcat":
                    {
                        newListRequired = true;

                        break;
                    }

                case "cmb_tier":
                    {
                        newListRequired = true;

                        break;
                    }
                case "cmb_enchantment":

                    {
                        newListRequired = true;

                        break;
                    }

                case "cmb_returnrate":
                    {
                        newListRequired = true;
                        break;
                    }
                case "cmb_selling_location":
                    {
                        newListRequired = true;

                        break;
                    }
                case "cmb_buying_location":
                    {
                        newListRequired = true;

                        break;
                    }
            }
            UpdateRecipe(newListRequired);
        }


        private void UpdateRecipe(bool newListRequired)
        {
            if (cmb_cat.SelectedItem != null &&
                cmb_subcat.SelectedItem != null &&
                cmb_tier.SelectedItem != null &&
                cmb_enchantment.SelectedItem != null &&
                cmb_selling_location.SelectedItem != null &&
                cmb_buying_location.SelectedItem != null &&
                cmb_returnrate.SelectedItem != null &&
                Tb_Nutrition_Fee.Text != "" //&&
                                            //Tb_Spec_MainTree.Text != ""
                )

            {


                // Here we check wether our Token has expired yes or no.
                //if (!LM.ValidLicense())
                //{
                //    return;
                //}



                // All comboboxes have a value.
                string category = cmb_cat.SelectedItem.ToString();
                string subcategory = cmb_subcat.SelectedItem.ToString();
                int tier = int.Parse(cmb_tier.SelectedItem.ToString());
                int enchantment = int.Parse(cmb_enchantment.SelectedItem.ToString());
                string sellingLocation = cmb_selling_location.SelectedItem.ToString();
                string buyingLocation = cmb_buying_location.SelectedItem.ToString();



                // Make these "TryParse Methods"
                double returnrate = double.Parse(cmb_returnrate.SelectedItem.ToString()) / 100;
                int nutritionFee = int.Parse(Tb_Nutrition_Fee.Text);
                int mainTreeSpec = int.Parse(Tb_Spec_MainTree.Text);


                InputParameters inputParameters = new InputParameters { Category = category, SubCategory = subcategory, Tier = tier, Enchantment = enchantment, SellingLocation = sellingLocation , BuyingLocation = buyingLocation, ReturnRate = returnrate, NutritionFee = nutritionFee, MainTreeSpec = mainTreeSpec };


                List<SimplifiedItemV2> simplifiedItemsV2 = new List<SimplifiedItemV2>();

                Journalitem journalitem = null;



                if (category == "resources")
                {
                    for (int k = 0; k < SimplifiedItemsV2.Count; k++)
                    {
                        if (SimplifiedItemsV2[k].Shopcategory == category && SimplifiedItemsV2[k].Shopsubcategory == subcategory && SimplifiedItemsV2[k].Tier == tier)
                        {
                            simplifiedItemsV2.Add(SimplifiedItemsV2[k]);
                        }
                    }

                }
                else
                {
                    for (int k = 0; k < SimplifiedItemsV2.Count; k++)
                    {
                        if (SimplifiedItemsV2[k].Shopcategory == category && SimplifiedItemsV2[k].Shopsubcategory == subcategory && SimplifiedItemsV2[k].Tier == tier && SimplifiedItemsV2[k].Enchantment == enchantment)
                        {
                            simplifiedItemsV2.Add(SimplifiedItemsV2[k]);
                        }
                    }
                }




                List<string> itemlistForSellingLocation = new List<string>();
                List<string> itemlistForBuyingLocation = new List<string>();




                if (sellingLocation == buyingLocation)
                {


                    if (!(category == "resources") && !(category == "consumables"))
                    {
                        string n = simplifiedItemsV2[0].Uniquename;
                        if (enchantment != 0)
                        {
                            n = n.Replace($"@{enchantment}", "");
                        }
                        journalitem = Methods.FindJournalForEndproduct(journalitems, n);
                        itemlistForSellingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForSellingLocation, Methods.FindSimplifiedItem(journalitem.Uniquename, SimplifiedItemsV2).Uniquename);
                        itemlistForSellingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForSellingLocation, Methods.FindSimplifiedItem(journalitem.EmptyName, SimplifiedItemsV2).Uniquename);
                    }



                    // We can combine the order...
                    // Saving hassle.
                    for (int i = 0; i < simplifiedItemsV2.Count; i++)
                    {
                        string name = simplifiedItemsV2[i].Uniquename;
                        if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL1")) { name = name + "@1"; }
                        if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL2")) { name = name + "@2"; }
                        if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL3")) { name = name + "@3"; }

                        itemlistForSellingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForSellingLocation, name);

                        for (int j = 0; j < simplifiedItemsV2[i].Craftingrequirements[0].Craftresources.Count; j++)
                        {

                            name = simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename;

                            if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL1")) { name = name + "@1"; }
                            if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL2")) { name = name + "@2"; }
                            if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL3")) { name = name + "@3"; }


                            itemlistForSellingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForSellingLocation, name);
                        }
                    }
                }
                else
                {

                    if (!(category == "resources") && !(category == "consumables"))
                    {
                        string n = simplifiedItemsV2[0].Uniquename;
                        if (enchantment != 0)
                        {
                            n = n.Replace($"@{enchantment}", "");
                        }
                        journalitem = Methods.FindJournalForEndproduct(journalitems, n);
                        itemlistForSellingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForSellingLocation, Methods.FindSimplifiedItem(journalitem.Uniquename, SimplifiedItemsV2).Uniquename);
                        itemlistForSellingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForSellingLocation, Methods.FindSimplifiedItem(journalitem.EmptyName, SimplifiedItemsV2).Uniquename);
                    }


                    // here we have to make 2 calls to the API.

                    // First make request for endproducts
                    for (int i = 0; i < simplifiedItemsV2.Count; i++)
                    {
                        string name = simplifiedItemsV2[i].Uniquename;
                        if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL1")) { name = name + "@1"; }
                        if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL2")) { name = name + "@2"; }
                        if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL3")) { name = name + "@3"; }

                        itemlistForSellingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForSellingLocation, name);
                        for (int j = 0; j < simplifiedItemsV2[i].Craftingrequirements[0].Craftresources.Count; j++)
                        {

                            name = simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename;

                            if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL1")) { name = name + "@1"; }
                            if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL2")) { name = name + "@2"; }
                            if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL3")) { name = name + "@3"; }


                            itemlistForBuyingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForBuyingLocation, name);
                        }
                    }


                }











                //if (!(category == "resources") && !(category == "consumables"))
                //{
                //    string n = simplifiedItemsV2[0].Uniquename;
                //    if (enchantment != 0)
                //    {
                //        n = n.Replace($"@{enchantment}", "");
                //    }
                //    journalitem = Methods.FindJournalForEndproduct(journalitems, n);
                //    itemlistForSellingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForSellingLocation, Methods.FindSimplifiedItem(journalitem.Uniquename, SimplifiedItemsV2).Uniquename);
                //    itemlistForSellingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForSellingLocation, Methods.FindSimplifiedItem(journalitem.EmptyName, SimplifiedItemsV2).Uniquename);
                //}


                //for (int i = 0; i < simplifiedItemsV2.Count; i++)
                //{
                //    string name = simplifiedItemsV2[i].Uniquename;
                //    if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL1")) { name = name + "@1"; }
                //    if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL2")) { name = name + "@2"; }
                //    if (simplifiedItemsV2[i].Uniquename.Contains("_LEVEL3")) { name = name + "@3"; }

                //    itemlistForSellingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForSellingLocation, name);

                //    for (int j = 0; j < simplifiedItemsV2[i].Craftingrequirements[0].Craftresources.Count; j++)
                //    {

                //        name = simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename;

                //        if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL1")) { name = name + "@1"; }
                //        if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL2")) { name = name + "@2"; }
                //        if (simplifiedItemsV2[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("_LEVEL3")) { name = name + "@3"; }


                //        itemlistForSellingLocation = Methods.CreateItemListForAlbionDataApiV2(itemlistForSellingLocation, name);
                //    }
                //}

                List<AlbionDataPriceModel> datapriceModelsForSellingPrices = new List<AlbionDataPriceModel>() ;
                List<AlbionDataPriceModel> datapriceModelsForBuyingPrices = new List<AlbionDataPriceModel>();


                if(buyingLocation == sellingLocation)
                {
                    datapriceModelsForSellingPrices = Methods.GetMarketPrices(Methods.CreateAlbionDataUrl(new AlbionDataWebRequestModel() { Items = itemlistForSellingLocation, Locations = sellingLocation, Qualities = "1" }));
                    SimplifiedItemsV2 = Methods.UpdatePrices(datapriceModelsForSellingPrices, SimplifiedItemsV2);


                }
                else
                {
                    datapriceModelsForSellingPrices = Methods.GetMarketPrices(Methods.CreateAlbionDataUrl(new AlbionDataWebRequestModel() { Items = itemlistForSellingLocation, Locations = sellingLocation, Qualities = "1" }));
                    datapriceModelsForBuyingPrices = Methods.GetMarketPrices(Methods.CreateAlbionDataUrl(new AlbionDataWebRequestModel() { Items = itemlistForBuyingLocation, Locations = buyingLocation, Qualities = "1" }));



                    SimplifiedItemsV2 = Methods.UpdatePrices(datapriceModelsForSellingPrices, SimplifiedItemsV2);
                    SimplifiedItemsV2 = Methods.UpdatePrices(datapriceModelsForBuyingPrices, SimplifiedItemsV2);
                }
                



                if (true)
                {
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''Íncoming Prices from AlbionDataApi''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    for (int i = 0; i < datapriceModelsForSellingPrices.Count; i++)
                    {
                        Debug.WriteLine($"{datapriceModelsForSellingPrices[i].ItemId} : {datapriceModelsForSellingPrices[i].SellPriceMin}");
                    }


                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''Íncoming Prices from AlbionDataApi''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                }


                if (true)
                {
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''Íncoming Prices from AlbionDataApi''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    for (int i = 0; i < datapriceModelsForBuyingPrices.Count; i++)
                    {
                        Debug.WriteLine($"{datapriceModelsForBuyingPrices[i].ItemId} : {datapriceModelsForBuyingPrices[i].SellPriceMin}");
                    }


                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''Íncoming Prices from AlbionDataApi''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                    Debug.WriteLine("''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''");
                }




                Methods.SaveToFile(SimplifiedItemsV2);
                UIModelV2 uIModelV2 = new UIModelV2();
                uIModelV2.InputParameters = inputParameters;
                uIModelV2.simplifiedItems = simplifiedItemsV2;
                uIModelV2.Journal = journalitem;
                // uIModelV2.Specialisation = specialisationList;

                //UpdateUIV2(uIModelV2, priceUpdate);
                //UpdateUIV3(uIModelV2, priceUpdate);
                //CreateFrameWorkElement(uIModelV2, priceUpdate);


                //ObservableCollection<ItemV2> items = ItemsAttachedToUIV2;

                //Lb_Items.ItemsSource = 


                //if (newListRequired)
                //{
                //    items = CreateListForUI(uIModelV2);
                //    items = ApplySpecialisation(inputParameters, items, true);
                //}
                //else
                //{
                //    items = ApplySpecialisation(inputParameters, items, false);

                //}

                if (newListRequired)
                {
                    ItemsAttachedToUIV2 = CreateListForListbox(uIModelV2);
                    if (category == "resources")
                    {
                        ItemsAttachedToUIV2 = ApplySpecialisationForResources(inputParameters, ItemsAttachedToUIV2, true);
                    }
                    else
                    {
                        ItemsAttachedToUIV2 = ApplySpecialisationForCraftables(inputParameters, ItemsAttachedToUIV2, true);
                    }
                }
                else
                {
                    if (category == "resources")
                    {
                        ItemsAttachedToUIV2 = ApplySpecialisationForResources(inputParameters, ItemsAttachedToUIV2, false);
                    }
                    else
                    {
                        ItemsAttachedToUIV2 = ApplySpecialisationForCraftables(inputParameters, ItemsAttachedToUIV2, false);

                    }

                }

                Lb_Items.ItemsSource = null;
                Lb_Items.ItemsSource = ItemsAttachedToUIV2;


            }
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
            UpdateRecipe(false);
        }

        private ObservableCollection<ItemV2> ApplySpecialisationForResources(InputParameters inputParameters, ObservableCollection<ItemV2> items, bool categoryChanged)
        {
            string specName = $"SPEC_{inputParameters.Category}_{inputParameters.SubCategory}";
            List<FocusCostCalculcationModel> focusCostCalculcationModels = new List<FocusCostCalculcationModel>();

            int tier = 0;
            UserSpecInput userSpec = Methods.FindSetting(specName, Settings.UserSpecInput);
            // All cases, its only 1 resource.
            // This can be changed, by altering the comboboxes for selected a bit.
            // Disabling Enchantment for example.
            // This will then SHOW all resources of CLOTH for lets say T4.
            for (int i = 0; i < items.Count; i++)
            {
                int specialisationValue = 0;

                if (categoryChanged)
                {
                    // the category, changed, so now we have to check if there have been settings saved for this specialisation.
                    //Debug.WriteLine("0__" + specName);


                    specialisationValue = userSpec.Spec[items[i].Tier - 4];

                    tier = items[i].Tier;

                    //specialisationValue = userSpec.Spec[i];
                    items[0].Focus.CraftingSpec = specialisationValue.ToString();
                }
                else
                {
                    // The category did not change, so we have to just update the spec values from the userinput side.
                    specialisationValue = int.Parse(items[i].Focus.CraftingSpec);
                    // After the FOR loop, we will calculate FOCUS cost.

                    //for(int j = 0; j < items.Count; j++)
                    //{
                    //    items[j].Focus.CraftingSpec = specialisationValue.ToString();
                    //}


                    Settings = Methods.UpdateSpecialisations(Settings, specName, items[i].Tier - 4, specialisationValue);
                    //Debug.WriteLine($"Spec updated : {specialisationValue}");

                }
                FocusCostCalculcationModel focusCostCalculcationModel = new FocusCostCalculcationModel()
                {
                    UniqueName = items[i].EndProductName,
                    FocusCost = items[i].CraftingFocusForCrafting,
                    UserSpecInput = specialisationValue,
                    ArtefactItem = items[i].IsArtifact
                };

                focusCostCalculcationModels.Add(focusCostCalculcationModel);






            }

            for (int j = 0; j < 5; j++)
            {
                if (!(tier - 4 == j))
                {
                    int specialisationValue = userSpec.Spec[j];
                    FocusCostCalculcationModel focusCostCalculcationModel = new FocusCostCalculcationModel()
                    {
                        UniqueName = "empty",
                        FocusCost = 0,
                        UserSpecInput = specialisationValue,
                        ArtefactItem = false
                    };
                    focusCostCalculcationModels.Add(focusCostCalculcationModel);
                }
            }



            for (int i = 0; i < items.Count; i++)
            {


                double focusCost = Methods.CalculateFocusCosts(items[i].EndProductName, inputParameters.MainTreeSpec, focusCostCalculcationModels);
                //Debug.WriteLine($"FocusCost for {items[i].EndProductName} {focusCost}");
                items[i].Focus.FocusCost = focusCost.ToString();



            }


            return items;
        }


        private ObservableCollection<ItemV2> ApplySpecialisationForCraftables(InputParameters inputParameters, ObservableCollection<ItemV2> items, bool categoryChanged)
        {


            List<FocusCostCalculcationModel> focusCostCalculcationModels = new List<FocusCostCalculcationModel>();
            string specName = $"SPEC_{inputParameters.Category}_{inputParameters.SubCategory}";

            // Grab spec from settings.
            // Grab spec from userInput, if there is an itemsource.
            for (int i = 0; i < items.Count; i++)
            {

                if (items[i].EndProductName.Contains("ROYAL"))
                {
                    // Royal items dont have specialisation.
                }
                else
                {
                    int specialisationValue = 0;

                    if (categoryChanged)
                    {
                        // the category, changed, so now we have to check if there have been settings saved for this specialisation.
                        Debug.WriteLine("0__" + specName);
                        UserSpecInput userSpec = Methods.FindSetting(specName, Settings.UserSpecInput);
                        specialisationValue = userSpec.Spec[i];
                        items[i].Focus.CraftingSpec = specialisationValue.ToString();
                    }
                    else
                    {
                        // The category did not change, so we have to just update the spec values from the userinput side.
                        specialisationValue = int.Parse(items[i].Focus.CraftingSpec);
                        // After the FOR loop, we will calculate FOCUS cost.
                        Settings = Methods.UpdateSpecialisations(Settings, specName, i, specialisationValue);
                        Debug.WriteLine($"Spec updated : {specialisationValue}");

                    }

                    FocusCostCalculcationModel focusCostCalculcationModel = new FocusCostCalculcationModel()
                    {
                        UniqueName = items[i].EndProductName,
                        FocusCost = items[i].CraftingFocusForCrafting,
                        UserSpecInput = specialisationValue,
                        ArtefactItem = items[i].IsArtifact
                    };

                    focusCostCalculcationModels.Add(focusCostCalculcationModel);
                }


            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].EndProductName.Contains("ROYAL"))
                {
                    items[i].Focus.Visible = "Hidden";
                }
                else
                {
                    double focusCost = Methods.CalculateFocusCosts(items[i].EndProductName, inputParameters.MainTreeSpec, focusCostCalculcationModels);
                    Debug.WriteLine($"FocusCost for {items[i].EndProductName} {focusCost}");
                    items[i].Focus.FocusCost = focusCost.ToString();
                }


            }

            return items;
        }


        private ObservableCollection<ItemV2> CreateListForRefining(UIModelV2 uIModel)
        {
            ObservableCollection<ItemV2> items = new ObservableCollection<ItemV2>();





            for (int i = 0; i < uIModel.simplifiedItems.Count; i++)
            {


                // Create the endproduct.
                ItemV2 item = new ItemV2(ShowGridLines);

                item.EndProductName = uIModel.simplifiedItems[i].Uniquename;
                item.EndProductPrice = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Uniquename, uIModel.InputParameters.SellingLocation).ToString();
                item.EndProductImage = new BitmapImage(new Uri(Methods.DownloadIfNotExistsLocallyV2(item.EndProductName), UriKind.Absolute));
                AlbionCraftingInformation albionCraftingInformation = Methods.CalculateProfitV3(uIModel.simplifiedItems[i], uIModel.InputParameters.NutritionFee, 0.045, uIModel.InputParameters.SellingLocation , uIModel.InputParameters.BuyingLocation, SimplifiedItemsV2, uIModel.Journal, uIModel.InputParameters.ReturnRate);

                for (int j = 0; j < uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources.Count; j++)
                {
                    Ingredients ingredient = new Ingredients();


                    int investment = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[j].Count;
                    int returned = albionCraftingInformation.Resources.ResourceReturnModels[j].Count;
                    int cost = investment - returned;
                    int ingPrice = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[j].Uniquename, uIModel.InputParameters.SellingLocation);

                    string ingName = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[j].Uniquename;
                    BitmapImage bitmapImage = new BitmapImage(new Uri(Methods.DownloadIfNotExistsLocallyV2(ingName), UriKind.Absolute));

                    ingredient.IngredientName = ingName + " * (" + investment.ToString() + "-" + returned.ToString() + ") = " + cost.ToString();
                    ingredient.IngredientPrice = ingPrice.ToString() + " * " + investment.ToString() + "-" + returned.ToString() + " = " + (cost * ingPrice);
                    ingredient.IngredientImage = bitmapImage;


                    item.Ingredients.Add(ingredient);
                }

                // If we have less then 4 ingredients, we will populate the rest with empty classes
                if (uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources.Count < 4)
                {
                    for (int j = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources.Count; j < 4; j++)
                    {
                        Ingredients ingredient = new Ingredients();
                        item.Ingredients.Add(ingredient);
                    }
                }

                int focusCost;
                int.TryParse(uIModel.simplifiedItems[i].Craftingrequirements[0].Craftingfocus, out focusCost);

                item.CraftingFocusForCrafting = focusCost;


            }





            return items;
        }


        private ObservableCollection<ItemV2> CreateListForListbox(UIModelV2 uIModel)
        {




            ObservableCollection<ItemV2> items = new ObservableCollection<ItemV2>();

            for (int i = 0; i < uIModel.simplifiedItems.Count; i++)
            {
                ItemV2 item = new ItemV2(false);



                item.EndProductName = uIModel.simplifiedItems[i].Uniquename;
                item.EndProductPrice = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Uniquename, uIModel.InputParameters.SellingLocation).ToString();
                item.EndProductImage = new BitmapImage(new Uri(Methods.DownloadIfNotExistsLocallyV2(item.EndProductName), UriKind.Absolute));

                AlbionCraftingInformation albionCraftingInformation = Methods.CalculateProfitV3(uIModel.simplifiedItems[i], uIModel.InputParameters.NutritionFee, 0.045, uIModel.InputParameters.SellingLocation , uIModel.InputParameters.BuyingLocation, SimplifiedItemsV2, uIModel.Journal, uIModel.InputParameters.ReturnRate);

                for (int j = 0; j < uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources.Count; j++)
                {
                    Ingredients ingredient = new Ingredients();


                    int investment = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[j].Count;
                    int returned = albionCraftingInformation.Resources.ResourceReturnModels[j].Count;
                    int cost = investment - returned;
                    int ingPrice = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[j].Uniquename, uIModel.InputParameters.SellingLocation);

                    string ingName = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[j].Uniquename;
                    BitmapImage bitmapImage = new BitmapImage(new Uri(Methods.DownloadIfNotExistsLocallyV2(ingName), UriKind.Absolute));
                    if (uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources[j].Uniquename.Contains("ARTEFACT"))
                    {
                        ingName = "Artefact";
                        item.IsArtifact = true;
                    }



                    //item.Focus.Visible = true;


                    ingredient.IngredientName = ingName + " * (" + investment.ToString() + "-" + returned.ToString() + ") = " + cost.ToString();
                    ingredient.IngredientPrice = ingPrice.ToString() + " * " + investment.ToString() + "-" + returned.ToString() + " = " + (cost * ingPrice);
                    ingredient.IngredientImage = bitmapImage;


                    item.Ingredients.Add(ingredient);
                }

                // If we have less then 4 ingredients, we will populate the rest with empty classes
                if (uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources.Count < 4)
                {
                    for (int j = uIModel.simplifiedItems[i].Craftingrequirements[0].Craftresources.Count; j < 4; j++)
                    {
                        Ingredients ingredient = new Ingredients();
                        item.Ingredients.Add(ingredient);
                    }
                }

                if (uIModel.Journal != null)
                {
                    ///////////////////////////////////////////////////////////////////////
                    ///     Is it a royal item ?
                    ///////////////////////////////////////////////////////////////////////
                    if (uIModel.simplifiedItems[i].Uniquename.Contains("ROYAL"))
                    {
                        // Make the JOURNAL part dissapear.

                        item.Journal.JournalFullPrice = "0";
                        item.Journal.JournalEmptyPrice = "0";
                        item.Journal.JournalFilledPercentage = "0";
                        item.Journal.JournalImage = null;
                        item.Journal.Visible = "Hidden";
                        item.Focus.Visible = "Hidden";



                    }
                    ///////////////////////////////////////////////////////////////////////
                    ///     Is it not a royal item ?, and we have a journal.
                    ///     This applies, to most items we are looking for generally.
                    ///////////////////////////////////////////////////////////////////////
                    else
                    {
                        int journalPrice = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.Journal.Uniquename, uIModel.InputParameters.SellingLocation);
                        int journal_empty_price = Methods.GetPriceFromItemsList(SimplifiedItemsV2, uIModel.Journal.EmptyName, uIModel.InputParameters.SellingLocation);
                        double percentageFilled = Math.Round(albionCraftingInformation.Journal.JournalFilledPercentage, 3) * 100;
                        double filledPercentageValue = Math.Round(albionCraftingInformation.Journal.JournalFilledPercentageValue, 3);

                        BitmapImage bitmapImage = new BitmapImage(new Uri(Methods.DownloadIfNotExistsLocallyV2(uIModel.Journal.Uniquename), UriKind.Absolute));

                        item.Journal.JournalFullPrice = "Full = " + journalPrice.ToString();
                        item.Journal.JournalEmptyPrice = "Empty = " + journal_empty_price.ToString();
                        item.Journal.JournalFilledPercentage = percentageFilled.ToString() + "% = " + Math.Round(filledPercentageValue, 0);
                        item.Journal.Visible = "Visible";
                        item.Journal.JournalImage = bitmapImage;




                    }
                }
                else
                ///////////////////////////////////////////////////////////////////////
                ///     We have NO journal
                ///     This applies to Consumables,Mounts
                ///     But there is still a nutrition cost.
                ///////////////////////////////////////////////////////////////////////
                {
                    item.Journal.JournalFullPrice = "";
                    item.Journal.JournalEmptyPrice = "";
                    item.Journal.JournalFilledPercentage = "";
                    item.Journal.JournalImage = null;
                    item.Focus.Visible = "Hidden";
                    item.Extras.NutritionTax = albionCraftingInformation.NutritionCost.ToString();
                    item.Extras.SellOrderTax = albionCraftingInformation.SellorderCost.ToString();
                }
                item.Focus.Visible = "Visible";

                int focusCost;
                int.TryParse(uIModel.simplifiedItems[i].Craftingrequirements[0].Craftingfocus, out focusCost);

                item.CraftingFocusForCrafting = focusCost;

                item.Extras.NutritionTax = albionCraftingInformation.NutritionCost.ToString();

                item.Extras.SellOrderTax = albionCraftingInformation.SellorderCost.ToString();

                item.Tier = uIModel.simplifiedItems[i].Tier;
                item.Enchantment = uIModel.simplifiedItems[i].Enchantment;






                if (i == 1 || i == 3 || i == 5 || i == 7)
                {
                    item.BackgroundColor = "#CD8C57";
                    //(Lb_Items.Items[i] as ListBoxItem).Background = new SolidColorBrush(Colors.Orange);

                }
                else
                {
                    item.BackgroundColor = "Transparent";
                    //(Lb_Items.Items[i] as ListBoxItem).Background = new SolidColorBrush(Colors.Transparent);


                }


                item.Profit.Investment = "IN " + albionCraftingInformation.Profit.Investment.ToString();
                item.Profit.Returned = "OUT " + albionCraftingInformation.Profit.Returned.ToString();
                //item.Profit.IsProfitable = albionCraftingInformation.Profit.IsProfitable;

                BitmapImage image;

                if (albionCraftingInformation.Profit.IsProfitable)
                {
                    image = new BitmapImage((new Uri("pack://application:,,,/Img/StatusOK.png")));
                }
                else
                {
                    image = new BitmapImage((new Uri("pack://application:,,,/Img/StatusError.png")));

                }

                item.Profit.ProfitImage = image;

                if (uIModel.InputParameters.Category == "resources")
                {
                    if (i == 0)
                    {
                        item.Focus.IsEnabled = "true";
                    }
                    else
                    {
                        item.Focus.IsEnabled = "false";

                    }
                }
                else
                {
                    item.Focus.IsEnabled = "true";
                }


                items.Add(item);
            }










            return items;
        }
    }

    public class ItemV2
    {
        public ItemV2(bool showGridLines)
        {
            ShowGridLines = showGridLines.ToString();
        }

        public string EndProductName { get; set; } = "";
        public string EndProductPrice { get; set; } = "";
        public BitmapImage EndProductImage { get; set; } = null;

        public int Tier { get; set; }
        public int Enchantment { get; set; }

        public int CraftingFocusForCrafting { get; set; }

        public bool IsArtifact { get; set; }

        public List<Ingredients> Ingredients { get; set; } = new List<Ingredients>();

        public JournalUI Journal { get; set; } = new JournalUI();
        public Focus Focus { get; set; } = new Focus();
        public Extras Extras { get; set; } = new Extras();

        public ProfitUI Profit { get; set; } = new ProfitUI();



        // Theme
        public string BackgroundColor { get; set; } = "";

        public string ShowGridLines { get; set; }
    }




    public class Extras
    {
        public string SellOrderTax { get; set; } = "";

        public string NutritionTax { get; set; } = "";
    }

    public class Ingredients
    {
        public BitmapImage IngredientImage { get; set; } = null;

        public string IngredientName { get; set; } = "";
        public string IngredientPrice { get; set; } = "";

    }


    public class JournalUI
    {
        public string JournalFilledPercentage { get; set; } = "";
        public string JournalFullPrice { get; set; } = "";

        public string JournalEmptyPrice { get; set; } = "";

        public string JournalFilledPercentageValue { get; set; } = "";
        public BitmapImage JournalImage { get; set; } = null;

        public string Visible { get; set; } = "Hidden";
    }


    public class Focus
    {
        public string CraftingSpec { get; set; } = "0";

        public string FocusCost { get; set; } = "0";

        public string Visible { get; set; } = "Hidden";

        public string IsEnabled { get; set; } = "false";
    }



    public static class UIElementExtensions
    {
        public static T FindControl<T>(this Visual parent, string ControlName) where T : FrameworkElement
        {
            if (parent == null)
                return null;

            if (parent.GetType() == typeof(T) && ((T)parent).Name == ControlName)
            {
                //Debug.WriteLine((parent as FrameworkElement).Name);
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
            //Debug.WriteLine((result as FrameworkElement).Name);

            return result;
        }
    }


}
