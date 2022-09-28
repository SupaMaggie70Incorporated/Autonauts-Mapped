using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupaMaggie70
{
    public static class TASMain
    {
        public static InputFaker inputFaker
        {
            get
            {
                return InputFaker.Instance;
            }
        }
    }
}
