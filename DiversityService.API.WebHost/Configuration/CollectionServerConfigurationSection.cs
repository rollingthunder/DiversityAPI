namespace DiversityService.API.WebHost
{
    using System;
    using System.Configuration;

    public class CollectionServerConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("publicServers", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServerLoginCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ServerLoginCollection PublicServers
        {
            get
            {
                return (ServerLoginCollection)this["publicServers"];
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

    public class CollectionServerElement : ServerElement
    {
        [ConfigurationProperty("catalog", DefaultValue = "DiversityCollection_Test", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Catalog
        {
            get
            {
                return (string)this["catalog"];
            }

            set
            {
                this["catalog"] = value;
            }
        }

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
    }

    public class ServerElement : ConfigurationElement
    {
        [ConfigurationProperty("address", DefaultValue = "127.0.0.1", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Address
        {
            get
            {
                return (string)this["address"];
            }

            set
            {
                this["address"] = value;
            }
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

    public class ServerLoginCatalogElement : ServerElement
    {
        [ConfigurationProperty("catalog", DefaultValue = "", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Catalog
        {
            get
            {
                return (string)this["catalog"];
            }

            set
            {
                this["catalog"] = value;
            }
        }

        [ConfigurationProperty("kind", DefaultValue = "taxa", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 5, MaxLength = 60)]
        public string Kind
        {
            get
            {
                return (string)this["kind"];
            }

            set
            {
                this["kind"] = value;
            }
        }

        [ConfigurationProperty("name", DefaultValue = "taxa", IsRequired = true)]
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

        [ConfigurationProperty("user", DefaultValue = "anon", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string User
        {
            get
            {
                return (string)this["user"];
            }

            set
            {
                this["user"] = value;
            }
        }
    }

    public class ServerLoginCollection : ConfigurationElementCollection<ServerLoginCatalogElement>
    {
        public ServerLoginCollection()
            : base(x => x.Name)
        {
        }
    }

    public class ServersCollection : ConfigurationElementCollection<CollectionServerElement>
    {
        public ServersCollection()
            : base(x => x.Name)
        {
        }
    }
}