using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Services;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using GovBids.DAL;

namespace GovBids
{
    /// <summary>
    /// PR Government available bids to support GovBids application.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService()]
    public class AvailBids : System.Web.Services.WebService
    {

        #region Web Methods
        /// <summary>
        /// Gets all or some bids based on the filter entries.
        /// </summary>
        /// <returns>Returns the list as XML format.</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Xml, UseHttpGet = true, XmlSerializeString = true)]
        public string GetBids(string agency, string category, string location, string title)
        {
            string response = String.Empty;
            AvailBidResponse availBidRes = new AvailBidResponse();
            AvailBidResponseAvailBids availBids = new AvailBidResponseAvailBids();
            try
            {
                availBids.Bid = GovBidsDAL.FetchDataBids(agency, category, location, title).ToArray();
                availBidRes.Items = new AvailBidResponseAvailBids[] { availBids };
                response = Serialize(availBidRes);
            }
            catch (Exception ex)
            {
                AvailBidResponseException exception = new AvailBidResponseException();
                exception.Error = ex.Message;
                response = Serialize(exception);
            }
            finally
            {
                availBidRes = null;
                availBids = null;
            }
            return response;
        }

        /// <summary>
        /// Gets all or some bids IDs based on the filter entries.
        /// </summary>
        /// <returns>Returns the list as XML format.</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Xml, UseHttpGet = true, XmlSerializeString = true)]
        public string GetBidIds(string agency, string category, string location, string title)
        {
            string response = String.Empty;
            AvailBidIdResponse availBidRes = new AvailBidIdResponse();
            AvailBidIdResponseAvailBidIds availBidIds = new AvailBidIdResponseAvailBidIds();
            try
            {
                availBidIds.Bid = GovBidsDAL.FetchDataBidIds(agency, category, location, title).ToArray();
                availBidRes.Items = new AvailBidIdResponseAvailBidIds[] { availBidIds };
                response = Serialize(availBidRes);
            }
            catch (Exception ex)
            {
                AvailBidResponseException exception = new AvailBidResponseException();
                exception.Error = ex.Message;
                response = Serialize(exception);
            }
            finally
            {
                availBidRes = null;
                availBidIds = null;
            }
            return response;
        }

        /// <summary>
        /// Gets all the categories available for bids.
        /// </summary>
        /// <returns>Returns the list as XML format.</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Xml, UseHttpGet = true, XmlSerializeString = true)]
        public string GetCategories()
        {
            string response = String.Empty;
            Category category = new Category();
            try
            {
                category.Items = GovBidsDAL.FetchDataCategories().ToArray();
                response = Serialize(category);
            }
            catch (Exception ex)
            {
                AvailBidResponseException exception = new AvailBidResponseException();
                exception.Error = ex.Message;
                response = Serialize(exception);
            }
            return response;
        }

        /// <summary>
        /// Gets all the agencies available for bids. 
        /// </summary>
        /// <returns>Returns the list as XML format.</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Xml, UseHttpGet = true, XmlSerializeString = true)]
        public string GetAgencies()
        {
            string response = String.Empty;
            Agency agency = new Agency();
            try
            {
                agency.Items = GovBidsDAL.FetchDataAgencies().ToArray();
                response = Serialize(agency);
            }
            catch (Exception ex)
            {
                AvailBidResponseException exception = new AvailBidResponseException();
                exception.Error = ex.Message;
                response = Serialize(exception);
            }
            return response;
        }

        /// <summary>
        /// Gets all the locaions available for bids. 
        /// </summary>
        /// <returns>Returns the list as XML format.</returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Xml, UseHttpGet = true, XmlSerializeString = true)]
        public string GetLocations()
        {
            string response = String.Empty;
            Location location = new Location();
            try
            {
                location.Items = GovBidsDAL.FetchDataLocation().ToArray();
                response = Serialize(location);
            }
            catch (Exception ex)
            {
                AvailBidResponseException exception = new AvailBidResponseException();
                exception.Error = ex.Message;
                response = Serialize(exception);
            }
            return response;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Deserializes the XML string to be used, and put into the generated classes.
        /// </summary>
        /// <param name="xml">XML as string.</param>
        /// <param name="type">Class type for which it is going to be deserialized.</param>
        /// <returns>Object of type Object deserialized from a XML string, to the aoType passed as param.</returns>
        private object Deserialize(string xml, Type type)
        {
            object xmlObj = null;
            XmlSerializer serializer = null;
            XmlTextReader xmlReader = null;
            StringReader stringReader = null;
            try
            {
                if (!String.IsNullOrEmpty(xml))
                {
                    serializer = new XmlSerializer(type);
                    stringReader = new StringReader(xml);
                    xmlReader = new XmlTextReader(stringReader);
                    xmlObj = serializer.Deserialize(xmlReader);
                }

                return xmlObj;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (stringReader != null)
                {
                    stringReader.Close();
                }
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
                serializer = null;
                xmlReader = null;
                stringReader = null;
            }
        }

        /// <summary>
        /// Serializes the XML object into a structured XML string.
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <returns>XML as string.</returns>
        private string Serialize(object obj)
        {
            string xml = String.Empty;
            XmlSerializer serializer = null;
            StringWriter writer = null;
            try
            {
                if (obj != null)
                {
                    serializer = new XmlSerializer(obj.GetType());
                    writer = new StringWriter();
                    serializer.Serialize(writer, obj);
                    xml = writer.ToString();
                }

                return xml;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                }
                writer = null;
                serializer = null;
            }
        }
        #endregion
    }
}