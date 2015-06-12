using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DiversityService.API.WebHost
{
    public class CollectionServerConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("publicServers")]
        public PublicServersElement PublicServers
        {
            get
            {
                return (PublicServersElement)this["publicServers"];
            }
        }

        [ConfigurationProperty("servers", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServersCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ServersCollection Servers
        {
            get
            {
                ServersCollection urlsCollection =
                    (ServersCollection)base["servers"];
                return urlsCollection;
            }
        }
    }

    public class PublicServersElement : ConfigurationElement
    {
        [ConfigurationProperty("taxa")]
        public ServerLoginElement Taxa
        {
            get
            {
                return (ServerLoginElement)this["taxa"];
            }
        }

        [ConfigurationProperty("terms")]
        public ServerLoginCatalogElement Terms
        {
            get
            {
                return (ServerLoginCatalogElement)this["terms"];
            }
        }
    }

    public class ServersCollection : ConfigurationElementCollection
    {
        public ServersCollection()
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CollectionServerElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((CollectionServerElement)element).Name;
        }

        public CollectionServerElement this[int index]
        {
            get
            {
                return (CollectionServerElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public CollectionServerElement this[string Name]
        {
            get
            {
                return (CollectionServerElement)BaseGet(Name);
            }
        }

        public int IndexOf(CollectionServerElement url)
        {
            return BaseIndexOf(url);
        }

        public void Add(CollectionServerElement url)
        {
            BaseAdd(url);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(CollectionServerElement url)
        {
            if (BaseIndexOf(url) >= 0)
                BaseRemove(url.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }

    public class ServerElement : ConfigurationElement
    {
        [ConfigurationProperty("address", DefaultValue = "127.0.0.1", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Address
        {
            get
            { return (string)this["address"]; }
            set
            { this["address"] = value; }
        }

        [ConfigurationProperty("port", DefaultValue = "1433", IsRequired = true)]
        [IntegerValidator(MinValue = 1024, MaxValue = Int32.MaxValue)]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }
            set
            {
                this["port"] = value;
            }
        }
    }

    public class ServerLoginElement : ServerElement
    {
        [ConfigurationProperty("user", DefaultValue = "anon", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string User
        {
            get
            { return (string)this["user"]; }
            set
            { this["user"] = value; }
        }

        [ConfigurationProperty("password", DefaultValue = "opensesame", IsRequired = true)]
        [StringValidator(InvalidCharacters = "", MinLength = 1, MaxLength = 60)]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }
    }

    public class ServerLoginCatalogElement : ServerLoginElement
    {
        [ConfigurationProperty("catalog", DefaultValue = "DiversityCollection_Test", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Catalog
        {
            get
            { return (string)this["catalog"]; }
            set
            { this["catalog"] = value; }
        }
    }

    public class CollectionServerElement : ServerElement
    {
        [ConfigurationProperty("id", DefaultValue = "0", IsRequired = true)]
        [IntegerValidator(MinValue = 0, MaxValue = Int32.MaxValue)]
        public int Id
        {
            get
            {
                return (int)this["id"];
            }
            set
            {
                this["id"] = value;
            }
        }

        [ConfigurationProperty("name", DefaultValue = "Collection", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 5, MaxLength = 60)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("catalog", DefaultValue = "DiversityCollection_Test", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Catalog
        {
            get
            { return (string)this["catalog"]; }
            set
            { this["catalog"] = value; }
        }
    }
}