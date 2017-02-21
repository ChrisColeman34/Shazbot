using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using Geocoding;
using Geocoding.Google;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Shazbot.Amazon;
using Shazbot.AmazonService;
using Shazbot.Custom_API_Models;

namespace Shazbot.BotLogic
{
    public class BotActions
    {
        public static Activity YouTubeSearch(Activity activity, string searchTerm)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyACw0k4PBVHN7aM-oLIPJ7MypJweQcyHns",
                ApplicationName = "Shazbot"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = searchTerm;
            searchListRequest.MaxResults = 5;
            var searchListResponse = searchListRequest.Execute();
            var results = searchListResponse.Items.Where(searchResult => searchResult.Id.Kind == "youtube#video");
            // Just get our first result for now

            var sb = new StringBuilder();
            sb.AppendLine($"Hello {activity.From.Name} - Top 5 Search results for {searchTerm}." + Environment.NewLine);

            foreach (var result in results)
            {
                var url = $"https://www.youtube.com/watch?v={result.Id.VideoId}";
                var title = result.Snippet.Title;
                sb.AppendLine($"{title} : {url}" + Environment.NewLine);
            }

            var reply = activity.CreateReply(sb.ToString());
            reply.Type = "message";
            return reply;
        }

        public static Activity AmazonSearch(Activity activity, string searchTerm)
        {
            var accessKeyId = "AKIAIOXGBEXN4SPR5PYQ";
            var secretKey = "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";

            //var client = new AWSECommerceServicePortTypeClient();
            //client.ChannelFactory.Endpoint.Behaviors.Add(new Amazon.AmazonSigningEndpointBehavior(accessKeyId, secretKey));

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
            {
                MaxReceivedMessageSize = int.MaxValue
            };

            var client = new AWSECommerceServicePortTypeClient(binding, new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService"));

            // add authentication to the ECS client
            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(accessKeyId, secretKey));

            var lookup = new ItemSearch();
            var request = new ItemSearchRequest();
            lookup.AssociateTag = accessKeyId;
            lookup.AWSAccessKeyId = secretKey;
            request.Keywords = searchTerm;
            var response = client.ItemSearch(lookup);


            var sb = new StringBuilder();
            sb.AppendLine($"Hello {activity.From.Name} - Amazon Search results for {searchTerm}." + Environment.NewLine);

            var reply = activity.CreateReply(sb.ToString());
            reply.Type = "message";
            return reply;
        }

        public static Activity ForcastSearch(Activity activity, string searchTerm)
        {
            var apiKey = "174629f16ab0dcaf1e63d4853cb66830";
            var geocoder = new GoogleGeocoder();
            var addresses = geocoder.Geocode(searchTerm);

            double lat = addresses.First().Coordinates.Latitude;
            double longi = addresses.First().Coordinates.Longitude;

            var apiUrl = $"https://api.darksky.net/forecast/{apiKey}/{lat},{longi}";

            var client = new HttpClient
            {
                BaseAddress = new Uri("https://api.darksky.net/forecast/")
            };
            
            var resp = client.GetAsync(apiUrl).Result;
            string json = resp.Content.ReadAsStringAsync().Result;

            var forecast = JsonConvert.DeserializeObject<ForecastAPI.Forecast>(json);

            var sb = new StringBuilder();
            var reply = activity.CreateReply(sb.ToString());
            reply.Type = "message";
            return reply;
        }

    }
}