using LedBlinker.LedToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedBlinker.LedToolkit.Tools.Impl
{
    public interface ILogStateTool
    {

        public Task<List<Logs>> LoadLogsAsync(DateTime? from, DateTime? to);
    }
}
