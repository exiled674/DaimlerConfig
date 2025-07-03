using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfig.Components.Models
{
    public interface ICopyable<T>
    {
        public T Clone();
    }

}
