﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimlerConfig.Components.Models
{
    public interface ICopyable<T>
    {
        public T Clone();
    }

}
