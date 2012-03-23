using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Tridion.ContentManager;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Dreamweaver;
using Tridion.Extensions.Mediators.Razor.Configuration;
using Tridion.Extensions.Mediators.Razor.Templating;

namespace Tridion.Extensions.Mediators.Razor
{
    /// <summary>
    /// Razor Template Content Handler. Current piggy backs off of the Dreamwaever Content Handler.
    /// </summary>
    public class RazorContentHandler : AbstractTemplateContentHandler
    {
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
            return _dwHandler.PerformExtractReferences();
        }

        /// <summary>
        /// Replaces previous extracted references to Tridion items in the template.
        /// </summary>
        /// <param name="newReferences"></param>
        public override void PerformSubstituteReferences(string[] newReferences)
        {
            _dwHandler.PerformSubstituteReferences(newReferences);
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
            handler.Compile(DateTime.Now);
        }
    }
}
