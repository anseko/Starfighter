using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Net
{
    public class DiscordApp
    {
        private readonly HttpClient _discordClient;

        public DiscordApp()
        {
            // Get OAuth2 Token
            // create HttpClient
            // GetChannels, define channels with cadets commands
            // Send test hello message.

            _discordClient = new HttpClient()
            {
                BaseAddress = new Uri("https://discord.com/api")
            };
            
            _discordClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot",
                "");
        }
        
        //Sub on events with stress, hp and etc
    }
}