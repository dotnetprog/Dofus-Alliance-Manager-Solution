using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Common.Helper
{
    internal static class FileHelper
    {
        public static async Task<byte[]?> DownloadFile(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return null; 
                }
                var file = await response.Content.ReadAsByteArrayAsync();
                return file;
            }
        }
    }
}
