﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    public class KeywordModel : AbstractRepositoryLocalObject<Keyword>
    {
        /// <summary>
        /// Gets the position (when in a list).
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Gets whether or not this is the first item (when in a list).
        /// </summary>
        public bool IsFirst
        {
            get
            {
                return Index == 0;
            }
        }

        /// <summary>
        /// Gets or sets whether or not this is the last item (when in a list);
        /// </summary>
        public bool IsLast
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">The Tridion Templating Engine.</param>
        /// <param name="keyword">The Tridion folder object.</param>
        public KeywordModel(Engine engine, Keyword keyword) : base(engine, keyword)
        {

        }

        /// <summary>
        /// Gets a description of the Keyword.
        /// </summary>
        public string Description
        {
            get
            {
                return _tridionObject.Description;
            }
        }

        /// <summary>
        /// Gets whether or not the Keyword is abstract.
        /// </summary>
        public bool IsAbstract
        {
            get
            {
                return _tridionObject.IsAbstract;
            }
        }

        /// <summary>
        /// Gets whether the Keyword is a root Keyword (no parent Keywords).
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return _tridionObject.IsRoot;
            }
        }

        /// <summary>
        /// Gets the custom key for the Keyword.
        /// </summary>
        public string Key
        {
            get
            {
                return _tridionObject.Key;
            }
        }
    }
}