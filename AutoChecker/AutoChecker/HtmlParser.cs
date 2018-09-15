using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace AutoChecker
{
    class HtmlParser
    {
        private const string LI_TAG = "li";
        private const string P_TAG = "p";
        private const string SPAN_TAG = "span";

        private const string ROOMS_LI_XPATH = "//html/body" +
                "/div[@class='t_myarea t_mainbox clearfix mt15 t_zindex0']" +
                "/div[@class='t_newlistbox']/ul[@id='houseList']";
        private const string ROOM_IMG_XPATH = "div[@class='img pr']/a/img";
        private const string ROOM_NAME_XPATH = "div[@class='txt']/h3/a";
        private const string ROOM_LOCATION_XPATH = "div[@class='txt']/h4/a";
        private const string ROOM_DETAIL_XPATH = "div[@class='txt']/div[@class='detail']";
        private const string ROOM_STYLE_XPATH = "div[@class='txt']/p/a/span";

        private const string AVAILABLE = "Available";

        private Stream _htmlStream = null;

        public HtmlParser(string uri)
        {
            _htmlStream = GetPageHtml(uri);
        }

        private Stream GetPageHtml(string uri)
        {
            Stream htmlStream = null;
            using (WebClient webClient = new WebClient())
            {
                htmlStream = webClient.OpenRead(uri);
            }
            
            return htmlStream;
        }

        public void GetRoomInfo()
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(_htmlStream, Encoding.UTF8);
            var liNodes = htmlDoc.DocumentNode.SelectSingleNode(ROOMS_LI_XPATH).ChildNodes.Where(x => LI_TAG.Equals(x.Name));

            // Take the img src url
            string imgUrl = string.Empty;

            string name = string.Empty, location = string.Empty, status = string.Empty,
                area = string.Empty, floor = string.Empty, structure = string.Empty,
                style = string.Empty;

            HtmlNode roomDetailNode = null;
            HtmlNode[] roomDetailSpanNodes = null;
            foreach (var liNode in liNodes)
            {
                imgUrl = liNode.SelectSingleNode(ROOM_IMG_XPATH).Attributes[0].Value.Trim();
                if (ConfigReader.FilterVal.Equals(imgUrl))
                {
                    continue;
                }
                status = AVAILABLE;
                name = liNode.SelectSingleNode(ROOM_NAME_XPATH).InnerText.Trim();
                location = liNode.SelectSingleNode(ROOM_LOCATION_XPATH).InnerText.Trim();
                roomDetailNode = liNode.SelectSingleNode(ROOM_DETAIL_XPATH).ChildNodes.Where(x => P_TAG.Equals(x.Name)).First();
                roomDetailSpanNodes = roomDetailNode.ChildNodes.Where(x => SPAN_TAG.Equals(x.Name)).ToArray();
                area = roomDetailSpanNodes[0].InnerText.Trim();
                floor = roomDetailSpanNodes[1].InnerText.Trim();
                structure = roomDetailSpanNodes[2].InnerText.Trim();
                style = liNode.SelectSingleNode(ROOM_STYLE_XPATH).InnerText.Trim();
                roomDetailNode = null;
                roomDetailSpanNodes = null;
            }
        }

        
    }

    class Room
    {
        public string Name { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;
        public string Status { get; private set; } = string.Empty;
        public string Area { get; private set; } = string.Empty;
        public string Floor { get; private set; } = string.Empty;
        public string Structure { get; private set; } = string.Empty;
        public string Style { get; private set; } = string.Empty;

        public Room(string name, string location, string status, string area, string floor, string structure, string style)
        {
            Name = name;
            Location = location;
            Status = status;
            Area = area;
            Floor = floor;
            Structure = structure;
            Style = style;
        }
    }
}
