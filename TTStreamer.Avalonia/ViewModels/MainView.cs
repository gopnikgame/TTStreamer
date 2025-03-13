using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Logging;

using TikTokLiveDotNet;
using TikTokLiveDotNet.Notifications;
using TikTokLiveDotNet.Protobuf;

using TTStreamer.Models;
using TTStreamer.Services;

namespace TTStreamer.Models
{
    [ObservableObject]
    public partial class MainView
    {
        public ObservableCollection<GiftView> Gifts { get; set; } = new ObservableCollection<GiftView>();
        public ObservableCollection<string> VoiceList { get; set; } = new ObservableCollection<string>();
        public string UserId { get; set; }
        public bool AutoScroll { get; set; }
        public bool SpeechGift { get; set; }
        public bool SpeechLike { get; set; }
        public bool SpeechMember { get; set; }
        public string SpeechVoice { get; set; }
        public string SpeechRate { get; set; }
        public bool Notification { get; set; }

        [ObservableProperty]
        bool connectEnabled = true;

        [ObservableProperty]
        bool disconnectEnabled;

        private TikTokLiveClient tikTokLiveClient;
        private readonly SpeechService speechService;
        private readonly SoundService soundService;
        private readonly GiftService giftService;
        private readonly ILogger<MainView> logger;

        public MainView(
            SpeechService speechService,
            SoundService soundService,
            GiftService giftService,
            ILogger<MainView> logger)
        {
            this.speechService = speechService;
            this.soundService = soundService;
            this.giftService = giftService;
            this.logger = logger;

            LoadState();

        }

        [RelayCommand]
        public async Task Connect()
        {
            try
            {
                tikTokLiveClient = new TikTokLiveClient(UserId);
                tikTokLiveClient.GiftMessageReceived.Subscribe(GiftUpdate);
                tikTokLiveClient.LikeMessageReceived.Subscribe(LikeUpdate);
                tikTokLiveClient.MemberMessageReceived.Subscribe(MemberUpdate);
                tikTokLiveClient.DisconnectionHappened.Subscribe(OnDisconnected);
                await tikTokLiveClient.Connect();

                if (tikTokLiveClient.ClientState.IsConnecting)
                {
                    tikTokLiveClient.Disconnect();
                    return;
                }

                if(tikTokLiveClient.ClientState.IsConnected)
                {
                    ConnectEnabled = false;
                    DisconnectEnabled = true;
                    return;
                }

                ConnectEnabled = true;
                DisconnectEnabled = false;
            }
            catch (Exception e)
            {
                ConnectEnabled = true;
                logger.LogError(e, e.Message);
                MessageBox.Show(e.Message);
            }
        }

        [RelayCommand]
        public void Disconnect()
        {
            tikTokLiveClient.Disconnect();
        }

        private void OnDisconnected(DisconnectionInfo disconnectionInfo)
        {
            ConnectEnabled = true;
            DisconnectEnabled = false;
            if (disconnectionInfo.IsFailure) MessageBox.Show(disconnectionInfo.FailureCause);
        }

        private async void MemberUpdate(WebcastMemberMessage msg)
        {
            if (int.TryParse(SpeechRate, out var rate) && SpeechMember) await speechService.Speech($"{msg.User.Nickname} подключился к стримчику", SpeechVoice, rate);
        }

        private async void LikeUpdate(WebcastLikeMessage msg)
        {
            if (int.TryParse(SpeechRate, out var rate) && SpeechLike) await speechService.Speech($"{msg.User.Nickname} старательно лайкает", SpeechVoice, rate);
        }


        private async void GiftUpdate(WebcastGiftMessage msg)
        {
            try
            {
                var giftData = await giftService.Create(msg.giftId, msg.giftDetails.giftName, msg.giftDetails.giftImage.giftPictureUrl);
                var giftView = Gifts.LastOrDefault(g => g != null && g.Sender.Equals(msg.User.uniqueId) && g.GiftId == msg.giftId && !g.RepeatEnded);
                var text = $"{msg.User.Nickname} прислал {msg.giftDetails.giftName}";

                if (msg.giftDetails.giftType == 1 && msg.repeatEnd == 1)
                {
                    if (giftView != null)
                    {
                        giftView.RepeatEnded = true;
                        giftView.RepeatCount = msg.repeatCount;
                        return;
                    }
                    else
                    {
                        Gifts.Add(new GiftView()
                        {
                            Image = Convert.FromBase64String(giftData.Image), 
                            GiftName = msg.giftDetails.giftName, 
                            Sender = msg.User.uniqueId,
                            GiftId = msg.giftId,
                            RepeatCount = msg.repeatCount, RepeatEnded = msg.repeatEnd == 1
                        });
                    }
                }

                if (msg.giftDetails.giftType == 1 && msg.repeatEnd == 0 && giftView != null)
                {
                    giftView.RepeatCount = msg.repeatCount;
                }
                else
                {
                    Gifts.Add(new GiftView()
                    {
                        Image = Convert.FromBase64String(giftData.Image),
                        GiftName = msg.giftDetails.giftName, 
                        Sender = msg.User.uniqueId,
                        GiftId = msg.giftId,
                        RepeatCount = msg.repeatCount, RepeatEnded = msg.repeatEnd == 1
                    });
                }

                if (Notification) soundService.Play(msg.giftId);
                if (SpeechGift) speechService.Speech(text, SpeechVoice, int.Parse(SpeechRate));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadState()
        {
            try
            {
                UserId = Settings.Default.UserId;
                AutoScroll = Settings.Default.AutoScroll;
                SpeechGift = Settings.Default.SpeechGift;
                SpeechLike = Settings.Default.SpeechLike;
                Notification = Settings.Default.Notify;
                SpeechVoice = Settings.Default.SpeechVoice;
                SpeechRate = string.IsNullOrEmpty(Settings.Default.SpeechRate) ? "4" : Settings.Default.SpeechRate;
                SpeechMember = Settings.Default.SpeechMember;
                VoiceList = new ObservableCollection<string>(speechService.VoiceList());
                if (string.IsNullOrEmpty(SpeechVoice)) SpeechVoice = VoiceList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        public void SaveState()
        {
            Settings.Default.UserId = UserId;
            Settings.Default.AutoScroll = AutoScroll;
            Settings.Default.SpeechGift = SpeechGift;
            Settings.Default.SpeechLike = SpeechLike;
            Settings.Default.SpeechMember = SpeechMember;
            Settings.Default.Notify = Notification;
            Settings.Default.SpeechVoice = SpeechVoice;
            Settings.Default.SpeechRate = SpeechRate;
            Settings.Default.Save();
        }

    }
}