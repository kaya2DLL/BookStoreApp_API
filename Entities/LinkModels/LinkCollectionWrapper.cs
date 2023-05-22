using Entities.LinkModels;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class LinkCollectionWrapper<T> : LinkResourceBase
    {
        public List<T> Value { get; set; } = new List<T>();

        public LinkCollectionWrapper(List<T> value)
        {
            Value = value;
        }
        public LinkCollectionWrapper()
        {

        }
    }


