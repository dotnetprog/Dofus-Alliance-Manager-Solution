using AnkamaWebClient.Abstractions;
using DAM.Domain.JsonData;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkamaWebClient.Client
{
    public class AnkamaWebService : IAnkamaService
    {
        const string ANKAMA_URL = "https://account.ankama.com/fr/profil-ankama/";
        public async Task<bool> ValidatePseudo(string pseudo)
        {
            if (string.IsNullOrWhiteSpace(pseudo)){
                return false;
            }

            var tokens = pseudo.Split('#');
            if(tokens.Length != 2)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(tokens[0]) || string.IsNullOrWhiteSpace(tokens[1]))
            {
                return false;
            }

            if (!int.TryParse(tokens[1],out int result))
            {
                return false;
            }
            return true;

        }


        public async Task<IReadOnlyCollection<AnkamaPseudoData>> SearchAnkadex(string pseudo)
        {
            var isValid = await ValidatePseudo(pseudo);
            if (!isValid)
            {
                return null;
            }

            var parsedPseudo = await ParsePseudo(pseudo);


            var rowsXpath = @"//div[@id=""tab1""]/div[@class=""ak-block-persos""]//table/tbody/tr";
            var htmlnode  = new HtmlWeb();
            var htmlDocument = await htmlnode.LoadFromWebAsync(ANKAMA_URL + parsedPseudo);
            if (htmlDocument == null)
                return null;

            var rows = htmlDocument.DocumentNode.SelectNodes(rowsXpath);
            if (rows == null)
            {
                rowsXpath = @"//div[@id=""tab1""]/div[@class=""ak-block-persos""]/div[@class=""ak-responsivetable-wrapper""]/table/tbody/tr";
                rows = htmlDocument.DocumentNode.SelectNodes(rowsXpath);
                if (rows == null)
                {
                    return null;
                }
            }
                
            var personnages = rows.Select(ParseTr).ToList();
            
            return personnages;
            

        }
        private AnkamaPseudoData ParseTr(HtmlNode node)
        {
            var persoUrl = node.SelectSingleNode("td[1]/a")?.Attributes["href"]?.Value;
            var persoName = node.SelectSingleNode("td[1]/a")?.InnerText;
            var Serveur = node.SelectSingleNode("td[4]")?.InnerText;
            var Guild = node.SelectSingleNode("td[5]/a")?.InnerText;
            if (string.IsNullOrEmpty(persoName))
            {
                return null;
            }
            var pseudo = new AnkamaPseudoData()
            {
                Guild = Guild,
                NomPersonnage = persoName,
                PageUrl = persoUrl,
                Serveur = Serveur
            };
            return pseudo;
        }


        public async Task<string> ParsePseudo(string pseudo)
        {
            return pseudo.Replace("#", "-");
        }
    }
}
