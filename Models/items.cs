using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionOnlineCraftingCalculator
{
    public class Acceptedfood
    {
        [JsonProperty("@foodcategory")]
        public string Foodcategory { get; set; }
    }

    public class AssetVfxPreset
    {
        [JsonProperty("@name")]
        public string Name { get; set; }
    }

    public class AudioInfo
    {
        [JsonProperty("@name")]
        public string Name { get; set; }
    }

    public class Canharvest
    {
        [JsonProperty("@resourcetype")]
        public string Resourcetype { get; set; }
    }

    public class Consumablefrominventoryitem
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@tradable")]
        public string Tradable { get; set; }

        [JsonProperty("@uisprite")]
        public string Uisprite { get; set; }

        [JsonProperty("@abilitypower")]
        public string Abilitypower { get; set; }

        [JsonProperty("@consumespell")]
        public string Consumespell { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@tier")]
        public int Tier { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }

        [JsonProperty("@dummyitempower")]
        public string Dummyitempower { get; set; }

        [JsonProperty("@maxstacksize")]
        public string Maxstacksize { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@uicraftsoundstart")]
        public string Uicraftsoundstart { get; set; }

        [JsonProperty("@uicraftsoundfinish")]
        public string Uicraftsoundfinish { get; set; }

        [JsonProperty("@uispriteoverlay1")]
        public string Uispriteoverlay1 { get; set; }

        [JsonProperty("@Itemvalue")]
        public double Itemvalue { get; set; }
        [JsonProperty("@craftingrequirements")]
        public List<Craftingrequirements> Craftingrequirements { get; set; }


        [JsonProperty("@allowfullstackusage")]
        public string Allowfullstackusage { get; set; }

        [JsonProperty("@logconsumption")]
        public string Logconsumption { get; set; }

        [JsonProperty("@enchantmentlevel")]
        public string Enchantmentlevel { get; set; }

        [JsonProperty("@descriptionlocatag")]
        public string Descriptionlocatag { get; set; }

        [JsonProperty("@craftingcategory")]
        public string Craftingcategory { get; set; }
    }

    public class Consumableitem
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@fishingfame")]
        public string Fishingfame { get; set; }

        [JsonProperty("@fishingminigamesetting")]
        public string Fishingminigamesetting { get; set; }

        [JsonProperty("@descriptionlocatag")]
        public string Descriptionlocatag { get; set; }

        [JsonProperty("@uisprite")]
        public string Uisprite { get; set; }

        [JsonProperty("@nutrition")]
        public string Nutrition { get; set; }

        [JsonProperty("@abilitypower")]
        public string Abilitypower { get; set; }

        [JsonProperty("@slottype")]
        public string Slottype { get; set; }

        [JsonProperty("@consumespell")]
        public string Consumespell { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@resourcetype")]
        public string Resourcetype { get; set; }

        [JsonProperty("@tier")]
        public int Tier { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }

        [JsonProperty("@dummyitempower")]
        public string Dummyitempower { get; set; }

        [JsonProperty("@maxstacksize")]
        public string Maxstacksize { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@unlockedtoequip")]
        public string Unlockedtoequip { get; set; }

        [JsonProperty("@uicraftsoundstart")]
        public string Uicraftsoundstart { get; set; }

        [JsonProperty("@uicraftsoundfinish")]
        public string Uicraftsoundfinish { get; set; }

        [JsonProperty("@craftingcategory")]
        public string Craftingcategory { get; set; }



        [JsonProperty("craftingrequirements")]
        [JsonConverter(typeof(CraftingrequirementsConverter))]
        public List<Craftingrequirements> Craftingrequirements { get; set; }



        [JsonProperty("enchantments")]
        public Enchantments Enchantments { get; set; }


    }

    public class Consumption
    {
        public Food Food { get; set; }
    }

    public class Container
    {
        [JsonProperty("@capacity")]
        public string Capacity { get; set; }

        [JsonProperty("@weightlimit")]
        public string Weightlimit { get; set; }
    }

    public class Craftingrequirements
    {
        [JsonProperty("@silver")]
        public string Silver { get; set; }

        [JsonProperty("@time")]
        public string Time { get; set; }

        [JsonProperty("@craftingfocus")]
       // [JsonConverter(typeof(NullConverter))]
        public string Craftingfocus { get; set; }



        [JsonProperty("craftresource")]
        [JsonConverter(typeof(CraftresourcesConverter))]
        public List<Craftresource> Craftresources { get; set; }

        //public Craftresource[] Craftresources { get; set; }

        [JsonProperty("@swaptransaction")]
        public string Swaptransaction { get; set; }

        [JsonProperty("@amountcrafted")]
        public int Amountcrafted { get; set; }

        [JsonProperty("@forcesinglecraft")]
        public string Forcesinglecraft { get; set; }
        public Currency Currency { get; set; }
    }

    public class NullConverter : JsonConverter
    {

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int i = 0;

            if (reader.TokenType == JsonToken.Null)
            {
                

                return i;


            }
            else //if (reader.TokenType == JsonToken.StartObject)
            {

                //Debug.WriteLine($"{reader.TokenType}");

                // Single Object
                var o = JObject.Load(reader);
                
                return int.Parse( o.ToObject<string>());
            }


            //return i;
        }

        public override bool CanConvert(Type objectType) => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }


    public class EnchantmentsConverter : JsonConverter
    {

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {


            if (reader.TokenType == JsonToken.StartArray)
            {
                // Multi Object
                var a = JArray.Load(reader);
                var l = new List<Enchantment>();

                foreach (var item in a)
                {
                    l.Add(item.ToObject<Enchantment>());
                }

                return l;


            }
            else
            {
                // Single Object
                var o = JObject.Load(reader);
                var l = new List<Enchantment>
                {
                    o.ToObject<Enchantment>()
                };
                return l;
            }
        }

        public override bool CanConvert(Type objectType) => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }






    public class CraftresourcesConverter : JsonConverter
    {

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {


            if (reader.TokenType == JsonToken.StartArray)
            {
                // Multi Object
                var a = JArray.Load(reader);
                var l = new List<Craftresource>();

                foreach (var item in a)
                {
                    l.Add(item.ToObject<Craftresource>());
                }

                return l;


            }
            else if(reader.TokenType == JsonToken.StartObject)
            {
                //List<Craftresource> l = new List<Craftresource>();
                //// Single Object
                //if (reader.Value == null) 
                //{

                //}
                //else
                //{
                //    var o = JObject.Load(reader);
                //    l.Add(o.ToObject<Craftresource>());
                //}

                // Single Object


               
                    var o = JObject.Load(reader);
                    var l = new List<Craftresource>
                {
                    o.ToObject<Craftresource>()
                };
                    return l;
                
                



                
            }
            else if(reader.TokenType == JsonToken.Null)
            {
                return new List<Craftresource>();
            }


            return new List<Craftresource>();
        }

        public override bool CanConvert(Type objectType) => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }



    public class CraftingrequirementsConverter : JsonConverter
    {

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
 

            if (reader.TokenType == JsonToken.StartArray)
            {
                // Multi Object
                var a = JArray.Load(reader);
                var l = new List<Craftingrequirements>();

                foreach (var item in a)
                {
                    l.Add(item.ToObject<Craftingrequirements>());
                }

                return l;


            }
            else
            {
                // Single Object
                var o = JObject.Load(reader);
                var l = new List<Craftingrequirements>
                {
                    o.ToObject<Craftingrequirements>()
                };
                return l;
            }
        }

        public override bool CanConvert(Type objectType) => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }


    public class ShopsubcategoriesConverter : JsonConverter
    {

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {


            if (reader.TokenType == JsonToken.StartArray)
            {
                // Multi Object
                var a = JArray.Load(reader);
                var l = new List<ShopSubCategory>();

                foreach (var item in a)
                {
                    l.Add(item.ToObject<ShopSubCategory>());
                }

                return l;


            }
            else
            {
                // Single Object
                var o = JObject.Load(reader);
                var l = new List<ShopSubCategory>
                {
                    o.ToObject<ShopSubCategory>()
                };
                return l;
            }
        }

        public override bool CanConvert(Type objectType) => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }





    public class Craftingspelllist
    {
        [JsonProperty("@craftspell")]
        public Craftspell[] Craftspell { get; set; }
    }

    public class Craftitemfame
    {
        [JsonProperty("@mintier")]
        public string Mintier { get; set; }

        [JsonProperty("@value")]
        public string Value { get; set; }
        public List<Validitem> Validitem { get; set; }
    }

    public class Craftresource
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@count")]
        public int Count { get; set; }

        [JsonProperty("@maxreturnamount")]
        public string Maxreturnamount { get; set; }

        [JsonProperty("@enchantmentlevel")]
        public string Enchantmentlevel { get; set; }
    }

    public class Craftspell
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }
    }

    public class Crystalleagueitem
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@uisprite")]
        public string Uisprite { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@tier")]
        public string Tier { get; set; }

        [JsonProperty("@enchantmentlevel")]
        public string Enchantmentlevel { get; set; }

        [JsonProperty("@resourcetype")]
        public string Resourcetype { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }

        [JsonProperty("@maxstacksize")]
        public string Maxstacksize { get; set; }

        [JsonProperty("@namelocatag")]
        public string Namelocatag { get; set; }

        [JsonProperty("@descriptionlocatag")]
        public string Descriptionlocatag { get; set; }

        [JsonProperty("@descvariable0")]
        public string Descvariable0 { get; set; }

        [JsonProperty("@salvageable")]
        public string Salvageable { get; set; }

        [JsonProperty("@Itemvalue")]
        public double Itemvalue { get; set; }

        [JsonProperty("@tradable")]
        public string Tradable { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@canbestoredinbattlevault")]
        public string Canbestoredinbattlevault { get; set; }




        [JsonConverter(typeof(CraftingrequirementsConverter))]
        [JsonProperty("craftingrequirements")]
        public List<Craftingrequirements> Craftingrequirements { get; set; }

        [JsonProperty("@uispriteoverlay1")]
        public string Uispriteoverlay1 { get; set; }
    }

    public class Currency
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@amount")]
        public string Amount { get; set; }
    }

    public class Enchantment
    {
        [JsonProperty("@enchantmentlevel")]
        public int Enchantmentlevel { get; set; }

        [JsonProperty("@abilitypower")]
        public string Abilitypower { get; set; }

        [JsonProperty("@dummyitempower")]
        public string Dummyitempower { get; set; }

        [JsonProperty("@consumespell")]
        public string Consumespell { get; set; }




        [JsonConverter(typeof(CraftingrequirementsConverter))]
        [JsonProperty("craftingrequirements")]
        public List<Craftingrequirements> Craftingrequirements { get; set; }
        public Upgraderequirements Upgraderequirements { get; set; }

        [JsonProperty("@itempower")]
        public string Itempower { get; set; }

        [JsonProperty("@durability")]
        public string Durability { get; set; }
    }

    public class Enchantments
    {

        [JsonConverter(typeof(EnchantmentsConverter))]
        [JsonProperty("enchantment")]

        public List<Enchantment> Enchantment { get; set; }
    }

    public class Equipmentitem
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@uisprite")]
        public string Uisprite { get; set; }

        [JsonProperty("@maxqualitylevel")]
        public string Maxqualitylevel { get; set; }

        [JsonProperty("@abilitypower")]
        public string Abilitypower { get; set; }

        [JsonProperty("@slottype")]
        public string Slottype { get; set; }

        [JsonProperty("@itempowerprogressiontype")]
        public string Itempowerprogressiontype { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@uicraftsoundstart")]
        public string Uicraftsoundstart { get; set; }

        [JsonProperty("@uicraftsoundfinish")]
        public string Uicraftsoundfinish { get; set; }

        [JsonProperty("@skincount")]
        public string Skincount { get; set; }

        [JsonProperty("@tier")]
        public int Tier { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }

        [JsonProperty("@activespellslots")]
        public string Activespellslots { get; set; }

        [JsonProperty("@passivespellslots")]
        public string Passivespellslots { get; set; }

        [JsonProperty("@physicalarmor")]
        public string Physicalarmor { get; set; }

        [JsonProperty("@magicresistance")]
        public string Magicresistance { get; set; }

        [JsonProperty("@durability")]
        public string Durability { get; set; }

        [JsonProperty("@durabilityloss_attack")]
        public string DurabilitylossAttack { get; set; }

        [JsonProperty("@durabilityloss_spelluse")]
        public string DurabilitylossSpelluse { get; set; }

        [JsonProperty("@durabilityloss_receivedattack")]
        public string DurabilitylossReceivedattack { get; set; }

        [JsonProperty("@durabilityloss_receivedspell")]
        public string DurabilitylossReceivedspell { get; set; }

        [JsonProperty("@offhandanimationtype")]
        public string Offhandanimationtype { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@unlockedtoequip")]
        public string Unlockedtoequip { get; set; }

        [JsonProperty("@hitpointsmax")]
        public string Hitpointsmax { get; set; }

        [JsonProperty("@hitpointsregenerationbonus")]
        public string Hitpointsregenerationbonus { get; set; }

        [JsonProperty("@energymax")]
        public string Energymax { get; set; }

        [JsonProperty("@energyregenerationbonus")]
        public string Energyregenerationbonus { get; set; }

        [JsonProperty("@crowdcontrolresistance")]
        public string Crowdcontrolresistance { get; set; }

        [JsonProperty("@itempower")]
        public string Itempower { get; set; }

        [JsonProperty("@physicalattackdamagebonus")]
        public string Physicalattackdamagebonus { get; set; }

        [JsonProperty("@magicattackdamagebonus")]
        public string Magicattackdamagebonus { get; set; }

        [JsonProperty("@physicalspelldamagebonus")]
        public string Physicalspelldamagebonus { get; set; }

        [JsonProperty("@magicspelldamagebonus")]
        public string Magicspelldamagebonus { get; set; }

        [JsonProperty("@healbonus")]
        public string Healbonus { get; set; }

        [JsonProperty("@bonusccdurationvsplayers")]
        public string Bonusccdurationvsplayers { get; set; }

        [JsonProperty("@bonusccdurationvsmobs")]
        public string Bonusccdurationvsmobs { get; set; }

        [JsonProperty("@threatbonus")]
        public string Threatbonus { get; set; }

        [JsonProperty("@magiccooldownreduction")]
        public string Magiccooldownreduction { get; set; }

        [JsonProperty("@bonusdefensevsplayers")]
        public string Bonusdefensevsplayers { get; set; }

        [JsonProperty("@bonusdefensevsmobs")]
        public string Bonusdefensevsmobs { get; set; }

        [JsonProperty("@magiccasttimereduction")]
        public string Magiccasttimereduction { get; set; }

        [JsonProperty("@attackspeedbonus")]
        public string Attackspeedbonus { get; set; }

        [JsonProperty("@movespeedbonus")]
        public string Movespeedbonus { get; set; }

        [JsonProperty("@healmodifier")]
        public string Healmodifier { get; set; }

        [JsonProperty("@canbeovercharged")]
        public string Canbeovercharged { get; set; }

        [JsonProperty("@showinmarketplace")]
        public string Showinmarketplace { get; set; }

        [JsonProperty("@energycostreduction")]
        public string Energycostreduction { get; set; }

        [JsonProperty("@masterymodifier")]
        public string Masterymodifier { get; set; }
        [JsonProperty("craftingrequirements")]
        [JsonConverter(typeof(CraftingrequirementsConverter))]
        public List<Craftingrequirements> Craftingrequirements { get; set; }




        [JsonProperty("@craftingcategory")]
        public string Craftingcategory { get; set; }

        [JsonProperty("@combatspecachievement")]
        public string Combatspecachievement { get; set; }
        public SocketPreset SocketPreset { get; set; }
        [JsonProperty("enchantments")]
        public Enchantments Enchantments { get; set; }

        [JsonProperty("@destinycraftfamefactor")]
        public string Destinycraftfamefactor { get; set; }
    }

    public class Famefillingmissions
    {
        public Gatherfame Gatherfame { get; set; }
        public Craftitemfame Craftitemfame { get; set; }
    }

    public class Farmableitem
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@tier")]
        public int Tier { get; set; }

        [JsonProperty("@placefame")]
        public string Placefame { get; set; }

        [JsonProperty("@pickupable")]
        public string Pickupable { get; set; }

        [JsonProperty("@destroyable")]
        public string Destroyable { get; set; }

        [JsonProperty("@unlockedtoplace")]
        public string Unlockedtoplace { get; set; }

        [JsonProperty("@maxstacksize")]
        public string Maxstacksize { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@kind")]
        public string Kind { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@animationid")]
        public string Animationid { get; set; }

        [JsonProperty("@activefarmfocuscost")]
        public string Activefarmfocuscost { get; set; }

        [JsonProperty("@activefarmmaxcycles")]
        public string Activefarmmaxcycles { get; set; }

        [JsonProperty("@activefarmactiondurationseconds")]
        public string Activefarmactiondurationseconds { get; set; }

        [JsonProperty("@activefarmcyclelengthseconds")]
        public string Activefarmcyclelengthseconds { get; set; }

        [JsonProperty("@activefarmbonus")]
        public string Activefarmbonus { get; set; }

        [JsonProperty("@Itemvalue")]
        public double Itemvalue { get; set; }

        [JsonProperty("craftingrequirements")]
        [JsonConverter(typeof(CraftingrequirementsConverter))]
        public List<Craftingrequirements> Craftingrequirements { get; set; }

        public AudioInfo AudioInfo { get; set; }
        public Harvest Harvest { get; set; }

        [JsonProperty("@prefabname")]
        public string Prefabname { get; set; }

        [JsonProperty("@prefabscale")]
        public string Prefabscale { get; set; }

        [JsonProperty("@resourcevalue")]
        public string Resourcevalue { get; set; }
        public Grownitem Grownitem { get; set; }
        public Consumption Consumption { get; set; }

        [JsonProperty("@tile")]
        public string Tile { get; set; }

        [JsonProperty("@uisprite")]
        public string Uisprite { get; set; }

        [JsonProperty("@showinmarketplace")]
        public string Showinmarketplace { get; set; }

        [JsonProperty("@hideinmarketplaceuntil")]
        public DateTime? Hideinmarketplaceuntil { get; set; }
    }

    public class Food
    {
        [JsonProperty("@nutritionmax")]
        public string Nutritionmax { get; set; }

        [JsonProperty("@secondspernutrition")]
        public string Secondspernutrition { get; set; }
        public Acceptedfood Acceptedfood { get; set; }

        [JsonProperty("@lossbeforehungry")]
        public string Lossbeforehungry { get; set; }
    }

    public class FootStepVfxPreset
    {
        [JsonProperty("@name")]
        public string Name { get; set; }
    }

    public class Furnitureitem
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@tier")]
        public int Tier { get; set; }

        [JsonProperty("@durability")]
        public string Durability { get; set; }

        [JsonProperty("@durabilitylossperdayfactor")]
        public string Durabilitylossperdayfactor { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@placeableindoors")]
        public string Placeableindoors { get; set; }

        [JsonProperty("@placeableoutdoors")]
        public string Placeableoutdoors { get; set; }

        [JsonProperty("@placeableindungeons")]
        public string Placeableindungeons { get; set; }

        [JsonProperty("@placeableinexpeditions")]
        public string Placeableinexpeditions { get; set; }

        [JsonProperty("@accessrightspreset")]
        public string Accessrightspreset { get; set; }

        [JsonProperty("@uicraftsoundstart")]
        public string Uicraftsoundstart { get; set; }

        [JsonProperty("@uicraftsoundfinish")]
        public string Uicraftsoundfinish { get; set; }
        [JsonProperty("craftingrequirements")]
        [JsonConverter(typeof(CraftingrequirementsConverter))]
        public List<Craftingrequirements> Craftingrequirements { get; set; }

        public Repairkit Repairkit { get; set; }

        [JsonProperty("@customizewithguildlogo")]
        public string Customizewithguildlogo { get; set; }

        [JsonProperty("@uisprite")]
        public string Uisprite { get; set; }

        [JsonProperty("@enchantmentlevel")]
        public string Enchantmentlevel { get; set; }

        [JsonProperty("@tile")]
        public string Tile { get; set; }

        [JsonProperty("@Itemvalue")]
        public double Itemvalue { get; set; }
        public Container Container { get; set; }

        [JsonProperty("@showinmarketplace")]
        public string Showinmarketplace { get; set; }

        [JsonProperty("@residencyslots")]
        public string Residencyslots { get; set; }

        [JsonProperty("@labourerfurnituretype")]
        public string Labourerfurnituretype { get; set; }

        [JsonProperty("@labourersaffected")]
        public string Labourersaffected { get; set; }

        [JsonProperty("@labourerhappiness")]
        public string Labourerhappiness { get; set; }

        [JsonProperty("@labourersperfurnitureitem")]
        public string Labourersperfurnitureitem { get; set; }

        [JsonProperty("@placeableonlyonislands")]
        public string Placeableonlyonislands { get; set; }

        [JsonProperty("@descriptionlocatag")]
        public string Descriptionlocatag { get; set; }
    }

    public class Gatherfame
    {
        [JsonProperty("@mintier")]
        public string Mintier { get; set; }

        [JsonProperty("@value")]
        public string Value { get; set; }
        public List<Validitem> Validitem { get; set; }
    }

    public class Grownitem
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@growtime")]
        public string Growtime { get; set; }

        [JsonProperty("@fame")]
        public string Fame { get; set; }
        public Offspring Offspring { get; set; }
    }

    public class Harvest
    {
        [JsonProperty("@growtime")]
        public string Growtime { get; set; }

        [JsonProperty("@lootlist")]
        public string Lootlist { get; set; }

        [JsonProperty("@lootchance")]
        public string Lootchance { get; set; }

        [JsonProperty("@fame")]
        public string Fame { get; set; }
        public Seed Seed { get; set; }
    }

    public class Hideoutitem
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@Itemvalue")]
        public double Itemvalue { get; set; }

        [JsonProperty("@tier")]
        public string Tier { get; set; }

        [JsonProperty("@mindistance")]
        public string Mindistance { get; set; }

        [JsonProperty("@mindistanceintunnel")]
        public string Mindistanceintunnel { get; set; }

        [JsonProperty("@placementduration")]
        public string Placementduration { get; set; }

        [JsonProperty("@primetimedurationminutes")]
        public string Primetimedurationminutes { get; set; }

        [JsonProperty("@maxstacksize")]
        public string Maxstacksize { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@uicraftsoundstart")]
        public string Uicraftsoundstart { get; set; }

        [JsonProperty("@uicraftsoundfinish")]
        public string Uicraftsoundfinish { get; set; }
        [JsonProperty("craftingrequirements")]
        [JsonConverter(typeof(CraftingrequirementsConverter))]
        public List<Craftingrequirements> Craftingrequirements { get; set; }

    }

    public class Items
    {
        [JsonProperty("@xmlns:xsi")]
        public string XmlnsXsi { get; set; }

        [JsonProperty("@xsi:noNamespaceSchemaLocation")]
        public string XsiNoNamespaceSchemaLocation { get; set; }
        public Shopcategories Shopcategories { get; set; }
        public Hideoutitem Hideoutitem { get; set; }
        public List<Farmableitem> Farmableitem { get; set; }
        public List<Simpleitem> Simpleitem { get; set; }
        public List<Consumableitem> Consumableitem { get; set; }
        public List<Consumablefrominventoryitem> Consumablefrominventoryitem { get; set; }
        public List<Equipmentitem> Equipmentitem { get; set; }
        public List<Weapon> Weapon { get; set; }
        public List<Mount> Mount { get; set; }
        public List<Furnitureitem> Furnitureitem { get; set; }
        public List<Journalitem> Journalitem { get; set; }
        public List<Labourercontract> Labourercontract { get; set; }
        public List<Mountskin> Mountskin { get; set; }
        public List<Crystalleagueitem> Crystalleagueitem { get; set; }
    }

    public class Journalitem
    {
        [JsonProperty("@salvageable")]
        public string Salvageable { get; set; }

        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@tier")]
        public int Tier { get; set; }

        [JsonProperty("@maxfame")]
        public int Maxfame { get; set; }

        [JsonProperty("@baselootamount")]
        public string Baselootamount { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@fasttravelfactor")]
        public string Fasttravelfactor { get; set; }
        [JsonProperty("craftingrequirements")]
        [JsonConverter(typeof(CraftingrequirementsConverter))]
        public List<Craftingrequirements> Craftingrequirements { get; set; }

        public Famefillingmissions Famefillingmissions { get; set; }
        public Lootlist Lootlist { get; set; }


        public string EmptyName { get; set; }
    }

    public class Labourercontract
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@tier")]
        public string Tier { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }
    }

    public class Lootlist
    {
        public object Loot { get; set; }
    }

    public class Mount
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@mountcategory")]
        public string Mountcategory { get; set; }

        [JsonProperty("@maxqualitylevel")]
        public string Maxqualitylevel { get; set; }

        [JsonProperty("@itempower")]
        public string Itempower { get; set; }

        [JsonProperty("@abilitypower")]
        public string Abilitypower { get; set; }

        [JsonProperty("@slottype")]
        public string Slottype { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@mountedbuff")]
        public string Mountedbuff { get; set; }

        [JsonProperty("@halfmountedbuff")]
        public string Halfmountedbuff { get; set; }

        [JsonProperty("@tier")]
        public int Tier { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }

        [JsonProperty("@activespellslots")]
        public string Activespellslots { get; set; }

        [JsonProperty("@passivespellslots")]
        public string Passivespellslots { get; set; }

        [JsonProperty("@durability")]
        public string Durability { get; set; }

        [JsonProperty("@durabilityloss_attack")]
        public string DurabilitylossAttack { get; set; }

        [JsonProperty("@durabilityloss_spelluse")]
        public string DurabilitylossSpelluse { get; set; }

        [JsonProperty("@durabilityloss_receivedattack")]
        public string DurabilitylossReceivedattack { get; set; }

        [JsonProperty("@durabilityloss_receivedspell")]
        public string DurabilitylossReceivedspell { get; set; }

        [JsonProperty("@durabilityloss_receivedattack_mounted")]
        public string DurabilitylossReceivedattackMounted { get; set; }

        [JsonProperty("@durabilityloss_receivedspell_mounted")]
        public string DurabilitylossReceivedspellMounted { get; set; }

        [JsonProperty("@durabilityloss_mounting")]
        public string DurabilitylossMounting { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@unlockedtoequip")]
        public string Unlockedtoequip { get; set; }

        [JsonProperty("@mounttime")]
        public string Mounttime { get; set; }

        [JsonProperty("@dismounttime")]
        public string Dismounttime { get; set; }

        [JsonProperty("@mounthitpointsmax")]
        public string Mounthitpointsmax { get; set; }

        [JsonProperty("@mounthitpointsregeneration")]
        public string Mounthitpointsregeneration { get; set; }

        [JsonProperty("@prefabname")]
        public string Prefabname { get; set; }

        [JsonProperty("@prefabscaling")]
        public string Prefabscaling { get; set; }

        [JsonProperty("@despawneffect")]
        public string Despawneffect { get; set; }

        [JsonProperty("@despawneffectscaling")]
        public string Despawneffectscaling { get; set; }

        [JsonProperty("@remountdistance")]
        public string Remountdistance { get; set; }

        [JsonProperty("@halfmountrange")]
        public string Halfmountrange { get; set; }

        [JsonProperty("@forceddismountcooldown")]
        public string Forceddismountcooldown { get; set; }

        [JsonProperty("@forceddismountspellcooldown")]
        public string Forceddismountspellcooldown { get; set; }

        [JsonProperty("@fulldismountcooldown")]
        public string Fulldismountcooldown { get; set; }

        [JsonProperty("@remounttime")]
        public string Remounttime { get; set; }

        [JsonProperty("@uicraftsoundstart")]
        public string Uicraftsoundstart { get; set; }

        [JsonProperty("@uicraftsoundfinish")]
        public string Uicraftsoundfinish { get; set; }

        [JsonProperty("@dismountbuff")]
        public string Dismountbuff { get; set; }

        [JsonProperty("@forceddismountbuff")]
        public string Forceddismountbuff { get; set; }

        [JsonProperty("@hostiledismountbuff")]
        public string Hostiledismountbuff { get; set; }

        [JsonProperty("@showinmarketplace")]
        public string Showinmarketplace { get; set; }

        [JsonProperty("@hidefromplayer")]
        public string Hidefromplayer { get; set; }
        [JsonProperty("craftingrequirements")]
        [JsonConverter(typeof(CraftingrequirementsConverter))]
        public List<Craftingrequirements> Craftingrequirements { get; set; }

        public Craftingspelllist Craftingspelllist { get; set; }
        public SocketPreset SocketPreset { get; set; }
        public AudioInfo AudioInfo { get; set; }
        public AssetVfxPreset AssetVfxPreset { get; set; }
        public FootStepVfxPreset FootStepVfxPreset { get; set; }

        [JsonProperty("@uisprite")]
        public string Uisprite { get; set; }

        [JsonProperty("@nametagoffset")]
        public string Nametagoffset { get; set; }

        [JsonProperty("@canuseingvg")]
        public string Canuseingvg { get; set; }
        public Mountspelllist Mountspelllist { get; set; }

        [JsonProperty("@enchantmentlevel")]
        public string Enchantmentlevel { get; set; }

        [JsonProperty("@vfxAddonKeyword")]
        public string VfxAddonKeyword { get; set; }

        [JsonProperty("@canuseinfactionwarfare")]
        public string Canuseinfactionwarfare { get; set; }

        [JsonProperty("@hideinmarketplaceuntil")]
        public DateTime? Hideinmarketplaceuntil { get; set; }

        [JsonProperty("@Itemvalue")]
        public double Itemvalue { get; set; }
    }

    public class Mountskin
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@uisprite")]
        public string Uisprite { get; set; }

        [JsonProperty("@prefabname")]
        public string Prefabname { get; set; }

        [JsonProperty("@prefabscaling")]
        public string Prefabscaling { get; set; }

        [JsonProperty("@despawneffect")]
        public string Despawneffect { get; set; }

        [JsonProperty("@despawneffectscaling")]
        public string Despawneffectscaling { get; set; }
        public SocketPreset SocketPreset { get; set; }
        public AudioInfo AudioInfo { get; set; }
        public FootStepVfxPreset FootStepVfxPreset { get; set; }
        public AssetVfxPreset AssetVfxPreset { get; set; }
    }

    public class Mountspelllist
    {
        public object Mountspell { get; set; }
    }

    public class Offspring
    {
        [JsonProperty("@chance")]
        public string Chance { get; set; }

        [JsonProperty("@amount")]
        public string Amount { get; set; }
    }

    public class Repairkit
    {
        [JsonProperty("@repaircostfactor")]
        public string Repaircostfactor { get; set; }

        [JsonProperty("@maxtier")]
        public string Maxtier { get; set; }
    }

    public class AlbionItemDocument
    {
        [JsonProperty("?xml")]
        public Xml Xml { get; set; }
        public Items Items { get; set; }
    }

    public class Seed
    {
        [JsonProperty("@chance")]
        public string Chance { get; set; }

        [JsonProperty("@amount")]
        public string Amount { get; set; }
    }

    public class Shopcategories
    {
        public List<Shopcategory> Shopcategory { get; set; }
    }

    public class Shopcategory
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@value")]
        public string Value { get; set; }


        [JsonProperty("Shopsubcategory")]
        [JsonConverter(typeof(ShopsubcategoriesConverter))]

        public List<ShopSubCategory> Shopsubcategory { get; set; }
    }

    public class Simpleitem
    {

        public Simpleitem()
        {

        }

        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@tier")]
        public int Tier { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }

        [JsonProperty("@maxstacksize")]
        public string Maxstacksize { get; set; }

        [JsonProperty("@uisprite")]
        public string Uisprite { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@Itemvalue")]
        public double Itemvalue { get; set; }

        [JsonProperty("@nutrition")]
        public string Nutrition { get; set; }

        [JsonProperty("@foodcategory")]
        public string Foodcategory { get; set; }

        [JsonProperty("@resourcetype")]
        public string Resourcetype { get; set; }

        [JsonProperty("@famevalue")]
        public string Famevalue { get; set; }

        [JsonProperty("@enchantmentlevel")]
        public int Enchantmentlevel { get; set; }

        [JsonProperty("@fishingfame")]
        public string Fishingfame { get; set; }

        [JsonProperty("@fishingminigamesetting")]
        public string Fishingminigamesetting { get; set; }


        [JsonProperty("craftingrequirements")]
        [JsonConverter(typeof(CraftingrequirementsConverter))]
        public List<Craftingrequirements> Craftingrequirements { get; set; }


        [JsonProperty("@descriptionlocatag")]
        public string Descriptionlocatag { get; set; }

        [JsonProperty("@fasttravelfactor")]
        public string Fasttravelfactor { get; set; }
    }

    public class SocketPreset
    {
        [JsonProperty("@name")]
        public string Name { get; set; }
    }

    public class Upgraderequirements
    {
        public Upgraderesource Upgraderesource { get; set; }
    }

    public class Upgraderesource
    {
        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@count")]
        public string Count { get; set; }
    }

    public class Validitem
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
    }

    public class Weapon
    {

        public Weapon()
        {

        }

        [JsonProperty("@uniquename")]
        public string Uniquename { get; set; }

        [JsonProperty("@mesh")]
        public string Mesh { get; set; }

        [JsonProperty("@uisprite")]
        public string Uisprite { get; set; }

        [JsonProperty("@maxqualitylevel")]
        public string Maxqualitylevel { get; set; }

        [JsonProperty("@abilitypower")]
        public string Abilitypower { get; set; }

        [JsonProperty("@slottype")]
        public string Slottype { get; set; }

        [JsonProperty("@shopcategory")]
        public string Shopcategory { get; set; }

        [JsonProperty("@shopsubcategory1")]
        public string Shopsubcategory1 { get; set; }

        [JsonProperty("@attacktype")]
        public string Attacktype { get; set; }

        [JsonProperty("@attackdamage")]
        public string Attackdamage { get; set; }

        [JsonProperty("@attackspeed")]
        public string Attackspeed { get; set; }

        [JsonProperty("@attackrange")]
        public string Attackrange { get; set; }

        [JsonProperty("@twohanded")]
        public string Twohanded { get; set; }

        [JsonProperty("@tier")]
        public int Tier { get; set; }

        [JsonProperty("@weight")]
        public string Weight { get; set; }

        [JsonProperty("@activespellslots")]
        public string Activespellslots { get; set; }

        [JsonProperty("@passivespellslots")]
        public string Passivespellslots { get; set; }

        [JsonProperty("@durability")]
        public string Durability { get; set; }

        [JsonProperty("@durabilityloss_attack")]
        public string DurabilitylossAttack { get; set; }

        [JsonProperty("@durabilityloss_spelluse")]
        public string DurabilitylossSpelluse { get; set; }

        [JsonProperty("@durabilityloss_receivedattack")]
        public string DurabilitylossReceivedattack { get; set; }

        [JsonProperty("@durabilityloss_receivedspell")]
        public string DurabilitylossReceivedspell { get; set; }

        [JsonProperty("@mainhandanimationtype")]
        public string Mainhandanimationtype { get; set; }

        [JsonProperty("@unlockedtocraft")]
        public string Unlockedtocraft { get; set; }

        [JsonProperty("@unlockedtoequip")]
        public string Unlockedtoequip { get; set; }

        [JsonProperty("@itempower")]
        public string Itempower { get; set; }

        [JsonProperty("@unequipincombat")]
        public string Unequipincombat { get; set; }

        [JsonProperty("@uicraftsoundstart")]
        public string Uicraftsoundstart { get; set; }

        [JsonProperty("@uicraftsoundfinish")]
        public string Uicraftsoundfinish { get; set; }

        [JsonProperty("@canbeovercharged")]
        public string Canbeovercharged { get; set; }
        public Canharvest Canharvest { get; set; }



        [JsonProperty("craftingrequirements")]
        [JsonConverter(typeof(CraftingrequirementsConverter))]
        public List<Craftingrequirements> Craftingrequirements { get; set; }



        public AudioInfo AudioInfo { get; set; }
        public SocketPreset SocketPreset { get; set; }

        [JsonProperty("@craftingcategory")]
        public string Craftingcategory { get; set; }

        [JsonProperty("@descriptionlocatag")]
        public string Descriptionlocatag { get; set; }
        public Craftingspelllist Craftingspelllist { get; set; }

        public Enchantments Enchantments { get; set; }
    }

    public class Xml
    {
        [JsonProperty("@version")]
        public string Version { get; set; }

        [JsonProperty("@encoding")]
        public string Encoding { get; set; }
    }

    public class ShopSubCategory
    {
        [JsonProperty("@id")]
        public string id { get; set; }


        [JsonProperty("@value")]
        public string value { get; set; }
    }




    public class SimplifiedItem
    {
        
      public  string Uniquename { get; set; }



        public string Shopcategory { get; set; }


        public string Shopsubcategory { get; set; }



        public int Tier { get; set; }

        public Enchantments Enchantments { get; set; }


        public List<Craftingrequirements> Craftingrequirements { get; set; }

        public double Itemvalue { get; set; }


    }


    public class SimplifiedItemV2
    {


        public SimplifiedItemV2()
        {
            //albionDataPriceModelsV2.Add(new AlbionDataPriceModel{ City = "Caerleon" });
            //albionDataPriceModelsV2.Add(new AlbionDataPriceModel { City = "Lymhurst" });
            //albionDataPriceModelsV2.Add(new AlbionDataPriceModel { City = "Fort Sterling" });
            //albionDataPriceModelsV2.Add(new AlbionDataPriceModel { City = "Martlock" });
            //albionDataPriceModelsV2.Add(new AlbionDataPriceModel { City = "Thetford" });
            //albionDataPriceModelsV2.Add(new AlbionDataPriceModel { City = "Bridgewatch" });
        }

        public string Uniquename { get; set; }



        public string Shopcategory { get; set; }


        public string Shopsubcategory { get; set; }



        public int Tier { get; set; }
        public int Enchantment { get; set; }

        // Item can still have multiple RECIPES even when enchanted.
        public List<Craftingrequirements> Craftingrequirements { get; set; }

        // Optional for certain items.
        public double Itemvalue { get; set; } = 0;

       // public AlbionDataPriceModel AlbionDataPriceModels { get; set; } = new AlbionDataPriceModel();



        public List<AlbionDataPriceModel> AlbionDataPriceModelsV2 { get; set; } = new List<AlbionDataPriceModel>();






      public  Famefillingmissions Famefillingmissions { get; set; }





    }





    //public class itemConverter : JsonConverter
    //{
    //    public override bool CanWrite => false;
    //    public override bool CanRead => true;
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(Iitem);
    //    }
    //    public override void WriteJson(JsonWriter writer,
    //        object value, JsonSerializer serializer)
    //    {
    //        throw new InvalidOperationException("Use default serialization.");
    //    }

    //    public override object ReadJson(JsonReader reader,
    //        Type objectType, object existingValue,
    //        JsonSerializer serializer)
    //    {
    //        var jsonObject = JObject.Load(reader);
    //        var profession = default(Iitem);



    //        if(jsonObject["shopcategory"].ToString() == "melee" || jsonObject["shopcategory"].ToString() == "ranged" || jsonObject["shopcategory"].ToString() == "magic")
    //        {
    //            // We have a Weapon.
    //            profession = new Weapon();
    //        }
    //        else
    //        {
    //            // We have a 
    //        }






    //        serializer.Populate(jsonObject.CreateReader(), profession);
    //        return profession;
    //    }
    //}



}
