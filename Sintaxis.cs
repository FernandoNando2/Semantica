// Fernando Hernández Domínguez
using System;

namespace Semantica{
    public class Sintaxis : Lexico{
        public Sintaxis(){
            NextToken();
        }

        public Sintaxis(String ruta) : base(ruta) {
            NextToken();
        }

        public void match(String espera){
            if(espera == getContenido())
                NextToken();
            else
                throw new Error("Error de sintaxis linea " +linea +": Se espera un " +espera +".", log);
        }

        public void match(tipos espera){
            if(espera == getClasificacion())
                NextToken();
            else
                throw new Error("Error de sintaxis linea " +linea +": Se espera un " +espera +".", log);
        }
    }
}