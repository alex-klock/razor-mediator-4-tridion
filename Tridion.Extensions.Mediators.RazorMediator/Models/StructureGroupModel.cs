using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager;
using Tridion.ContentManager.ContentManagement.Fields;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    public class StructureGroupModel : AbstractRepositoryLocalObject<StructureGroup>
    {
        /// <summary>
        /// The child StructureGroups of this StructureGroup (non-recursive)
        /// </summary>
        private List<StructureGroupModel> _structureGroups;

        /// <summary>
        /// The child pages of the StructureGroup
        /// </summary>
        private List<PageModel> _pages;

        /// <summary>
        /// Constructur
        /// </summary>
        /// <param name="sg">The Tridion StructureGroup instance.</param>
        public StructureGroupModel(Engine engine, StructureGroup sg) : base(engine, sg)
        {

        }

        /// <summary>
        /// Gets whether or not this is the root StructureGroup in a publication.
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return _tridionObject.IsRootOrganizationalItem;
            }
        }

        /// <summary>
        /// Gets all the Pages that are contained within this StructureGroup.
        /// </summary>
        public List<PageModel> Pages
        {
            get
            {
                if (_pages == null)
                {
                    _pages = new List<PageModel>();

                    OrganizationalItemItemsFilter pageFilter = new OrganizationalItemItemsFilter(_tridionObject.Session)
                    {
                        ItemTypes = new List<ItemType> { ItemType.Page },
                        Recursive = false
                    };

                    foreach (Page page in _tridionObject.GetItems(pageFilter))
                    {
                        _pages.Add(new PageModel(_engine, page));
                    }
                }
                return _pages;
            }
        }

        /// <summary>
        /// Gets the parent StructureGroup (if available). Returns null if this is the root item.
        /// </summary>
        public StructureGroupModel Parent
        {
            get
            {
                return OrganizationalItem;
            }
        }

        /// <summary>
        /// Gets all the children StructureGroup's of this StructureGroup (non-recursive).
        /// </summary>
        public List<StructureGroupModel> StructureGroups
        {
            get
            {
                if (_structureGroups == null)
                {
                    _structureGroups = new List<StructureGroupModel>();

                    OrganizationalItemItemsFilter sgFilter = new OrganizationalItemItemsFilter(_tridionObject.Session)
                    {
                        ItemTypes = new List<ItemType> { ItemType.StructureGroup },
                        Recursive = false
                    };

                    foreach (StructureGroup sg in _tridionObject.GetItems(sgFilter))
                    {
                        _structureGroups.Add(new StructureGroupModel(_engine, sg));
                    }
                }
                return _structureGroups;
            }
        }
    }
}
