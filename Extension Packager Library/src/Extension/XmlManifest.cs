// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Helper;
using System.IO;
using System.Xml;

namespace Extension_Packager_Library.src.Extension
{
    internal class XmlManifest
    {
        internal string Create(string crxFileUrl, string version, string appId)
        {
            bool fileFound = ResourceFile.TryGetFullPath(@"Assets\App.xml", out string appXmlPath);
            if (!fileFound)
            {
                throw new FileNotFoundException($"The default App.xml file was not found. Path={appXmlPath}");
            }
            XmlDocument doc = ReadXmlDoc(appXmlPath);
            SetXmlAttribute(doc.DocumentElement, "app", "appid", appId);
            SetXmlAttribute(doc.DocumentElement, "updatecheck", "codebase", crxFileUrl);
            SetXmlAttribute(doc.DocumentElement, "updatecheck", "version", version);
            return doc.OuterXml;
        }

        private XmlDocument ReadXmlDoc(string appXmlPath)
        {
            string defaultAppXml = File.ReadAllText(appXmlPath);
            XmlDocument doc = new();
            doc.LoadXml(defaultAppXml);
            return doc;
        }

        private void SetXmlAttribute(XmlElement docElement, string elementName, string name, string value)
        {
            XmlNode element = docElement.GetElementsByTagName(elementName)[0];
            XmlAttribute attribute = element.Attributes[name];
            attribute.Value = value;
        }

    }
}
