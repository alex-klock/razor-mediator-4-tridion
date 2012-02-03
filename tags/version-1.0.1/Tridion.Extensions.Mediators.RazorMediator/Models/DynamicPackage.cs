using System;
using System.Collections.Generic;
using System.Dynamic;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    /// <summary>
    /// Represents Package variables.
    /// </summary>
    public class DynamicPackage : DynamicDictionary
    {
        private Engine _engine;

        private Package _package;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="package"></param>
        public DynamicPackage(Engine engine, Package package)
        {
            _engine = engine;
            _package = package;
        }

        /// <summary>
        /// Attempts to get a property from the dynamic object.
        /// </summary>
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            
            if (!_dictionary.ContainsKey(name))
            {
                Item item = _package.GetByName(binder.Name);
                if (item == null)
                {
                    result = String.Empty;
                    return false;
                }

                AddPackageItem(item, name);
            }

            return _dictionary.TryGetValue(name, out result);
        }

        /// <summary>
        /// Add's the package item to the dynamic dictionary.
        /// </summary>
        /// <param name="item">The item being added.</param>
        /// <param name="name">The name of the package item.</param>
        private void AddPackageItem(Item item, string name)
        {
            if (item.ContentType == ContentType.Component)
            {
                Component component = _engine.GetObject(item.GetAsSource().GetValue("ID")) as Component;
                _dictionary[name] = new ComponentModel(_engine, component);
            }
            else if (item.ContentType == ContentType.ComponentArray)
            {
                List<ComponentModel> components = new List<ComponentModel>();
                foreach (ISource source in item.GetSources("Component"))
                {
                    Component component = _engine.GetObject(source.GetValue("ID")) as Component;
                    components.Add(new ComponentModel(_engine, component));
                }
                _dictionary[name] = components;
            }
            else if (item.ContentType == ContentType.ComponentPresentationArray)
            {
                List<ComponentPresentationModel> presentations = new List<ComponentPresentationModel>();
                IComponentPresentationList componentPresentations = ComponentPresentationList.FromXml(item.GetAsString());
                foreach (ComponentPresentation cp in componentPresentations)
                {
                    presentations.Add(new ComponentPresentationModel(_engine, cp.ComponentUri, cp.TemplateUri));
                }
                _dictionary[name] = presentations;
            }
            else
            {
                _dictionary[name] = item.GetAsString();
            }
        }
    }
}