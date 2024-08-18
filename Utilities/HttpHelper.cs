using System;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Microsoft.Playwright;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Html;

namespace tuzi_tsuki.Utilities
{
    public class HttpHelper
    {


        public async Task<String> getcontextAsync()
        {
            string url = "https://y.qq.com/musicmac/v6/playlist/detail.html?id=9224229586"; // Replace with your target URL
            string text = "";
            var exitcode = Microsoft.Playwright.Program.Main(new[] { "install" });
            if(exitcode != 0)
            {
                throw new Exception($"p{exitcode}");
            }
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(url);

            // Wait for a specific element that indicates the page has loaded
            await page.WaitForSelectorAsync(".songlist__li", new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached });

            // Alternatively, wait for network to be idle
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var content = await page.ContentAsync();
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(content);
            var songs = ExtractSongs(document);
            int k = 0;
            foreach (var song in songs)
            {
                Console.WriteLine($"歌曲名: {song.SongName}, 歌手名: {song.Artist}, 专辑名: {song.Album}, 时间: {song.Duration}");
                text += $"歌曲名: {song.SongName}, 歌手名: {song.Artist}, 专辑名: {song.Album}, 时间: {song.Duration}\n";
                k++;
            }
            
            return text;
        }
        public class SongInfo
        {
            public string SongName { get; set; }
            public string Artist { get; set; }
            public string Album { get; set; }
            public string Duration { get; set; }
        }
        public static List<SongInfo> ExtractSongs(HtmlDocument document)
        {
            List<SongInfo> songs = new List<SongInfo>();
            var songNodes = document.DocumentNode.SelectNodes("//li[contains(@class, 'songlist__li')]");

            if (songNodes != null)
            {
                foreach (var node in songNodes)
                {
                    var song = new SongInfo
                    {
                        SongName = node.SelectSingleNode(".//a[contains(@class, 'mod_songname__name')]")?.InnerText,
                        Artist = node.SelectSingleNode(".//a[contains(@class, 'singer_name')]")?.InnerText,
                        Album = node.SelectSingleNode(".//a[contains(@class, 'album_name')]")?.InnerText,
                        Duration = node.SelectSingleNode(".//span[contains(@class, 'songlist__time')]")?.InnerText
                    };
                    songs.Add(song);
                }
            }
            else
            {
                Console.WriteLine("No songs found.");
            }

            return songs;
        }
    }
}
