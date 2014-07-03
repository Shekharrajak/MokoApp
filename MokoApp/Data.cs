using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MokoApp
{
    public class DataSource : ObservableCollection<details.MokoModel>, ISupportIncrementalLoading
    {
        private HtmlWeb web = new HtmlWeb();
        private string m_model_page_url = "http://www.moko.cc/channels/post/23/";  // + 1.html
        private string m_main_url = "http://www.moko.cc";
        private uint m_maxCount = 100;
        private int m_page_number = 1;

        public async Task getModels()
        {
            var htmlNodeList = await parsing_html();

            foreach (var current_node in htmlNodeList)
            {
                if (current_node.HasChildNodes)
                {
                    var node = current_node.ChildNodes.ElementAt(1);
                    details.MokoModel model = new details.MokoModel();

                    model.Title = node.Attributes.Last().Value;

                    var href_node = node.ChildNodes.ElementAt(1);
                    model.href_link = m_main_url + href_node.Attributes.First().Value;

                    var img_node = href_node.ChildNodes.ElementAt(0);
                    model.ImagePath = img_node.Attributes.First().Value;

                    this.Add(model);
                }
            }
        }

        private async Task<List<HtmlNode>> parsing_html()
        {
            string url = m_model_page_url + m_page_number + ".html";

            HtmlDocument doc = await web.LoadFromWebAsync(url);
            return doc.DocumentNode.Descendants("ul").Where(n => n.Attributes.Contains("class")
                && n.Attributes.First().Value == "post small-post").ToList();
        }

        #region ISupportIncrementalLoading
        public bool HasMoreItems
        {
            get { return this.Count < m_maxCount; }
        }

        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run(async (cts) =>
            {
                await getModels();
                m_page_number++;
                return new LoadMoreItemsResult() { Count = (uint)this.Count };
            });
        }
        #endregion
    }

    namespace details
    {
        public class MokoModel
        {
            public string Title { get; set; }
            public string href_link { get; set; }
            public string ImagePath { get; set; }
        }
    }
}

#region GroupView DataModel
namespace MokoApp.GroupView
{
    public class MokoDataItem
    {
        //public string Title { get; set; }
        //public string Description { get; set; }
        public string ImagePath { get; set; }
    }

    public class DataGroup
    {
        public DataGroup()
        {
            this.Items = new ObservableCollection<MokoDataItem>();
        }
        public string Title { get; set; }
        public ObservableCollection<MokoDataItem> Items{get; set;}


        public override string ToString()
        {
            return this.Title;
        }
    }

    public sealed class DataSource
    {
        private HtmlWeb web = new HtmlWeb();
        private string url = "http://www.moko.cc/channels/post/23/1.html";
        private string main_url = "http://www.moko.cc";

        private ObservableCollection<DataGroup> _groups = new ObservableCollection<DataGroup>();

        public ObservableCollection<DataGroup> Groups
        {
            get { return this._groups; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DataGroup>> GetGroupsAsync()
        {
            await GetDataAsync();

            return Groups;
        }

        private async Task GetDataAsync()
        {
            if (this._groups.Count != 0)
                return;
            HtmlDocument doc = await web.LoadFromWebAsync(url);
            List<HtmlNode> nodes = doc.DocumentNode.Descendants("ul").Where(n => n.Attributes.Contains("class") && n.Attributes.First().Value == "post small-post").ToList();

            //foreach (var current_node in nodes)
            for (int i = 0; i < 6; ++i)
            {
                var current_node = nodes[i];
                if (current_node.HasChildNodes)
                {
                    var node = current_node.ChildNodes.ElementAt(1);
                    DataGroup data_group = new DataGroup();

                    data_group.Title = node.Attributes.Last().Value;
                    var href_link = main_url + node.ChildNodes.ElementAt(1).Attributes.First().Value;

                    // Get Items Async
                    await GetImagesAsync(data_group, href_link);

                    _groups.Add(data_group);
                }
            }
        }

        private async Task GetImagesAsync(DataGroup group, string link)
        {
            HtmlWeb img_web = new HtmlWeb();
            HtmlDocument detail_page = await img_web.LoadFromWebAsync(link);
            List<HtmlNode> nodes = detail_page.DocumentNode.Descendants("p").Where(n => n.Attributes.Contains("class") && n.Attributes.First().Value == "picBox").ToList();

            foreach (var current_node in nodes)
            {
                if (current_node.HasChildNodes)
                {
                    var node = current_node.LastChild;
                    var t = node.Attributes.First().Value;
                    group.Items.Add(new MokoDataItem() { ImagePath = t });
                }
            }
        }
    }
#endregion

}
