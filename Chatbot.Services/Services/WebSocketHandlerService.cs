using Chatbot.Infrastructure.Common;
using Chatbot.Infrastructure.Entities;
using Chatbot.Services.Interfaces;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Chatbot.Services.Services
{
    public class WebSocketHandlerService() : IWebSocketHandlerService
    {
        /// <summary>
        /// HANDLE WEBSOCKET
        /// </summary>
        /// <param name="webSockets"></param>
        /// <param name="webSocket"></param>
        /// <param name="socketId"></param>
        /// <param name="requestHeaders"></param>
        /// <returns></returns>
        public async Task HandleWebSocket(ConcurrentDictionary<string, WebSocket> webSockets, WebSocket webSocket, string socketId, Dictionary<string, string> requestHeaders)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (result.MessageType != WebSocketMessageType.Close)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                foreach (var socket in webSockets)
                {
                    if (socket.Key == socketId)
                    {
                        if (socket.Value.State == WebSocketState.Open)
                        {
                            MessageEntity messageEntity = await GetResponseMessage(message, requestHeaders);
                            if (messageEntity == null || string.IsNullOrEmpty(messageEntity.Message))
                            {
                                messageEntity = new MessageEntity();
                                messageEntity.Message = "Sorry, I didn't understand your command.";
                                messageEntity.MessageType = MessageType.Regular;
                            }
                            else
                            {
                                if (messageEntity.MessageType == MessageType.ConfirmEntry)
                                {
                                    #region SEND GREETING MESSAGE
                                    MessageEntity greetingResponseMessageEntity = new MessageEntity()
                                    {
                                        MessageType = MessageType.Regular,
                                        Message = "Thanks, lets confirm these entries…",
                                    };
                                    string greetingResponseMessage = JsonConvert.SerializeObject(greetingResponseMessageEntity);
                                    await socket.Value.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(greetingResponseMessage)), WebSocketMessageType.Text, true, CancellationToken.None);
                                    #endregion

                                    #region SEND CAPTURED ENTRIES
                                    MessageEntity capturedEntryResponseMessageEntity = new MessageEntity()
                                    {
                                        MessageType = MessageType.Regular,
                                        Message = messageEntity.Message,
                                    };
                                    string capturedEntryResponseMessage = JsonConvert.SerializeObject(capturedEntryResponseMessageEntity);
                                    await socket.Value.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(capturedEntryResponseMessage)), WebSocketMessageType.Text, true, CancellationToken.None);
                                    #endregion

                                    #region SEND CONFIRMATION MESSAGE
                                    MessageEntity confirmationResponseMessageEntity = new MessageEntity()
                                    {
                                        MessageType = MessageType.ConfirmEntry,
                                        Message = "Are these entries correct?",
                                    };
                                    string confirmationResponseMessage = JsonConvert.SerializeObject(confirmationResponseMessageEntity);
                                    await socket.Value.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(confirmationResponseMessage)), WebSocketMessageType.Text, true, CancellationToken.None);
                                    #endregion
                                }
                                else
                                {
                                    string responseMessage = JsonConvert.SerializeObject(messageEntity);
                                    await socket.Value.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(responseMessage)), WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                            }
                        }
                    }
                }
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            webSockets.TryRemove(socketId, out _);
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the server", CancellationToken.None);
        }

        /// <summary>
        /// GET RESPONSE MESSAGE
        /// </summary>
        /// <param name="jsonMessage"></param>
        /// <param name="requestHeaders"></param>
        /// <returns></returns>
        private async Task<MessageEntity> GetResponseMessage(string jsonMessage, Dictionary<string, string> requestHeaders)
        {
            MessageEntity responseMessageEntity = new MessageEntity();
            MessageEntity receivedMessageEntity = JsonConvert.DeserializeObject<MessageEntity>(jsonMessage) ?? new MessageEntity();
            string inputMessage = receivedMessageEntity.Message;
            try
            {
                List<string> greetingMessages = new List<string>()
                {
                    "How are you"
                    ,"Hey there"
                    ,"Hi"
                    ,"Hello"
                    ,"Good Morning"
                    ,"Good Afternoon"
                    ,"Good Evening"
                    ,"Good Day"
                    ,"Greetings"
                };
                List<string> questionMessages = new List<string>()
                {
                    "What is your name",
                };
                List<string> endingMessages = new List<string>()
                {
                    "Take care"
                    ,"See you later"
                    ,"Catch you later"
                    ,"Adios"
                    ,"Farewell"
                    ,"Good night"
                    ,"Later"
                    ,"Bye for now"
                    ,"Until next time"
                    ,"Peace out"
                    ,"Good Night"
                    ,"Bye"
                    ,"Bye Bye"
                    ,"See you soon"
                };
                if (greetingMessages.Select(m => m.ToLower()).Contains(Convert.ToString(inputMessage).ToLower()))
                {
                    responseMessageEntity.Message = $"{inputMessage}, How can I help you today?";
                    responseMessageEntity.MessageType = MessageType.Regular;
                }
                else if (questionMessages.Select(m => m.ToLower()).Contains(Convert.ToString(inputMessage).ToLower()))
                {
                    responseMessageEntity.Message = "Hi, I am Chatbot. I am your assistant. Tell me, How can I help you?";
                    responseMessageEntity.MessageType = MessageType.Regular;
                }
                else if (endingMessages.Select(m => m.ToLower()).Contains(Convert.ToString(inputMessage).ToLower()))
                {
                    responseMessageEntity.Message = "Have a good day. See you soon here.";
                    responseMessageEntity.MessageType = MessageType.Regular;
                }
                else if (receivedMessageEntity.MessageType == MessageType.ConfirmEntry)
                {
                    if (!string.IsNullOrEmpty(receivedMessageEntity.Message) && receivedMessageEntity.Message.ToLower().Contains("yes"))
                    {
                        responseMessageEntity.Message = "Thank you for your response, The entry has been submitted!!!";
                    }
                    else
                    {
                        responseMessageEntity.Message = "No worries. Tell me about matter in detail again.";
                    }
                    responseMessageEntity.MessageType = MessageType.Regular;
                }
                else
                {
                    string entryPattern = @"Was in court for the (.+?) case, (\d{1,2}-\d{1,2}), On (\d{1,2} \w+ \d{4})";
                    Match matchEntry = Regex.Match(inputMessage, entryPattern);
                    if (matchEntry.Success)
                    {
                        string clientName = matchEntry.Groups[1].Value;
                        string entryDate = matchEntry.Groups[3].Value;
                        string entryTime = matchEntry.Groups[2].Value;

                        string extractedMessage = $"Client Name:\t{clientName}\n";
                        extractedMessage += $"Matter Name:\tRAF//{clientName.ToUpper()}012\n";
                        extractedMessage += $"Entry Type:\tCourt Appearance\n";
                        extractedMessage += $"Date:\t{entryDate}\n";
                        extractedMessage += $"Time:\t{entryTime}";

                        responseMessageEntity.Message = extractedMessage;
                        responseMessageEntity.MessageType = MessageType.ConfirmEntry;
                    }
                    else
                    {
                        responseMessageEntity.Message = "Sorry, I didn't understand your command.";
                        responseMessageEntity.MessageType = MessageType.Regular;
                    }
                }
            }
            catch
            {
                responseMessageEntity.Message = "Technical exception occurred. Please try again later.";
                responseMessageEntity.MessageType = MessageType.Regular;
            }
            return responseMessageEntity;
        }
    }
}