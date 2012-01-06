using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement.Fields;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    public class PublicationModel
    {
        private Engine _engine;

        private dynamic _fields;

        private Publication _publication;

        private FolderModel _rootFolder;

        private StructureGroupModel _rootStructureGroup;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="publication"></param>
        public PublicationModel(Engine engine, Publication publication)
        {
            _engine = engine;
            _publication = publication;
        }

        /// <summary>
        /// Gets the Publication's metadata fields.
        /// </summary>
        public dynamic Fields
        {
            get
            {
                if (_fields == null)
                {
                    if (_publication.MetadataSchema == null)
                    {
                        _fields = new DynamicItemFields(_engine, null);
                    }
                    else
                    {
                        ItemFields itemFields = new ItemFields(_publication.Metadata, _publication.MetadataSchema);
                        _fields = new DynamicItemFields(_engine, itemFields);
                    }
                }

                return _fields;
            }
        }

        /// <summary>
        /// Gets the tcm uri of the Publication.
        /// </summary>
        public TcmUri ID
        {
            get { return _publication.Id; }
        }

        /// <summary>
        /// Gets the tcm uri of the Publication.
        /// </summary>
        public TcmUri Id
        {
            get { return _publication.Id; }
        }

        /// <summary>
        /// Gets the Publication's metadata fields.
        /// </summary>
        public dynamic MetaData
        {
            get { return Fields; }
        }

        /// <summary>
        /// Gets the Publication's metadata fields.
        /// </summary>
        public dynamic Metadata
        {
            get { return Fields; }
        }

        /// <summary>
        /// Gets the path to the directory containing published binaries.
        /// </summary>
        public string MultimediaPath
        {
            get { return _publication.MultimediaPath; }
        }

        /// <summary>
        /// Gets the url to the directory containing published binaries.
        /// </summary>
        public string MultimediaUrl
        {
            get { return _publication.MultimediaUrl; }
        }

        /// <summary>
        /// Gets the publication/publish path.
        /// </summary>
        public string PublicationPath
        {
            get
            {
                return _publication.PublicationPath;
            }
        }

        /// <summary>
        /// Gets the publication/publish URL.
        /// </summary>
        public string PublicationUrl
        {
            get
            {
                return _publication.PublicationUrl;
            }
        }

        /// <summary>
        /// Gets the Publication's root Folder.
        /// </summary>
        public FolderModel RootFolder
        {
            get
            {
                if (_rootFolder == null)
                {
                    _rootFolder = new FolderModel(_engine, _publication.RootFolder);
                }
                return _rootFolder;
            }
        }

        /// <summary>
        /// Gets the Publication's root Structure Group.
        /// </summary>
        public StructureGroupModel RootStructureGroup
        {
            get
            {
                if (_rootStructureGroup == null)
                {
                    _rootStructureGroup = new StructureGroupModel(_engine, _publication.RootStructureGroup);
                }

                return _rootStructureGroup;
            }
        }

        /// <summary>
        /// Gets the Publication's title.
        /// </summary>
        public string Title
        {
            get 
            { 
                return _publication.Title; 
            }
        }

        /// <summary>
        /// Gets the WebDAV URL of the publication.
        /// </summary>
        public string WebDavUrl
        {
            get
            {
                return _publication.WebDavUrl;
            }
        }

        /// <summary>
        /// Returns the Publication's Title.
        /// </summary>
        public override string ToString()
        {
            return _publication.Title;
        }
    }
}
