using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedBlinker.LedToolkit.Tools.Impl
{
    public  interface IConfigurationStateTool
    {
        public Task<float?> LoadConfigurationState();
    }
}
