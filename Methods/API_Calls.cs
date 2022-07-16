using AlbionOnlineCraftingCalculator.Models;
using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using RestSharp;
using System;
using System.Diagnostics;

namespace AlbionOnlineCraftingCalculator
{
    public class LicenseManager
    {
        private string _Token = String.Empty;
        //private string _Username = "";
        //private string _Password = "";

        public LicenseManager(string token)
        {

            //string token = GetToken();
            //if (!(token == String.Empty))
            //{
            //    // We have a JWT token.
            //    Debug.WriteLine(token);


            //    DateTime dateTime = JWTService.GetExpiryTimestamp(token);

            //    Debug.WriteLine(dateTime.ToString());

            //}
            _Token = token;
            

        }


        public bool ValidLicense()
        {

            DateTime expirationDate = JWTService.GetExpiryTimestamp(_Token);
            DateTime now = DateTime.Now;



            //TimeSpan timeSpan = TimeSpan.
            if ((expirationDate - now).TotalSeconds > 0 && expirationDate != DateTime.MinValue) // If we still have some seconds left on the Token.   // Could be days aswell, not sure...
            {
                // License is still valid.
                Debug.WriteLine("Found Valid License");
                return true;
            }
            else
            {
                // License is no longer valid.
                Debug.WriteLine("Found Invalid License");

                return false;
            }

        }





        public static string GetToken(string userName, string passWord)
        {

            

            string apiUrl = @"https://aoc-api.klumponline.nl";


            var registerModel = new RegisterModel();
            registerModel.Username = userName;
            registerModel.Password = passWord;

            var client = new RestClient(apiUrl);

            RestRequest request = new RestRequest("/api/Auth/login", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(registerModel);

            RestResponse response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var tokenString = response.Content;
                
                Debug.WriteLine(tokenString);
                return tokenString;
            }
            else
            {
                Debug.WriteLine("Code:" + response.StatusCode);
                return "error";
            }





        }



    }


    public static class JWTService
    {
        private static IJsonSerializer _serializer = new JsonNetSerializer();
        private static IDateTimeProvider _provider = new UtcDateTimeProvider();
        private static IBase64UrlEncoder _urlEncoder = new JwtBase64UrlEncoder();
        private static IJwtAlgorithm _algorithm = new HMACSHA256Algorithm();

        public static DateTime GetExpiryTimestamp(string accessToken)
        {

            Debug.WriteLine("Checking for: " + accessToken);

            try
            {
                IJwtValidator _validator = new JwtValidator(_serializer, _provider);
                IJwtDecoder decoder = new JwtDecoder(_serializer, _validator, _urlEncoder, _algorithm);
                var token = decoder.DecodeToObject<JwtToken>(accessToken);
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(token.exp);
                return dateTimeOffset.LocalDateTime;
            }
            catch (TokenExpiredException)
            {
                return DateTime.MinValue;
            }
            catch (SignatureVerificationException)
            {
                return DateTime.MinValue;
            }
            catch (Exception ex)
            {
                // ... remember to handle the generic exception ...
                if(ex.Message == "") { }
                return DateTime.MinValue;
            }
        }
    }


    public class JwtToken
    {
        public long exp { get; set; }
    }

}
