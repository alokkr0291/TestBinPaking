using System;
using System.Collections.Generic;
using System.Text;

namespace MobackPacker
{
    public interface IBinPacker
    {
        BinPackResult Pack(BinPackParameter parameter);
    }
}
