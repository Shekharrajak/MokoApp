using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using System.Diagnostics;

namespace MokoApp
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
            for (int i = 0; i < 4; ++i)
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

#if false
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem
    {
        public string Title { get; set; }
        public string Description { get;  set; }
        public string ImagePath { get;  set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup
    {
        public SampleDataGroup()
        {
            this.Items = new ObservableCollection<SampleDataItem>();
        }

        public string Title { get; set; }

        public ObservableCollection<SampleDataItem> Items { get;  set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        //private static SampleDataSource _sampleDataSource = new SampleDataSource();
        private HtmlWeb web = new HtmlWeb();
        private string url = "http://www.moko.cc/channels/post/23/1.html";
        private string main_url = "http://www.moko.cc";

        private ObservableCollection<SampleDataGroup> _groups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> Groups
        {
            get { return this._groups; }
        }

        public async Task<IEnumerable<SampleDataGroup>> GetGroupsAsync()
        {
            await GetSampleDataAsync();

            return Groups;
        }
        
        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            HtmlDocument doc = await web.LoadFromWebAsync(url);
            List<HtmlNode> nodes = doc.DocumentNode.Descendants("ul").Where(n => n.Attributes.Contains("class") && n.Attributes.First().Value == "post small-post").ToList();

            for (int i = 0; i < 4; ++i)
            {
                var current_node = nodes[i];
                if (current_node.HasChildNodes)
                {
                    var node = current_node.ChildNodes.ElementAt(1);
                    var data_group = new SampleDataGroup();

                    data_group.Title = node.Attributes.Last().Value;
                    var href_link = main_url + node.ChildNodes.ElementAt(1).Attributes.First().Value;

                    // Get Items Async
                    for (int j = 0; j < 3; j++)
                    {
                        data_group.Items.Add(new SampleDataItem(){Title="ffff", Description="ddd",ImagePath="ff"});
                    }

                    _groups.Add(data_group);
                }
            }
        }
    }
#endif
}
