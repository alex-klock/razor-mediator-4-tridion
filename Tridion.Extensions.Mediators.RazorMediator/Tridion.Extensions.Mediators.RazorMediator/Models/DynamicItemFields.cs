using System;
using System.Collections.Generic;
using System.Linq;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.ContentManagement.Fields;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    public class DynamicItemFields : DynamicDictionary
    {
        /// <summary>
        /// The Tridion Templating Engine.
        /// </summary>
        private Engine _engine;

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
        /// <param name="engine">The Tridion Templating engine.</param>
        /// <param name="itemFields">The itemfields to make dynamic.</param>
        public DynamicItemFields(Engine engine, ItemFields itemFields)
        {
            _engine = engine;
            PopulateDynamicItemFields(itemFields);
        }

        /// <summary>
        /// Gets an array of field names.
        /// </summary>
        /// <returns></returns>
        public string[] GetFieldNames()
        {
            return _dictionary.Keys.ToArray();
        }

        /// <summary>
        /// Gets the underlying dictionary to allow iteration of fields.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetFields()
        {
            return _dictionary;
        }

        /// <summary>
        /// Chckes whether the itemfields has a field or not.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool HasField(string fieldName)
        {
            return _dictionary.ContainsKey(fieldName.ToLower());
        }

        /// <summary>
        /// Checks whether the ItemField has a value or not.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool HasValue(string fieldName)
        {
            if (_dictionary.ContainsKey(fieldName.ToLower()))
            {
                var value = _dictionary[fieldName.ToLower()];

                if (value != null)
                {
                    if (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(IList<>))
                    {
                        return ((List<object>)value).Count > 0;
                    }
                    
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to get a member of the instance.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();

            if (!_dictionary.ContainsKey(name))
            {
                Logger.Warning(String.Format("Key '{0}' Not Found In ItemFields", name));
                result = null;
                return true;
            }

            return _dictionary.TryGetValue(name, out result);
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
            string name = indexes[0].ToString().ToLower();

            if (!_dictionary.ContainsKey(name))
            {
                Logger.Warning(String.Format("Key '{0}' Not Found In ItemFields", name));
                result = null;
                return true;
            }

            return _dictionary.TryGetValue(name, out result);
        }

        /// <summary>
        /// Populates the dictionary with values from the item fields.
        /// </summary>
        /// <param name="itemFields">The Tridion itemfields to populate the dictionary with.</param>
        private void PopulateDynamicItemFields(ItemFields itemFields)
        {
            if (itemFields == null)
                return;

            foreach (ItemField itemField in itemFields)
            {
                string key = itemField.Name.ToLower();

                if (itemField is XhtmlField)
                {
                    XhtmlField xhtmlField = (XhtmlField)itemField;
                    if (xhtmlField.Definition.MaxOccurs == 1)
                        _dictionary[key] = TemplateUtilities.ResolveRichTextFieldXhtml(xhtmlField.Value);
                    else
                    {
                        List<string> values = new List<string>();
                        foreach (string value in xhtmlField.Values)
                        {
                            values.Add(TemplateUtilities.ResolveRichTextFieldXhtml(value));
                        }
                        _dictionary[key] = values;
                    }
                }
                else if (itemField is TextField)
                {
                    TextField textField = (TextField)itemField;
                    if (textField.Definition.MaxOccurs == 1)
                        _dictionary[key] = textField.Value;
                    else
                        _dictionary[key] = textField.Values;
                }
                else if (itemField is DateField)
                {
                    DateField dateField = (DateField)itemField;
                    if (dateField.Definition.MaxOccurs == 1)
                        _dictionary[key] = dateField.Value;
                    else
                        _dictionary[key] = dateField.Values;
                }
                else if (itemField is KeywordField)
                {
                    KeywordField keywordField = (KeywordField)itemField;
                    if (keywordField.Definition.MaxOccurs == 1)
                        if (keywordField.Value == null)
                            _dictionary[key] = null;
                        else
                            _dictionary[key] = new KeywordModel(_engine, keywordField.Value);
                    else
                    {
                        List<KeywordModel> keywords = new List<KeywordModel>();
                        int i = 0;
                        foreach (Keyword k in keywordField.Values)
                        {
                            var kw = new KeywordModel(_engine, k);
                            kw.Index = i++;
                            kw.IsLast = Index == keywordField.Values.Count - 1;
                            keywords.Add(kw);
                        }
                        _dictionary[key] = keywords;
                    }
                }
                else if (itemField is EmbeddedSchemaField)
                {
                    EmbeddedSchemaField embeddedSchemaField = (EmbeddedSchemaField)itemField;
                    if (embeddedSchemaField.Definition.MaxOccurs == 1)
                        if (embeddedSchemaField.Values.Count == 0)
                            _dictionary[key] = null;
                        else
                            _dictionary[key] = new DynamicItemFields(_engine, embeddedSchemaField.Value);
                    else
                    {
                        List<dynamic> embeddedFields = new List<dynamic>();

                        int i = 0;
                        foreach (ItemFields fields in embeddedSchemaField.Values)
                        {
                            var dif = new DynamicItemFields(_engine, fields);
                            dif.Index = i++;
                            dif.IsLast = dif.Index == embeddedSchemaField.Values.Count - 1;
                            embeddedFields.Add(dif);
                        }
                        _dictionary[key] = embeddedFields;
                    }
                }
                else if (itemField is ComponentLinkField)
                {
                    ComponentLinkField componentLinkField = (ComponentLinkField)itemField;
                    if (componentLinkField.Definition.MaxOccurs == 1)
                        if (componentLinkField.Value == null)
                            _dictionary[key] = null;
                        else
                            _dictionary[key] = new ComponentModel(_engine, componentLinkField.Value);
                    else
                    {
                        List<ComponentModel> components = new List<ComponentModel>();
                        int i = 0;
                        foreach (Component c in componentLinkField.Values)
                        {
                            var cm = new ComponentModel(_engine, c);
                            cm.Index = i++;
                            cm.IsLast = cm.Index == componentLinkField.Values.Count - 1;
                            components.Add(cm);
                        }
                        _dictionary[key] = components;
                    }
                }
                else if (itemField is ExternalLinkField)
                {
                    ExternalLinkField externalLink = (ExternalLinkField)itemField;
                    if (externalLink.Definition.MaxOccurs == 1)
                        _dictionary[key] = externalLink.Value;
                    else
                        _dictionary[key] = externalLink.Values;
                }
                else if (itemField is NumberField)
                {
                    NumberField numberField = (NumberField)itemField;
                    if (itemField.Definition.MaxOccurs == 1)
                        _dictionary[key] = numberField.Value;
                    else
                        _dictionary[key] = numberField.Values;
                }
                else
                {
                    _dictionary[key] = itemField.ToString();
                }
            }
        }
    }
}
