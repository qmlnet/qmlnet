using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qt.NetCore
{
    public interface ITypeCreator
    {
        object Create(Type type);
    }
}
