using System.Collections.Generic;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    /// <summary>
    /// Represents a Tridion Page
    /// </summary>
    public class PageModel : AbstractRepositoryLocalObject<Page>
    {
        /// <summary>
        /// The Page's component presentations.
        /// </summary>
        private List<ComponentPresentationModel> _componentPresentations;

        /// <summary>
        /// The list of full Structure Group ancestry.
        /// </summary>
        private List<StructureGroupModel> _structureGroupAncestry;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="page"></param>
        public PageModel(Engine engine, Page page) : base(engine, page)
        {

        }

        /// <summary>
        /// Gets the Page's Component Presentations.
        /// </summary>
        public List<ComponentPresentationModel> ComponentPresentations
        {
            get
            {
                if (_componentPresentations == null)
                {
                    _componentPresentations = new List<ComponentPresentationModel>();
                    int i = 0;
                    foreach (Tridion.ContentManager.CommunicationManagement.ComponentPresentation cp in _tridionObject.ComponentPresentations)
                    {
                        var cpm = new ComponentPresentationModel(_engine, cp.Component, cp.ComponentTemplate);
                        cpm.Index = i++;
                        cpm.IsLast = cpm.Index == _tridionObject.ComponentPresentations.Count - 1;
                        
                        _componentPresentations.Add(cpm);
                    }
                }
                
                return _componentPresentations;
            }
        }

        /// <summary>
        /// Gets the Page's filename.
        /// </summary>
        public string FileName
        {
            get
            {
                return _tridionObject.FileName;
            }
        }

        /// <summary>
        /// Gets the Page's StructureGroup.
        /// </summary>
        public StructureGroupModel StructureGroup
        {
            get { return OrganizationalItem; }
        }

        /// <summary>
        /// Gets a list of the full Structure Group ancestry for the page.  The first item in the list is the root, while the last is the Page's SG.
        /// </summary>
        /// <remarks>
        /// Thanks Spencer Ludban for idea and code.
        /// </remarks>
        public List<StructureGroupModel> StructureGroupAncestry
        {
            get
            {
                if (_structureGroupAncestry == null)
                {
                    _structureGroupAncestry = new List<StructureGroupModel>();
                    StructureGroupModel sg = StructureGroup;
                    _structureGroupAncestry.Add(sg);
                    while (!sg.IsRoot)
                    {
                        _structureGroupAncestry.Add(sg.Parent);
                        sg = sg.Parent;
                    }
                    _structureGroupAncestry.Reverse();
                }

                return _structureGroupAncestry;
            }
        }

        /// <summary>
        /// Gets the publish path for the Page, inluding the filename and extension.
        /// </summary>
        public string PublishLocationPath
        {
            get { return _tridionObject.PublishLocationPath; }
        }

        /// <summary>
        /// Gets the publish URL for the Page.
        /// </summary>
        public string PublishLocationUrl
        {
            get { return _tridionObject.PublishLocationUrl; }
        }

        /// <summary>
        /// Gets the publish path of the page, excluding filename and extension.
        /// </summary>
        public string PublishPath
        {
            get { return _tridionObject.PublishPath; }
        }
    }
}
