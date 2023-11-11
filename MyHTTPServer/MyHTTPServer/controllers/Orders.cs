using HtmlAgilityPack;
using MyHTTPServer.attributes;
using MyHTTPServer.handlers;
using MyHTTPServer.model;

namespace MyHTTPServer.controllers;

[HttpController("OrdersController")]
public class Orders
{
    [Get("List")]
    public string List() => ToHtmlCode(GetSteamCards(15));

    public List<SteamCard> GetSteamCards(int cardsCount)
    {
        var steamWorkshopPath = @"https://steamcommunity.com/market/";  
        var web = new HtmlWeb();
        var steamCards = new List<SteamCard>();
        int pagesCount = 0;

        try
        {
            while (cardsCount > 0)
            {
                var html = web.Load(steamWorkshopPath + $"search?q=#p{pagesCount++}_popular_desc");
                for (int i = 0; i < Math.Min(10, cardsCount); i++)
                    steamCards.Add(ParseSteamCard(html.GetElementbyId($"result_{i}")));
                cardsCount -= 10;
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }

        return steamCards;
    }

    public SteamCard ParseSteamCard(HtmlNode htmlCard)
    {
        var card = new SteamCard();
        var htmlCardText = htmlCard.GetChildNodeByClass("market_listing_item_name_block");
        var htmlCardTitle = htmlCardText?.GetChildNodeByClass("market_listing_item_name");
        var htmlCardDesc = htmlCardText?.GetChildNodeByClass("market_listing_game_name");
        var htmlCardPrice = htmlCard.GetChildNodeByClass("market_listing_price_listings_block");
        var htmlCardImage = htmlCard.GetChildNodeByClass("market_listing_item_img");

        card.ImagePath = htmlCardImage?.Attributes["src"].Value;
        card.Title = htmlCardTitle?.InnerText;
        card.Description = htmlCardDesc?.InnerText;
        card.Price = htmlCardPrice?.InnerText;
        
        return card;
    }

    public string ToHtmlCode(List<SteamCard> steamCards)
    {
        string htmlCode = "<html><head></html><body style=\"background-color: white;\">";
        foreach (var card in steamCards)
            htmlCode += $"<div style=\"border-color: black;border-width: 2px; display: flex; flex-flow: raw wrap;\">" +
                        $"<img style=\"display: block;\" src={card.ImagePath}>" +
                        $"<span style=\"display: block;\">{card.Title}</span>" +
                        $"<span style=\"display: block;\">{card.Description}</span>" +
                        $"<div style=\"display: flex; flex-flow: raw wrap;\">" +
                        $"<span style=\"display: block;\">{card.Price}</span>" +
                        $"<button>Add</button>" +
                        $"</div></div>";
        
        htmlCode += "</body></html>";
        return htmlCode;
    }
}

public static class HtmlAgilityPackExtensions
{
    public static HtmlNode? GetChildNodeByClass(this HtmlNode parentNode, string nodeClass)
    {
        if (parentNode is null) throw new ArgumentNullException(nameof(parentNode));
        if (nodeClass is null) throw new ArgumentNullException(nameof(nodeClass));

        HtmlNode? childNode = parentNode.ChildNodes
            .Where(elem => elem.GetClasses().Contains(nodeClass, StringComparer.Ordinal)).FirstOrDefault();
        
        if(childNode is null) Console.WriteLine($"Child Node with class {nodeClass} is Not Found");
        return childNode;
    }

}
