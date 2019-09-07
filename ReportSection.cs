using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Lazyman
{

    public class ReportSection : ConfigurationSection
    {
        [ConfigurationProperty("reports", IsRequired = true)]
        [ConfigurationCollection(typeof(ReportElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public GenericConfigurationElementCollection<ReportElement> ReportElementCollection
        {
            get { return (GenericConfigurationElementCollection<ReportElement>)this["reports"]; }
        }

    }


    public class GenericConfigurationElementCollection<T> : ConfigurationElementCollection, IEnumerable<T> where T : ConfigurationElement, new()
    {
        List<T> _elements = new List<T>();

        protected override ConfigurationElement CreateNewElement()
        {
            T newElement = new T();
            _elements.Add(newElement);
            return newElement;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return _elements.Find(e => e.Equals(element));
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }


    public class ReportElement : ConfigurationElement
    {
        //queryFile="BankingDetailsQuery.sql" frequency="Weekly"
        [ConfigurationProperty("reportName", IsRequired = true)]
        public string ReportName
        {
            get
            {
                return (string)this["reportName"];
            }
            set
            {
                this["reportName"] = value;
            }
        }


        //queryFile="BankingDetailsQuery.sql" frequency="Weekly"
        [ConfigurationProperty("templateName", IsRequired = true)]
        public string TemplateName
        {
            get
            {
                return (string)this["templateName"];
            }
            set
            {
                this["templateName"] = value;
            }
        }
        [ConfigurationProperty("sqlScript", IsRequired = true)]
        public string SqlScript
        {
            get
            {
                return (string)this["sqlScript"];
            }
            set
            {
                this["sqlScript"] = value;
            }
        }

        [ConfigurationProperty("sqlParameters", IsRequired = false)]
        public string SqlParameters
        {
            get
            {
                return (string)this["sqlParameters"];
            }
            set
            {
                this["sqlParameters"] = value;
            }
        }


        [ConfigurationProperty("parameterNames", IsRequired = false)]
        public string ParameterNames
        {
            get
            {
                return (string)this["parameterNames"];
            }
            set
            {
                this["parameterNames"] = value;
            }
        }

        [ConfigurationProperty("dbList", IsRequired = true)]
        public string DbList
        {
            get
            {
                return (string)this["dbList"];
            }
            set
            {
                this["dbList"] = value;
            }
        }

    }
}
