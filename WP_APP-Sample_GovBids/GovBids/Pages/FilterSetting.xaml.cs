using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GovBids.WebService;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using GovBids.Models;

namespace GovBids.Pages
{
    public partial class FilterSetting : PhoneApplicationPage
    {
        AvailBidsSoapClient client;
        FilterSettings currentFilters;

        public FilterSetting()
        {
            InitializeComponent();

            //Initialize the soap client
            client = new AvailBidsSoapClient();

            client.GetLocationsCompleted += client_GetLocationsCompleted;
            client.GetAgenciesCompleted += client_GetAgenciesCompleted;
            client.GetCategoriesCompleted += client_GetCategoriesCompleted;

            asyncCompleted = 0;
            //Block the UI
            busyIndicator.IsRunning = true;

            client.GetAgenciesAsync();
            client.GetCategoriesAsync();
            client.GetLocationsAsync();

            currentFilters = (App.Current as App).filterSettings;
        }

        //To keep track of how many of the request have finish
        int asyncCompleted = 0;
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        void client_GetLocationsCompleted(object sender, GetLocationsCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string xml = e.Result;
                var locList = getCatListFromXml(xml);
                locationPicker.ItemsSource = locList;

                if (!String.IsNullOrWhiteSpace(currentFilters.Location))
                {
                    var current = (from p in locList where (p as Item).Name == currentFilters.Location select p).First();
                    locationPicker.SelectedItem = current;
                }
                
            }
            asyncCompleted++;
            CheckFinish();
        }

        void client_GetAgenciesCompleted(object sender, GetAgenciesCompletedEventArgs e)
        {
            asyncCompleted++;
            if (e.Error == null)
            {
                string xml = e.Result;
                var agenList = getCatListFromXml(xml);
                agencyPicker.ItemsSource = agenList;

                if (!String.IsNullOrWhiteSpace(currentFilters.Agency))
                {
                    var current = (from p in agenList where (p as Item).Name == currentFilters.Agency select p).First();
                    agencyPicker.SelectedItem = current;
                }
            }
            asyncCompleted++;
            CheckFinish();
        }

        void client_GetCategoriesCompleted(object sender, GetCategoriesCompletedEventArgs e)
        {
            asyncCompleted++;
            if (e.Error == null)
            {
                string xml = e.Result;
                var catList = getCatListFromXml(xml);
               categoryPicker.ItemsSource = catList;

               if (!String.IsNullOrWhiteSpace(currentFilters.Category))
               {
                   var current = (from p in catList where (p as Item).Name == currentFilters.Category select p).First();
                   categoryPicker.SelectedItem = current;
               }
            }
            asyncCompleted++;
            CheckFinish();
        }

        void CheckFinish()
        {
            //if asyncComplteted = 3 then we finish loadin
            if (asyncCompleted >= 3)
            {
                busyIndicator.IsRunning = false;
            }
        }

        /// <summary>
        /// Returns th specify Item string
        /// </summary>
        /// <param name="xml">String containing XML data from web service.</param>
        /// <returns></returns>
        private List<Item> getCatListFromXml(string xml)
        {
            //Create and XML reader from the string
            XmlReader r = XmlReader.Create(new StringReader(xml));

            //Load the document
            XDocument data = XDocument.Load(r);

            //Get all the descendants from Bid
            var test = from query in data.Descendants("Item")
                       select query;

            //Initialize a list (will be return)
            var list = new List<Item>();

            //add none item
            list.Add(new Item(){ Name = "ninguna"});

            #region Foreach bid...
            foreach (var query in test.ToList())
            {
                string item = string.Empty;

                //First check if it is not null (else it will crash)
                if (query.Value != null)
                    item = query.Value; //Cast and assign

                //add it to the list
                list.Add(new Item() { Name = item });
            }
            #endregion

            //return the list with all the bids
            return list;
        }

        /// <summary>
        /// Saves the filter settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            //Get the selected items from all the list pickers
            string loc = (locationPicker.SelectedItem as Item).Name;
            string cat = (categoryPicker.SelectedItem as Item).Name;
            string agent = (agencyPicker.SelectedItem as Item).Name;


            if (loc == "ninguna")
                loc = "";
            if (cat == "ninguna")
                cat = "";
            if (agent == "ninguna")
                agent = "";

            currentFilters.Location = loc;
            currentFilters.Category = cat;
            currentFilters.Agency = agent;
            currentFilters.Update = true;

            currentFilters.SaveSettings();
            NavigationService.GoBack();
        } 
    }

    public class Item
    {
        public string Name { get; set; }
    }
}