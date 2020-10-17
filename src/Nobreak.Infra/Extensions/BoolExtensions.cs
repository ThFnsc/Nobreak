using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static class BoolExtensions
    {
        public static string SimNao(this bool input) =>
            input ? "Sim" : "Não";
    }
}
