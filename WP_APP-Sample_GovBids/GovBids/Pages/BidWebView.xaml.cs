using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace GovBids.Pages
{
    public partial class BidWebView : PhoneApplicationPage
    {
        public BidWebView()
        {
            InitializeComponent();
        }

        string url;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string Id;

            if (NavigationContext.QueryString.TryGetValue("Id", out Id))
            {
                url = "http://www2.pr.gov/subasta/Pages/aviso.aspx?itemID=" + Id;
                webBrowser.Navigate(new Uri(url));
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            WebBrowserTask web = new WebBrowserTask();
            web.Uri = new Uri(url);
            web.Show();
        }
    }
}