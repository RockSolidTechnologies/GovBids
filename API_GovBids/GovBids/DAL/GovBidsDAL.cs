using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using GovBids.Entities;

namespace GovBids.DAL
{
    public static class GovBidsDAL
    {

        private static string _connectionString;

        /// <summary>
        /// Sets the connection string to the database.
        /// </summary>
        private static void SetConnectionString()
        {
            //Gets connection string in the web.config
            if (String.IsNullOrEmpty(_connectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["GovBids"].ConnectionString;
        }

        /// <summary>
        /// Fetches the data in the database for the bids based on the data for filters entered.
        /// </summary>
        /// <param name="agency">Agency for the bid.</param>
        /// <param name="category">Category of te bid.</param>
        /// <param name="location">Location fro the bid.</param>
        /// <param name="title">Title of the bid.</param>
        /// <returns>List with the available bids.</returns>
        public static List<AvailBidResponseAvailBidsBid> FetchDataBids(string agency, string category, string location, string title)
        {
            try
            {
                List<AvailBidResponseAvailBidsBid> list = new List<AvailBidResponseAvailBidsBid>();
                string sql = "SELECT BidID, ItemID, Title, Category, Agency, Location, DatePosted, DateOpened, PreBidDate, SpecificationsDate FROM Bid {0} ORDER BY Title";

                sql = String.Format(sql, BuildBidsWhere(agency, category, location, title));

                SetConnectionString();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                do
                                {
                                    //sets the value from the reader
                                    AvailBidResponseAvailBidsBid bid = new AvailBidResponseAvailBidsBid();
                                    bid.ID = reader["BidID"].ToString();
                                    bid.ItemID = reader["ItemID"].ToString();
                                    bid.Agency = (string)reader["Agency"];
                                    bid.Title = (string)reader["Title"];
                                    bid.Category = (string)reader["Category"];
                                    bid.Location = (string)reader["Location"];
                                    bid.DateOpened = reader["DateOpened"] is DBNull ? "" : reader["DateOpened"].ToString();
                                    bid.DatePosted = reader["DatePosted"] is DBNull ? "" : reader["DatePosted"].ToString();
                                    bid.PreBidDate = reader["PreBidDate"] is DBNull ? "" : reader["PreBidDate"].ToString();
                                    bid.SpecificationsDate = reader["SpecificationsDate"] is DBNull ? "" : reader["SpecificationsDate"].ToString();
                                    list.Add(bid);
                                    bid = null;
                                } while (reader.Read());
                            }
                        }
                    }
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Fetches the data in the database for the bids based on the data for filters entered.
        /// </summary>
        /// <param name="agency">Agency for the bid.</param>
        /// <param name="category">Category of te bid.</param>
        /// <param name="location">Location fro the bid.</param>
        /// <param name="title">Title of the bid.</param>
        /// <returns>List with the available bids.</returns>
        public static List<AvailBidIdResponseAvailBidIdsBid> FetchDataBidIds(string agency, string category, string location, string title)
        {
            try
            {
                List<AvailBidIdResponseAvailBidIdsBid> list = new List<AvailBidIdResponseAvailBidIdsBid>();
                string sql = "SELECT ItemID FROM Bid {0}";

                sql = String.Format(sql, BuildBidsWhere(agency, category, location, title));

                SetConnectionString();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                do
                                {
                                    //sets the value from the reader
                                    AvailBidIdResponseAvailBidIdsBid bid = new AvailBidIdResponseAvailBidIdsBid();
                                    bid.ItemID = reader["ItemID"].ToString();
                                    list.Add(bid);
                                    bid = null;
                                } while (reader.Read());
                            }
                        }
                    }
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Fetches the data in the database for the bids by a set ID.
        /// </summary>
        /// <param name="ID">Id of the bid.</param>
        /// <returns>An Available bid.</returns>
        public static AvailBidResponseAvailBidsBid FetchDataBidByID(int ID)
        {
            try
            {
                AvailBidResponseAvailBidsBid bid = new AvailBidResponseAvailBidsBid();
                string sql = "SELECT BidID, ItemID, Title, Category, Agency, Location, DatePosted, DateOpened, PreBidDate, SpecificationsDate FROM Bid {0}";
                sql = String.Format(sql, "WHERE BidID = " + ID.ToString());

                SetConnectionString();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //sets the value from the reader
                                bid.ID = reader["BidID"].ToString();
                                bid.ItemID = reader["ItemID"].ToString();
                                bid.Agency = (string)reader["Agency"];
                                bid.Title = (string)reader["Title"];
                                bid.Category = (string)reader["Category"];
                                bid.Location = (string)reader["Location"];
                                bid.DateOpened = reader["DateOpened"] is DBNull ? "" : reader["DateOpened"].ToString();
                                bid.DatePosted = reader["DatePosted"] is DBNull ? "" : reader["DatePosted"].ToString();
                                bid.PreBidDate = reader["PreBidDate"] is DBNull ? "" : reader["PreBidDate"].ToString();
                                bid.SpecificationsDate = reader["SpecificationsDate"] is DBNull ? "" : reader["SpecificationsDate"].ToString();
                            }
                        }
                    }
                }

                return bid;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Builds the where statement for the bids.
        /// </summary>
        /// <param name="agency">Agency for the bid.</param>
        /// <param name="category">Category of te bid.</param>
        /// <param name="location">Location fro the bid.</param>
        /// <param name="title">Title of the bid.</param>
        /// <returns>Returns a string representing the where statement.</returns>
        private static string BuildBidsWhere(string agency, string category, string location, string title)
        {
            try
            {
                string where = String.Empty;

                //if the variable is empty no need to add to the where statement
                if (!String.IsNullOrEmpty(agency))
                {
                    where += "Agency LIKE '%" + agency + "%' OR ";
                }

                if (!String.IsNullOrEmpty(category))
                {
                    where += "Category LIKE '%" + category + "%' OR ";
                }

                if (!String.IsNullOrEmpty(location))
                {
                    where += "Location LIKE '%" + location + "%' OR ";
                }

                if (!String.IsNullOrEmpty(title))
                {
                    where += "Title LIKE '%" + title + "%' OR ";
                }

                if (!String.IsNullOrEmpty(where))
                    where = String.Format("WHERE {0}", where.Remove(where.Length - 4, 4));

                return where;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Fetches all the categories in the table.
        /// </summary>
        /// <returns>Returns a list of categories.</returns>
        public static List<CategoryItem> FetchDataCategories()
        {
            try
            {
                List<CategoryItem> list = new List<CategoryItem>();
                string sql = "SELECT DISTINCT Category FROM Bid";

                SetConnectionString();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                do
                                {
                                    //sets the value from the reader
                                    CategoryItem item = new CategoryItem();
                                    item.Value = (string)reader["Category"];
                                    list.Add(item);
                                    item = null;
                                } while (reader.Read());
                            }
                        }
                    }
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Fetches all the agencies in the table.
        /// </summary>
        /// <returns>Returns a list of agencies.</returns>
        public static List<AgencyItem> FetchDataAgencies()
        {
            try
            {
                List<AgencyItem> list = new List<AgencyItem>();
                string sql = "SELECT DISTINCT Agency FROM Bid";

                SetConnectionString();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                do
                                {
                                    //sets the value from the reader
                                    AgencyItem item = new AgencyItem();
                                    item.Value = (string)reader["Agency"];
                                    list.Add(item);
                                    item = null;
                                } while (reader.Read());
                            }
                        }
                    }
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Fetches all the locations in the table.
        /// </summary>
        /// <returns>Returns a list of locations.</returns>
        public static List<LocationItem> FetchDataLocation()
        {
            try
            {
                List<LocationItem> list = new List<LocationItem>();
                string sql = "SELECT DISTINCT Location FROM Bid";

                SetConnectionString();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                do
                                {
                                    //sets the value from the reader
                                    LocationItem item = new LocationItem();
                                    item.Value = (string)reader["Location"];
                                    list.Add(item);
                                    item = null;
                                } while (reader.Read());
                            }
                        }
                    }
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Fetches all the
        /// </summary>
        /// <returns>Returns a list of locations.</returns>
        public static List<Device> FetchDataDeviceTokens()
        {
            try
            {
                List<Device> list = new List<Device>();
                string sql = "SELECT DeviceID, DeviceToken, DeviceType FROM Device";

                SetConnectionString();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                do
                                {
                                    //sets the value from the reader
                                    Device item = new Device();
                                    item.DeviceID = (int)reader["DeviceID"];
                                    item.DeviceToken = (string)reader["DeviceToken"];
                                    item.DeviceType = (string)reader["DeviceType"];
                                    list.Add(item);
                                    item = null;
                                } while (reader.Read());
                            }
                        }
                    }
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}