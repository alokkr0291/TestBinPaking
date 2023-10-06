using System;
using System.Collections.Generic;
using System.Text;

namespace MobackPacker
{
    public interface IBinPackAlgorithm
    {
        void Insert(IEnumerable<Cuboid> cuboids);
    }
}
