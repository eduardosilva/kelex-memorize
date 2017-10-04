using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kelex_memorize.Commands
{
    public interface IKelexCommand
    {
        string Code { get; }
        string Description { get;  }
        IEnumerable<IKelexCommandParameter> Parameters { get;  }

        void Execute(string[] args);
    }
}
