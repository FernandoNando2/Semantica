﻿using System;

namespace Semantica{
    public class Program{

        static void Main(string[] args){

            try{
                Lenguaje a = new Lenguaje("C:\\Users\\Fernando Hernandez\\Desktop\\ITQ\\5to Semestre\\Lenguajes y Automatas II\\Semantica\\prueba.cpp");
                a.Programa();
                /*while(!a.FinArchivo()){
                    a.NextToken();
                }*/
                a.Cerrar();
            }
            catch(Exception e){
                Console.WriteLine(e.Message);
            }
        }
    }
}