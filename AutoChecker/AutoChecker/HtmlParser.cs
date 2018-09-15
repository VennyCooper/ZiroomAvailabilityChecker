using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoChecker
{
    class HtmlParser
    {
        // HTML tag texts
        private const string LI_TAG = "li";
        private const string P_TAG = "p";
        private const string SPAN_TAG = "span";
        private const string TEXT_TAG = "#text";

        // HTML Document XPaths
        private const string ROOMS_LI_XPATH = "//html/body" +
                "/div[@class='t_myarea t_mainbox clearfix mt15 t_zindex0']" +
                "/div[@class='t_newlistbox']/ul[@id='houseList']";
        private const string ROOM_IMG_XPATH = "div[@class='img pr']/a/img";
        private const string ROOM_NAME_XPATH = "div[@class='txt']/h3/a";
        private const string ROOM_LOCATION_XPATH = "div[@class='txt']/h4/a";
        private const string ROOM_DETAIL_XPATH = "div[@class='txt']/div[@class='detail']";
        private const string ROOM_STYLE_XPATH = "div[@class='txt']/p";

        private const string NUMBER_REGEX = @"\d+[[,.]\d+]?";

        private Stream _htmlStream = null;
        private string uri = string.Empty;

        public HtmlParser(string uri)
        {
            this.uri = uri;
            
        }

        public IEnumerable<RoomInfo> GetValidRooms()
        {
            var allRooms = GetRoomInfo();
            return FilterRooms(allRooms);
        }

        /// <summary>
        /// Reload the page to refresh the HTML text stream
        /// </summary>
        public void RefreshPage()
        {
            _htmlStream = GetPageHtml(uri);
        }

        /// <summary>
        /// Get the page HTML text stream
        /// </summary>
        /// <param name="uri">URI of the page</param>
        /// <returns>HTML stream</returns>
        private Stream GetPageHtml(string uri)
        {
            Stream htmlStream = null;
            using (WebClient webClient = new WebClient())
            {
                htmlStream = webClient.OpenRead(uri);
            }
            
            return htmlStream;
        }

        /// <summary>
        /// Get info of rooms which are available now
        /// </summary>
        /// <returns>room info of type Room</returns>
        private IEnumerable<RoomInfo> GetRoomInfo()
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(_htmlStream, Encoding.UTF8);
            var liNodes = htmlDoc.DocumentNode.SelectSingleNode(ROOMS_LI_XPATH).ChildNodes.Where(x => LI_TAG.Equals(x.Name));
            string imgUrl = string.Empty, name = string.Empty, location = string.Empty,
                status = string.Empty, floor = string.Empty, structure = string.Empty;
            double area = 0;
            List<string> styles = null;
            HtmlNode[] roomDetailSpanNodes = null;
            foreach (var liNode in liNodes)
            {
                imgUrl = liNode.SelectSingleNode(ROOM_IMG_XPATH).Attributes[0].Value.Trim();
                name = liNode.SelectSingleNode(ROOM_NAME_XPATH).InnerText.Trim();
                location = liNode.SelectSingleNode(ROOM_LOCATION_XPATH).InnerText.Trim();
                roomDetailSpanNodes = liNode.SelectSingleNode(ROOM_DETAIL_XPATH).ChildNodes
                    .Where(x => P_TAG.Equals(x.Name))
                    .First().ChildNodes
                    .Where(x => SPAN_TAG.Equals(x.Name))
                    .ToArray();
                area = double.Parse(Regex.Match(roomDetailSpanNodes[0].InnerText.Trim(), NUMBER_REGEX).Groups[0].Value);
                floor = roomDetailSpanNodes[1].InnerText.Trim();
                structure = roomDetailSpanNodes[2].InnerText.Trim();
                styles = liNode.SelectSingleNode(ROOM_STYLE_XPATH).ChildNodes
                    .Where(x => !TEXT_TAG.Equals(x.Name))
                    .Select(x => x.InnerText)
                    .ToList();

                yield return new RoomInfo(name, location, imgUrl, area, floor, structure, styles);
            }
        }

        private IEnumerable<RoomInfo> FilterRooms(IEnumerable<RoomInfo> rooms)
        {
            var validRooms = rooms
                .Where(x => !ConfigReader.ImgExclusion.Equals(x.Img))
                .Where(x => x.Area <= ConfigReader.MaxArea && x.Area >= ConfigReader.MinArea);
            if (!String.IsNullOrEmpty(ConfigReader.Orientation))
            {
                validRooms = validRooms.Where(x => ConfigReader.Orientation.Equals(x.Name.Split('-')[0].Trim()));
            }
            if (ConfigReader.StyleList.Count != 0)
            {
                validRooms = validRooms.Where(x => !x.Styles.Except(ConfigReader.StyleList).Any());
            }
            return validRooms;
        }
    }


    class RoomInfo
    {
        public string Name { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;
        public string Img { get; private set; } = string.Empty;
        public double Area { get; private set; } = 0;
        public string Floor { get; private set; } = string.Empty;
        public string Structure { get; private set; } = string.Empty;
        public List<string> Styles { get; private set; } = null;

        public RoomInfo(
            string name, 
            string location, 
            string img, 
            double area, 
            string floor, 
            string structure, 
            List<string> styles)
        {
            Name = name;
            Location = location;
            Img = img;
            Area = area;
            Floor = floor;
            Structure = structure;
            Styles = styles;
        }
    }
}
