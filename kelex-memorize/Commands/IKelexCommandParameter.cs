﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kelex_memorize.Commands
{
    public interface IKelexCommandParameter
    {
        string Key { get; set; }
        string Description { get; set; }
        bool Required { get; set; }
        string ErrorRequiredMessage { get; }
        bool NotRequiredAValue { get; set; }
        bool Declared { get; set; }
        string Value { get; set; }
    }

    public class KelexCommandParameter : IKelexCommandParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public string ErrorRequiredMessage { get; set; }

        //boolean types
        public bool NotRequiredAValue { get; set; }
        public bool Declared { get; set; }
    }
}
