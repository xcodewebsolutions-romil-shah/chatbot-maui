using Chatbot.App.Helpers;
using Chatbot.Infrastructure.Common;
using Chatbot.Infrastructure.Entities;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace Chatbot.App.Pages
{
    public partial class ChatPage : ContentPage
    {
        private ClientWebSocket _clientWebSocket = new ClientWebSocket();
        private readonly Uri _serverUri = new Uri(AppSettings.WebSocketServiceLocalURL);
        private MessageEntity messageEntity = new MessageEntity();

        [Obsolete]
        public ChatPage()
        {
            InitializeComponent();
            ConnectToServer();
        }

        #region EVENTS
        /// <summary>
        /// SEND MESSAGE ON BUTTON CLICK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private async void SendMessage_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(edInputMessage.Text))
                {
                    messageEntity = new MessageEntity()
                    {
                        Message = edInputMessage.Text,
                        MessageType = MessageType.Regular,
                    };
                    await SendMessage(messageEntity);
                    edInputMessage.Text = "";
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region METHODS
        /// <summary>
        /// CONNECT TO SERVER
        /// </summary>
        [Obsolete]
        private async void ConnectToServer()
        {
            try
            {
                if (_clientWebSocket.State != WebSocketState.Open)
                {
                    _clientWebSocket = new ClientWebSocket();
                    await _clientWebSocket.ConnectAsync(_serverUri, CancellationToken.None);
                }
                _ = Task.Run(ReceiveMessages);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// RECEIVE MESSAGES
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        private async Task ReceiveMessages()
        {
            try
            {
                var buffer = new byte[1024 * 4];
                while (_clientWebSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;
                    StringBuilder recievedMessage = new StringBuilder();
                    MessageEntity receivedMessageEntity = new MessageEntity();
                    do
                    {
                        result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string jsonMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            receivedMessageEntity = JsonConvert.DeserializeObject<MessageEntity>(jsonMessage) ?? new MessageEntity();
                            if (receivedMessageEntity == null || string.IsNullOrEmpty(receivedMessageEntity.Message))
                            {
                                receivedMessageEntity = new MessageEntity();
                                receivedMessageEntity.Message = "Sorry, I didn't understand your command.";
                                receivedMessageEntity.MessageType = MessageType.Regular;
                            }
                            recievedMessage.Append(receivedMessageEntity.Message);
                        }
                        await Task.Delay(1000);
                    }
                    while (!result.EndOfMessage);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (receivedMessageEntity != null && receivedMessageEntity.MessageType == MessageType.ConfirmEntry)
                        {
                            Button btnRecapture = new Button
                            {
                                Text = "No, Recapture",
                                CornerRadius = 50,
                                HeightRequest = 35,
                                FontSize = 12,
                                BackgroundColor = Colors.White,
                                TextColor = Colors.Black,
                            };
                            btnRecapture.Clicked += async (sender, e) =>
                            {
                                MessageEntity recaptureResponseMessageEntity = new MessageEntity()
                                {
                                    Message = btnRecapture.Text,
                                    MessageType = MessageType.ConfirmEntry,
                                };
                                await SendMessage(recaptureResponseMessageEntity);
                            };

                            Button btnConfirmEntryInfo = new Button
                            {
                                Text = "Yes, Correct",
                                FontFamily = "AvenirBold",
                                CornerRadius = 50,
                                HeightRequest = 35,
                                FontSize = 12,
                                BackgroundColor = new Color(144, 188, 52),
                                TextColor = Colors.White,
                            };
                            btnConfirmEntryInfo.Clicked += async (sender, e) =>
                            {
                                MessageEntity confirmedResponseMessageEntity = new MessageEntity()
                                {
                                    Message = btnConfirmEntryInfo.Text,
                                    MessageType = MessageType.ConfirmEntry,
                                };
                                await SendMessage(confirmedResponseMessageEntity);
                            };

                            chatMessages.Children.Add(new Frame
                            {
                                BorderColor = Colors.Transparent,
                                CornerRadius = 15,
                                BackgroundColor = new Color(240, 236, 236),
                                Padding = new Thickness(10),
                                Margin = new Thickness(10, 3, 50, 3),
                                Content = new VerticalStackLayout
                                {
                                    Spacing = 10,
                                    Children =
                                    {
                                        new Label
                                        {
                                            Text = recievedMessage.ToString(),
                                            TextColor = Colors.Black,
                                            BackgroundColor = Colors.Transparent,
                                            HorizontalTextAlignment = TextAlignment.Start,
                                            VerticalTextAlignment = TextAlignment.Center,
                                        },
                                        new HorizontalStackLayout
                                        {
                                            Spacing = 5,
                                            Children =
                                            {
                                                btnRecapture,
                                                btnConfirmEntryInfo,
                                            }
                                        }
                                    }
                                },
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                            });
                        }
                        else
                        {
                            chatMessages.Children.Add(new Frame
                            {
                                BorderColor = Colors.Transparent,
                                CornerRadius = 15,
                                BackgroundColor = new Color(232, 196, 4),
                                Padding = new Thickness(10),
                                Margin = new Thickness(10, 3, 50, 3),
                                Content = new Label
                                {
                                    Text = recievedMessage.ToString(),
                                    TextColor = Colors.White,
                                    BackgroundColor = Colors.Transparent,
                                    HorizontalTextAlignment = TextAlignment.Start,
                                    VerticalTextAlignment = TextAlignment.Center,
                                },
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                            });
                        }
                        await chatMessagesScrollView.ScrollToAsync(0, chatMessagesScrollView.ContentSize.Height, true);
                    });
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// SEND MESSAGES
        /// </summary>
        /// <param name="responseMessageEntity"></param>
        /// <returns></returns>
        [Obsolete]
        private async Task SendMessage(MessageEntity messageEntity)
        {
            try
            {
                if (_clientWebSocket.State == WebSocketState.Open)
                {
                    string jsonMessage = JsonConvert.SerializeObject(messageEntity);
                    var jsonMessageBuffer = Encoding.UTF8.GetBytes(jsonMessage);
                    await _clientWebSocket.SendAsync(new ArraySegment<byte>(jsonMessageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        chatMessages.Children.Add(new Frame
                        {
                            BorderColor = Colors.Transparent,
                            CornerRadius = 15,
                            BackgroundColor = new Color(80, 76, 76),
                            Padding = new Thickness(10),
                            Margin = new Thickness(50, 3, 10, 3),
                            Content = new Label
                            {
                                Text = messageEntity.Message,
                                TextColor = Colors.White,
                                BackgroundColor = Colors.Transparent,
                                HorizontalTextAlignment = TextAlignment.Start,
                                VerticalTextAlignment = TextAlignment.Center,
                            },
                            HorizontalOptions = LayoutOptions.EndAndExpand,
                        });
                        await chatMessagesScrollView.ScrollToAsync(0, chatMessagesScrollView.ContentSize.Height, true);
                    });
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}