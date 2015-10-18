namespace DiversityService.API.WebHost
{
    using System;
    using System.Configuration;

    public class ConfigurationElementCollection<T> : ConfigurationElementCollection
        where T : ConfigurationElement, new()
    {
        private readonly Func<T, string> keyAccessor;

        public ConfigurationElementCollection(Func<T, string> keyAccessor)
        {
            this.keyAccessor = keyAccessor;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public T this[int index]
        {
            get
            {
                return (T)BaseGet(index);
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

        new public T this[string Name]
        {
            get
            {
                return (T)BaseGet(Name);
            }
        }

        public void Add(T url)
        {
            BaseAdd(url);
        }

        public void Clear()
        {
            BaseClear();
        }

        public int IndexOf(T url)
        {
            return BaseIndexOf(url);
        }

        public void Remove(T x)
        {
            if (BaseIndexOf(x) >= 0)
            {
                BaseRemove(keyAccessor(x));
            }
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return keyAccessor((T)element);
        }
    }
}