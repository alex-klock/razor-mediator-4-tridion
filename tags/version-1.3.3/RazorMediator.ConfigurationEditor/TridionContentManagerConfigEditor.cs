using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RazorMediator.ConfigurationEditor
{
    public class TridionContentManagerConfigEditor
    {
        private const string RAZOR_SECTION_XML = "<section name=\"razor.mediator\" type=\"Tridion.Extensions.Mediators.Razor.Configuration.RazorMediatorConfigurationSection, Tridion.Extensions.Mediators.Razor, Version=1.3.3.0, Culture=neutral, PublicKeyToken=5eeceedb34d9dfd7\" />";
        
        private const string RAZOR_MEDIATOR_XML = "<mediator matchMIMEType=\"text/x-tcm-cshtml\" type=\"Tridion.Extensions.Mediators.Razor.RazorMediator, Tridion.Extensions.Mediators.Razor, Version=1.3.3.0, Culture=neutral, PublicKeyToken=5eeceedb34d9dfd7\" />";
        
        private const string RAZOR_TEMPLATE_TYPE_XML =
            "<add id=\"{0}\" name=\"RazorTemplate\" mimeType=\"text/x-tcm-cshtml\" hasBinaryContent=\"false\" contentHandler=\"Tridion.Extensions.Mediators.Razor.RazorContentHandler, Tridion.Extensions.Mediators.Razor, Version=1.3.3.0, Culture=neutral, PublicKeyToken=5eeceedb34d9dfd7\">" +
            "<webDavFileExtensions>" + 
            "<add itemType=\"TemplateBuildingBlock\" fileExtension=\"cshtml\" />" +
            "</webDavFileExtensions>" +
            "</add>";

        private const string RAZOR_CONFIG_SECTION_INNER_XML =
            "<namespaces>" +
            "<!-- <add namespace=\"System.Linq\" />-->" +
            "<!-- <add namespace=\"Test.Sample\" />-->" +
            "</namespaces>" +
            "<assemblies>" +
            "<!-- <add assembly=\"C:\\Program Files\\Assembly\\Test.Sample.dll\" /> -->" +
            "<!-- <add assembly=\"RazorSample.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=60ad7434f03dfcdc\" /> -->" +
            "</assemblies>" +
            "<imports>" +
            "<!-- <add import=\"tcm:120-2233-2048\" />-->" +
            "<!-- <add import=\"/wevdav/020 Design/Building Blocks/System/TBBs/Helpers/razor-helpers.cshtml\" />-->" +
            "<!-- <add import=\"C:\\Program Files\\Razor Mediator\\razor-helpers.txt\" />-->" +
            "<!-- <add import=\"tcm:120-2200-2048\" publications=\"020 Design Master,030 Another Web Design\" />-->" +
            "</imports>" +
            "<importSettings includeConfigWhereUsed=\"true\" includeImportWhereUsed=\"true\" replaceRelativePaths=\"false\" />";

        private string _tridionInstallPath;

        private XmlDocument _configuration;

        private XmlNode _mediators;

        private XmlNode _templateTypes;

        public TridionContentManagerConfigEditor()
        {
            _configuration = new XmlDocument();
            _configuration.Load(ConfigurationFilePath);
            _mediators = _configuration.SelectSingleNode("/configuration/tridion.templating/mediators");
            _templateTypes = _configuration.SelectSingleNode("/configuration/templateTypeRegistry/templateTypes");
        }

        public void Install()
        {
            CreateBackUp();

            RemoveRazorSectionXml();
            RemoveRazorConfigSectionXml();
            RemoveRazorTemplateTypeXml();
            RemoveRazorMediatorXml();

            AddRazorSectionXml();
            AddRazorConfigSectionXml();
            AddRazorTemplateTypeXml();
            AddRazorMediatorXml();

            Save();
        }

        public void UnInstall()
        {
            CreateBackUp();

            RemoveRazorSectionXml();
            RemoveRazorConfigSectionXml();
            RemoveRazorTemplateTypeXml();
            RemoveRazorMediatorXml();

            Save();
        }

        private void AddRazorSectionXml()
        {
            XmlNode sections = _configuration.SelectSingleNode("/configuration/configSections");
            sections.InnerXml += RAZOR_SECTION_XML;
        }

        private void AddRazorConfigSectionXml()
        {
            XmlNode configSection = _configuration.CreateElement("razor.mediator");
            configSection.InnerXml = RAZOR_CONFIG_SECTION_INNER_XML;

            XmlAttribute extractBinaries = _configuration.CreateAttribute("extractBinaries");
            XmlAttribute adminUser = _configuration.CreateAttribute("adminUser");

            extractBinaries.Value = "true";
            adminUser.Value = "INSERT TRIDION USERNAME";

            configSection.Attributes.Append(extractBinaries);
            configSection.Attributes.Append(adminUser);

            _configuration.DocumentElement.AppendChild(configSection);
        }

        private void AddRazorMediatorXml()
        {
            _mediators.InnerXml += RAZOR_MEDIATOR_XML;
        }

        private void AddRazorTemplateTypeXml()
        {
            XmlNode dwType = _configuration.SelectSingleNode("/configuration/templateTypeRegistry/templateTypes/add[@name='DreamweaverTemplate']");
            _templateTypes.InnerXml = _templateTypes.InnerXml.Replace(dwType.OuterXml, dwType.OuterXml + String.Format(RAZOR_TEMPLATE_TYPE_XML, GetNextTemplateTypeID()));
        }

        private void CreateBackUp()
        {
            DateTime saved = DateTime.Now;
            _configuration.Save(ConfigurationFilePath + "." + DateTime.Now.ToString("yyyy-MM-dd.hh-mm-ss") + ".BAK");
        }

        /// <summary>
        /// Gets the next template ID. Selects "8" if available, otherwise choosest the next highest number.
        /// </summary>
        /// <returns></returns>
        private string GetNextTemplateTypeID()
        {
            int highestID = 0;
            bool isEightTaken = false;

            foreach (XmlNode node in _templateTypes.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment) continue;
                if (node.Name.Equals("clear")) continue;

                int id = Int32.Parse(node.Attributes["id"].Value);
                if (id > highestID)
                    highestID = id;
                if (id == 8)
                    isEightTaken = true;
            }

            if (!isEightTaken)
            {
                return "8";
            }
            
            return (highestID + 1).ToString();
        }

        private void RemoveRazorSectionXml()
        {
            XmlNode razorSection = _configuration.SelectSingleNode("/configuration/configSections/section[@name='razor.mediator']");
            if (razorSection != null)
            {
                XmlNode sections = _configuration.SelectSingleNode("/configuration/configSections");
                sections.RemoveChild(razorSection);
            }
        }

        private void RemoveRazorConfigSectionXml()
        {
            XmlNode razorConfig = _configuration.SelectSingleNode("/configuration/razor.mediator");
            if (razorConfig != null)
            {
                _configuration.DocumentElement.RemoveChild(razorConfig);
            }
        }

        private void RemoveRazorMediatorXml()
        {
            XmlNode razor = _configuration.SelectSingleNode("/configuration/tridion.templating/mediators/mediator[@matchMIMEType=\"text/x-tcm-cshtml\"]");
            if (razor != null)
            {
                _mediators.RemoveChild(razor);
            }
        }

        private void RemoveRazorTemplateTypeXml()
        {
            XmlNode razorType = _configuration.SelectSingleNode("/configuration/templateTypeRegistry/templateTypes/add[@name='RazorTemplate']");
            if (razorType != null)
            {
                _templateTypes.RemoveChild(razorType);
            }
        }

        private void Save()
        {
            _configuration.Save(ConfigurationFilePath);
        }

        private string TridionInstallPath
        {
            get
            {
                if (_tridionInstallPath == null)
                {
                    Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Tridion");
                    if (registryKey == null)
                    {
                        registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Tridion");
                    }
                    if (registryKey == null)
                    {
                        throw new Exception("Tridion Installation Path can not be found.");
                    }

                    _tridionInstallPath = registryKey.GetValue("InstallDir").ToString();
                }
                return _tridionInstallPath;
            }
        }

        private string ConfigurationFilePath
        {
            get
            {
                return TridionInstallPath + @"config\Tridion.ContentManager.config";
            }
        }
    }

}
