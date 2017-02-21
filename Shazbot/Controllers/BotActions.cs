using System;
using System.Linq;
using System.Text;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Bot.Connector;

namespace Shazbot
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
    }
}