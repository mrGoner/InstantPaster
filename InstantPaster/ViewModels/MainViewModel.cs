using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantPaster.ViewModels
{
    class MainViewModel
    {
        public List<HotKeyConfiguration> Configuration { get; set; } =
            new List<HotKeyConfiguration> {new HotKeyConfiguration()};

        public HotKeyConfiguration SelectedConfiguration { get; set; }
    }
}
