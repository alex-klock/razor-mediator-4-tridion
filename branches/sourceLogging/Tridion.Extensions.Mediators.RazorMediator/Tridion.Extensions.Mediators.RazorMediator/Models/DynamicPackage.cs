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
            Item item = _package.GetByName(binder.Name);
            if (item == null)
            {
                result = null;
                // DominicCronin: Not sure if this return value is intentional.... leaving it alone for now.
                return true;
            }

            result = GetDynamicItemFromTridionPackageItem(item);
            return true;
        }

        /// <summary>
        /// Attempts to get an index of the instance.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetIndex(System.Dynamic.GetIndexBinder binder, object[] indexes, out object result)
        {
            string name = indexes[0].ToString();

            Item item = _package.GetByName(name);
            if (item == null)
            {
                result = null;
                // DominicCronin: Not sure if this return value is intentional.... leaving it alone for now.
                return true;
            }

            result = GetDynamicItemFromTridionPackageItem(item);
            return true;
        }

        /// <summary>
        /// Gets an Item from the package by its name.
        /// </summary>
        /// <param name="name">The name of the item to get.</param>
        /// <returns>A Tridion Item instance.</returns>
        public Item GetByName(string name)
        {
            return _package.GetByName(name);
        }

        /// <summary>
        /// Gets an Item from the package by its type.
        /// </summary>
        /// <param name="type">The ContentType to retrieve.</param>
        /// <returns>A Tridion Item instance.</returns>
        public Item GetByType(ContentType type)
        {
            return _package.GetByType(type);
        }

        /// <summary>
        /// Gets a value from the Package.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name.</param>
        /// <returns>A value from the package.</returns>
        public string GetValue(string fullyQualifiedName)
        {
            return _package.GetValue(fullyQualifiedName);
        }

        /// <summary>
        /// Add's the package item to the dynamic dictionary.
        /// </summary>
        /// <param name="item">The item being added.</param>
        /// <param name="nameissomethingverererelong">The name of the package item.</param>
        private dynamic GetDynamicItemFromTridionPackageItem(Item item)
        {
            if (item.ContentType == ContentType.Component)
            {
                Component component = _engine.GetObject(item.GetAsSource().GetValue("ID")) as Component;
                return new ComponentModel(_engine, component);
            }
            else if (item.ContentType == ContentType.ComponentArray)
            {
                List<ComponentModel> components = new List<ComponentModel>();
                IComponentPresentationList componentPresentations = ComponentPresentationList.FromXml(item.GetAsString());
                foreach (ComponentPresentation cp in componentPresentations)
                {
                    Component component = _engine.GetObject(cp.ComponentUri) as Component;
                    components.Add(new ComponentModel(_engine, component));
                }
                return components;
            }
            else if (item.ContentType == ContentType.ComponentPresentationArray)
            {
                List<ComponentPresentationModel> presentations = new List<ComponentPresentationModel>();
                IComponentPresentationList componentPresentations = ComponentPresentationList.FromXml(item.GetAsString());
                foreach (ComponentPresentation cp in componentPresentations)
                {
                    presentations.Add(new ComponentPresentationModel(_engine, cp.ComponentUri, cp.TemplateUri));
                }
                return presentations;
            }
            else
            {
                return item.GetAsString();
            }
        }
    }
}