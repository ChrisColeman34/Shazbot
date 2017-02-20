using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Shazbot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var reply = PresentNextStep(activity);

                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity PresentNextStep(Activity activity)
        {
            return DataAccessReply(activity);
        }


        private static Activity DataAccessReply(Activity activity)
        {
            var reply = activity.CreateReply(string.Format("Hello {0}", activity.From.Name));
            reply.Type = "message";
            reply.Attachments = new List<Attachment>();

            var cardImages = new List<CardImage>();

            var cardButtons = new List<CardAction>
            {
                new CardAction
                {
                    Type = "postBack",
                    Title = "Yes",
                    Value = "Q0-1",
                },
                new CardAction
                {
                    Type = "postBack",
                    Title = "No",
                    Value = "Q0-0",
                }
            };

            var plCard = new ThumbnailCard()
            {
                Title = "Cheese...",
                Subtitle = "Hello World?",
                Images = cardImages,
                Buttons = cardButtons
            };

            var plAttachment = plCard.ToAttachment();
            reply.Attachments.Add(plAttachment);

            return reply;
        }


        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}