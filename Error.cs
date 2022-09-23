// Fernando Hernández Domínguez
using System;

namespace Semantica{
    public class Error : Exception{
        public Error(String mensaje, StreamWriter log) : base(mensaje) {
            log.WriteLine(mensaje);
        }
    }
}