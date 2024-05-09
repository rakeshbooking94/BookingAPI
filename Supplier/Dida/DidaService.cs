using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using TravillioXMLOutService.Models;

namespace TravillioXMLOutService.Supplier.Dida
{
    public class DidaService : IDisposable
    {
        #region Global vars
        string _ClientID = string.Empty;
        string _Licensekey = string.Empty;
        string customerid = string.Empty;
        string dmc = string.Empty;
        const int supplierid = 50;
        int chunksize = 50;
        int sup_cutime = 20, threadCount = 2;
        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
        DidaRequest didaRequest;
        XNamespace xmlns = "http://schemas.xmlsoap.org/soap/envelope/";
        #endregion
         public DidaService(string _customerid)
        {
            XElement suppliercred = supplier_Cred.getsupplier_credentials(_customerid, "50");
            try
            {
                _ClientID = suppliercred.Descendants("ClientID").FirstOrDefault().Value;
                _Licensekey = suppliercred.Descendants("Licensekey").FirstOrDefault().Value;
            }
            catch { }
        }
        public DidaService()
        {
             
        }
        #region Hotel Availability

        #endregion
        #region Remove Namespaces
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            XElement xmlDocumentWithoutNs = removeAllNamespaces(xmlDocument);
            return xmlDocumentWithoutNs;
        }

        private static XElement removeAllNamespaces(XElement xmlDocument)
        {
            var stripped = new XElement(xmlDocument.Name.LocalName);
            foreach (var attribute in
                    xmlDocument.Attributes().Where(
                    attribute =>
                        !attribute.IsNamespaceDeclaration &&
                        String.IsNullOrEmpty(attribute.Name.NamespaceName)))
            {
                stripped.Add(new XAttribute(attribute.Name.LocalName, attribute.Value));
            }
            if (!xmlDocument.HasElements)
            {
                stripped.Value = xmlDocument.Value;
                return stripped;
            }
            stripped.Add(xmlDocument.Elements().Select(
                el =>
                    RemoveAllNamespaces(el)));
            return stripped;
        }
        #endregion
        #region Dispose
        /// <summary>
        /// Dispose all used resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}