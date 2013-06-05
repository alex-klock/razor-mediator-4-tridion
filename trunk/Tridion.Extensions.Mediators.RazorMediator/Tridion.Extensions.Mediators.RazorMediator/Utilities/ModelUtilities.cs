using System;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;
using Tridion.Extensions.Mediators.Razor.Models;

namespace Tridion.Extensions.Mediators.Razor.Utilities
{
    /// <summary>
    /// Class for getting Model objects out of Tridion.
    /// </summary>
    public class ModelUtilities
    {
        /// <summary>
        /// The Tridion Templating Engine instance.
        /// </summary>
        protected Engine _engine;

        /// <summary>
        /// The Tridion Templating Logger
        /// </summary>
        private TemplatingLogger _logger;

        /// <summary>
        /// Gets the Tridion Templating Logger
        /// </summary>
        protected TemplatingLogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = TemplatingLogger.GetLogger(this.GetType());

                return _logger;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">The Tridion tempalating instance</param>
        public ModelUtilities(Engine engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Gets a ComponentModel object.
        /// </summary>
        /// <param name="tcmUri">The tcm uri to get the object for.</param>
        /// <returns>A ComponentModel</returns>
        public dynamic GetComponent(TcmUri tcmUri)
        {
            return GetComponent(tcmUri.ToString());
        }

        /// <summary>
        /// Gets a ComponentModel object.
        /// </summary>
        /// <param name="itemUriOrWebDavUrl"></param>
        /// <returns></returns>
        public dynamic GetComponent(string itemUriOrWebDavUrl)
        {
            Component c = _engine.GetObject(itemUriOrWebDavUrl) as Component;

            if (c == null)
            {
                Logger.Error(String.Format("Unable To GetComponent With '{0}'", itemUriOrWebDavUrl));
                return null;
            }

            return new ComponentModel(_engine, c);
        }

        /// <summary>
        /// Gets a ComponentTemplateModel object.
        /// </summary>
        /// <param name="tcmUri"></param>
        /// <returns></returns>
        public dynamic GetComponentTemplate(TcmUri tcmUri)
        {
            return GetComponentTemplate(tcmUri.ToString());
        }

        /// <summary>
        /// Gets a ComponentTemplateModel object.
        /// </summary>
        /// <param name="itemUriOrWebDavUrl"></param>
        /// <returns></returns>
        public dynamic GetComponentTemplate(string itemUriOrWebDavUrl)
        {
            ComponentTemplate ct = _engine.GetObject(itemUriOrWebDavUrl) as ComponentTemplate;

            if (ct == null)
            {
                Logger.Error(String.Format("Unable to GetComponentTemplate With '{0}'", itemUriOrWebDavUrl));
                return null;
            }
            return new ComponentTemplateModel(_engine, ct);
        }

        /// <summary>
        /// Gets a FolderModel object.
        /// </summary>
        /// <param name="tcmUri"></param>
        /// <returns></returns>
        public dynamic GetFolder(TcmUri tcmUri)
        {
            return GetFolder(tcmUri.ToString());
        }

        /// <summary>
        /// Gets a FolderModel object.
        /// </summary>
        /// <param name="itemUriOrWebDavUrl">The tcm uri or webdav url of the folder to get.</param>
        /// <returns></returns>
        public dynamic GetFolder(string itemUriOrWebDavUrl)
        {
            Folder f = _engine.GetObject(itemUriOrWebDavUrl) as Folder;

            if (f == null)
            {
                Logger.Error(String.Format("Unable To GetFolder With '{0}'", itemUriOrWebDavUrl));
                return null;
            }

            return new FolderModel(_engine, f);
        }

        /// <summary>
        /// Gets a KeywordModel object.
        /// </summary>
        /// <param name="itemUriOrWebDavUrl">The tcm uri or webdav url of the keyword to get.</param>
        /// <returns></returns>
        public dynamic GetKeyword(string itemUriOrWebDavUrl)
        {
            Keyword k = _engine.GetObject(itemUriOrWebDavUrl) as Keyword;
            if (k == null)
            {
                Logger.Error(String.Format("Unable to GetKeyword With '{0}'", itemUriOrWebDavUrl));
                return null;
            }

            return new KeywordModel(_engine, k);
        }

        /// <summary>
        /// Gets a KeywordModel object.
        /// </summary>
        /// <param name="tcmUri">The TcmUri of the keyword to get.</param>
        /// <returns></returns>
        public dynamic GetKeyword(TcmUri tcmUri)
        {
            return GetKeyword(tcmUri.ToString());
        }

        public dynamic GetPage(TcmUri tcmUri)
        {
            return GetPage(tcmUri.ToString());
        }

        public dynamic GetPage(string itemUriOrWebDavUrl)
        {
            Page p = _engine.GetObject(itemUriOrWebDavUrl) as Page;

            if (p == null)
            {
                Logger.Error(String.Format("Unable To GetPage With '{0}'", itemUriOrWebDavUrl));
                return null;
            }

            return new PageModel(_engine, p);
        }

        public dynamic GetStructureGroup(TcmUri tcmUri)
        {
            return GetStructureGroup(tcmUri.ToString());
        }

        public dynamic GetStructureGroup(string itemUriOrWebDavUrl)
        {
            StructureGroup sg = _engine.GetObject(itemUriOrWebDavUrl) as StructureGroup;

            if (sg == null)
            {
                Logger.Error(String.Format("Unable To GetStructureGroup With '{0}'", itemUriOrWebDavUrl));
                return null;
            }

            return new StructureGroupModel(_engine, sg);
        }
    }
}
