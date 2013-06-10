using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace GovBids.Models
{
    public class FilterSettings
    {
        public FilterSettings()
        {
            //Load the settings from iso
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            //if it contains the first run key then we load the settings
            if (settings.Contains("FirstRun"))
            {
                Location = settings["Location"] as string;
                Category = settings["Category"] as string;
                Agency = settings["Agency"] as string;
            }

            //Load default
            else
            {
                Location = "";
                settings.Add("Location", Location);

                Location = "";
                settings.Add("Category", Category);

                Location = "";
                settings.Add("Agency", Agency);

                settings.Add("FirstRun", DateTime.Now);

                //Save the settings
                settings.Save();
            }
        }

        public void SaveSettings()
        {
            //Load the settings from iso
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["Agency"] = Agency;
            settings["Location"] = Location;
            settings["Category"] = Category;
            settings.Save();
        }

        public string Location { get; set; }
        public string Category { get; set; }
        public string Agency { get; set; }
        public bool Update { get; set; }
    }
}
