using System;
using System.Collections.Generic;
using System.Linq;
using Tridion.ContentManager;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Dreamweaver;

namespace Tridion.Extensions.Mediators.Razor
{
    /// <summary>
    /// Razor Template Content Handler. Current piggy backs off of the Dreamwaever Content Handler.
    /// </summary>
    public class RazorContentHandler : AbstractTemplateContentHandler
    {
        /// <summary>
        /// Reference to the Tridion Dreamweaver Content Handler.
        /// </summary>
        private DreamweaverContentHandler _dwHandler;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateId"></param>
        public RazorContentHandler(TcmUri templateId) : base(templateId)
        {
            _dwHandler = new DreamweaverContentHandler(templateId);
        }

        /// <summary>
        /// Indicates the content type that this handler supports.
        /// </summary>
        public override AbstractTemplateContentHandler.RelevantContentTypes GetRelevantContentTypes()
        {
            return RelevantContentTypes.String;
        }

        /// <summary>
        /// Extarcts the references to Tridion items from the template content. It is allowed to have values in the result that are not valid Tridion references.
        /// </summary>
        /// <returns></returns>
        public override string[] PerformExtractReferences()
        {
            TemplatingLogger log = TemplatingLogger.GetLogger(this.GetType());

            string[] dwReferences = _dwHandler.PerformExtractReferences();

            RazorHandler handler = new RazorHandler(TemplateId.ToString(), WebDavUrl, Content);
            handler.Initialize();

            List<string> imports = handler.GetImportReferences();
            List<string> references = dwReferences.ToList();

            foreach (string path in imports)
            {
                if (!path.ToLower().StartsWith("tcm:") && !path.ToLower().StartsWith("/webdav/"))
                {
                    references.Add(GetRelativeImportPath(path));
                }
                else
                {
                    if (path.StartsWith("/webdav/"))
                    {
                        string[] pathParts = path.Split('/');
                        string[] webDavParts = WebDavUrl.Split('/');

                        if (pathParts[2] != webDavParts[2])
                        {
                            pathParts[2] = webDavParts[2];
                        }

                        references.Add(String.Join("/", pathParts));
                    }
                    else if (TcmUri.IsValid(path))
                    {
                        TcmUri uri = new TcmUri(path);
                        if (uri.PublicationId != TemplateId.PublicationId)
                        {
                            uri = new TcmUri(uri.ItemId, uri.ItemType, TemplateId.PublicationId);
                        }
                        references.Add(uri.ToString());
                    }
                }
            }

            return references.ToArray();
        }

        /// <summary>
        /// Replaces previous extracted references to Tridion items in the template.
        /// </summary>
        /// <param name="newReferences"></param>
        public override void PerformSubstituteReferences(string[] newReferences)
        {
            RazorHandler handler = new RazorHandler(TemplateId.ToString(), WebDavUrl, Content);
            handler.Initialize();
            List<string> references = handler.GetImportReferences();

            List<string> dwReferences = new List<string>();
            int count = newReferences.Length - references.Count;

            for (int i = 0; i < count; i++)
            {
                dwReferences.Add(newReferences[i]);
            }

            _dwHandler.PerformSubstituteReferences(dwReferences.ToArray());

            if (handler.Config.ImportSettings.ReplaceRelativePaths)
            {
                foreach (string path in references)
                {
                    if (!path.StartsWith("tcm:") && !path.StartsWith("/webdav/"))
                    {
                        Content = Content.Replace(path, GetRelativeImportPath(path));
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the content of the template is syntactically correct.
        /// </summary>
        public override void PerformValidateContent()
        {
            _dwHandler.PerformValidateContent();
            _dwHandler.ValidateContent();
            ValidateCompilation();
        }

        /// <summary>
        /// Compiles the template. If there are any compilation errors, an error will be thrown at save time.
        /// </summary>
        private void ValidateCompilation()
        {
            RazorHandler handler = new RazorHandler(TemplateId.ToString(), WebDavUrl, Content);
            handler.Initialize();
            handler.CompileOnly(DateTime.Now);
        }

        /// <summary>
        /// Gets the relative import path of an import.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetRelativeImportPath(string path)
        {
            List<string> templatePathParts = WebDavUrl.Split('/').ToList();
            if (WebDavUrl.Contains(".cshtml"))
            {
                templatePathParts.RemoveAt(templatePathParts.Count - 1);
            }

            string[] pathParts = path.Split('/');

            if (pathParts.Length == 1)
            {
                // If there's no directory separator, path is in the same directory as this template.
                templatePathParts.Add(path);
            }
            else
            {
                foreach (string part in pathParts)
                {
                    if (part.Trim().Length == 0 || part.Equals("."))
                    {
                        // Ignore?
                    }
                    else if (part.Equals(".."))
                    {
                        templatePathParts.RemoveAt(templatePathParts.Count - 1);
                    }
                    else
                    {
                        templatePathParts.Add(part);
                    }
                }
            }

            return String.Join("/", templatePathParts);
        }
    }
}
