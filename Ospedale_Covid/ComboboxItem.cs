using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ospedale_Covid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Gestionale
    {
        class ComboboxItem
        {
            public object Value { get; set; }
            public string Text { get; set; }
            public ComboboxItem(string t, object v)
            {
                this.Text = t;
                this.Value = v;
            }
        }
    }
}
