using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionOnlineCraftingCalculator.Models
{
    
    public class Achievement
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@category")]
        public string Category { get; set; }

        [JsonProperty("@ring")]
        public string Ring { get; set; }

        [JsonProperty("@isfameachievement")]
        public string Isfameachievement { get; set; }

        [JsonProperty("@textureReward")]
        public string TextureReward { get; set; }

        [JsonProperty("title")]
        public Title Title { get; set; }

        [JsonProperty("masterylevels")]
        public Masterylevels Masterylevels { get; set; }

        [JsonProperty("parentachievements")]
        public Parentachievements Parentachievements { get; set; }

        [JsonProperty("@tracking")]
        public string Tracking { get; set; }

        [JsonProperty("@itemforsprite")]
        public string Itemforsprite { get; set; }

        [JsonProperty("@spriteReward")]
        public string SpriteReward { get; set; }
    }

    public class Achievements
    {
        [JsonProperty("@Version")]
        public string Version { get; set; }

        [JsonProperty("@xsi:noNamespaceSchemaLocation")]
        public string XsiNoNamespaceSchemaLocation { get; set; }

        [JsonProperty("@xmlns:xsi")]
        public string XmlnsXsi { get; set; }

        [JsonProperty("template")]
        public List<Template> Template { get; set; }

        [JsonProperty("achievement")]
        public List<Achievement> Achievement { get; set; }

        [JsonProperty("templateachievement")]
        public List<Templateachievement> Templateachievement { get; set; }
    }

    public class Baselevels
    {
        [JsonProperty("@structure")]
        public string Structure { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class Baserewards
    {
        [JsonProperty("unlock")]
        [JsonConverter(typeof(UnlockConverter))]

        public List<Unlock> Unlock { get; set; }

        [JsonProperty("bonus")]
        public object Bonus { get; set; }
    }

    public class Elitelevels
    {
        [JsonProperty("@structure")]
        public string Structure { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public class Eliterewards
    {
        [JsonProperty("bonus")]
        public object Bonus { get; set; }
    }

    public class Itemlist
    {
        [JsonProperty("itempattern")]
        [JsonConverter(typeof(PatternConverter))]

        public List<Pattern> Itempattern { get; set; }
    }

    public class Pattern
    {


        [JsonProperty("@pattern")]
        public string pattern { get; set; } = "";
    }
    public class PatternConverter : JsonConverter
    {

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {


            if (reader.TokenType == JsonToken.StartArray)
            {
                Debug.WriteLine("FoundArray");
                // Multi Object
                var a = JArray.Load(reader);
                var l = new List<Pattern>();

                foreach (var item in a)
                {
                    l.Add(item.ToObject<Pattern>());
                }

                return l;


            }
            
            if(reader.TokenType == JsonToken.StartObject)
            {
                Debug.WriteLine("FoundObject");
                // Single Object
                var o = JObject.Load(reader);
                var l = new List<Pattern>
                {
                    o.ToObject<Pattern>()
                };
                return l;
            }

            return new List<Pattern>();
        }

        public override bool CanConvert(Type objectType) => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
    public class UnlockConverter : JsonConverter
    {

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {


            if (reader.TokenType == JsonToken.StartArray)
            {
                Debug.WriteLine("FoundArray");
                // Multi Object
                var a = JArray.Load(reader);
                var l = new List<Unlock>();

                foreach (var item in a)
                {
                    l.Add(item.ToObject<Unlock>());
                }

                return l;


            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                Debug.WriteLine("FoundObject");
                // Single Object
                var o = JObject.Load(reader);
                var l = new List<Unlock>
                {
                    o.ToObject<Unlock>()
                };
                return l;
            }

            return new List<Pattern>();
        }

        public override bool CanConvert(Type objectType) => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }



    public class Masterylevel
    {
        [JsonProperty("missions")]
        public Missions Missions { get; set; }

        [JsonProperty("rewardgroups")]
        public Rewardgroups Rewardgroups { get; set; }

        [JsonProperty("@learningpoints")]
        public string Learningpoints { get; set; }

        [JsonProperty("@progresstospendlp")]
        public string Progresstospendlp { get; set; }
    }

    public class Masterylevels
    {
        [JsonProperty("masterylevel")]
        public Masterylevel Masterylevel { get; set; }
    }

    public class Mission
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("@value")]
        public string Value { get; set; }

        [JsonProperty("@mintier")]
        public string Mintier { get; set; }

        [JsonProperty("equipitem")]
        public object Equipitem { get; set; }

        [JsonProperty("validitem")]
        public List<Validitem> Validitem { get; set; }
    }

    public class Missions
    {
        [JsonProperty("mission")]
        public Mission Mission { get; set; }
    }

    public class Parentachievements
    {
        [JsonProperty("@allrequired")]
        public string Allrequired { get; set; }

        [JsonProperty("achievement")]
        [JsonConverter(typeof(AchievementConverter))]
        public List<AchievementInParent> Achievement { get; set; }
    }

    public class AchievementInParent
    {
        [JsonProperty("@id")]

        public string Id { get; set; }
    }


    public class AchievementConverter : JsonConverter
    {

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {


            if (reader.TokenType == JsonToken.StartArray)
            {
                Debug.WriteLine("FoundArray");
                // Multi Object
                var a = JArray.Load(reader);
                var l = new List<AchievementInParent>();

                foreach (var item in a)
                {
                    l.Add(item.ToObject<AchievementInParent>());
                }

                return l;


            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                Debug.WriteLine("FoundObject");
                // Single Object
                var o = JObject.Load(reader);
                var l = new List<AchievementInParent>
                {
                    o.ToObject<AchievementInParent>()
                };
                return l;
            }

            return new List<AchievementInParent>();
        }

        public override bool CanConvert(Type objectType) => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

    

    public class Rewardgroups
    {
        [JsonProperty("rewardgroup")]
        public object Rewardgroup { get; set; }
    }

    public class Root
    {
        [JsonProperty("?xml")]
        public Xml Xml { get; set; }

        [JsonProperty("achievements")]
        public Achievements Achievements { get; set; }
    }

    public class Template
    {
        [JsonProperty("@name")]
        public string Name { get; set; }

        [JsonProperty("baselevels")]
        public Baselevels Baselevels { get; set; }

        [JsonProperty("elitelevels")]
        public Elitelevels Elitelevels { get; set; }
    }

    public class Templateachievement
    {
        [JsonProperty("@usetemplate")]
        public string Usetemplate { get; set; }

        [JsonProperty("@missiontype")]
        public string Missiontype { get; set; }

        [JsonProperty("@lpmultiplier")]
        public string Lpmultiplier { get; set; }

        [JsonProperty("@famemultiplier")]
        public string Famemultiplier { get; set; }

        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@re-spectype")]
        public string ReSpectype { get; set; }

        [JsonProperty("@category")]
        public string Category { get; set; }

        [JsonProperty("@ring")]
        public string Ring { get; set; }

        [JsonProperty("@spriteReward")]
        public string SpriteReward { get; set; }

        [JsonProperty("@itemforsprite")]
        public string Itemforsprite { get; set; }

        [JsonProperty("@isfameachievement")]
        public string Isfameachievement { get; set; }

        [JsonProperty("title")]
        public Title Title { get; set; }

        [JsonProperty("itemlist")]
        public Itemlist Itemlist { get; set; }

        [JsonProperty("baserewards")]
        public Baserewards Baserewards { get; set; }

        [JsonProperty("eliterewards")]
        public Eliterewards Eliterewards { get; set; }

        [JsonProperty("parentachievements")]
        public Parentachievements Parentachievements { get; set; }

        [JsonProperty("@respecmodifier")]
        public string Respecmodifier { get; set; }
    }

    public class Title
    {
        [JsonProperty("@tag")]
        public string Tag { get; set; }
    }

    public class Unlock
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("itempattern")]
        [JsonConverter(typeof(PatternConverter))]

        public List<Pattern> Itempattern { get; set; }
    }

    public class Validitem
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
    }

    public class Xml
    {
        [JsonProperty("@version")]
        public string Version { get; set; }

        [JsonProperty("@encoding")]
        public string Encoding { get; set; }
    }
}
