// Fernando Hernández Domínguez
using System;
/*
Requerimiento 1: Actualizar el dominante para variables en la expresión. ya
    Ejemplo: float x, char y, y = x
Requerimiento 2: Actualizar el dominante para el casteo. ya
Requerimiento 3: Programar un metodo de conversion de un valor a un tipo de dato. ya
    private float convertir(float valor, string tipo_dato)
    Deberan usar el residuo de la division %255, *65535
Requerimeinto 4: Evaluar nuevamente la condicion del if, while, for, do while con respecto al parametro que recibe. ya
Requerimiento 5: Levantar una Excepcion cuando la captura no sea un numero (Scanf) ya
Requerimiento 6: Ejecutar el For()
*/
namespace Semantica{
    public class Lenguaje : Sintaxis{
        List <Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.tipoDato dominante;
        public Lenguaje(){

        }

        public Lenguaje(String ruta) : base(ruta) {

        }

        public void addVariable(String nombre, Variable.tipoDato tipo){
            variables.Add(new Variable(nombre,tipo));
        }

        private void setPosicion(long posicion){
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(posicion, SeekOrigin.Begin);
        }

        public void displayVariables(){
            log.WriteLine("\nVariables: ");
            foreach (Variable v in variables)
                log.WriteLine(v.getNombre() +" " +v.getTipo() +" " +v.getValor());
        }

        private bool existeVariable(string nombre){
            foreach(Variable v in variables){
                if(v.getNombre().Equals(nombre))
                    return true;
            }
            return false;
        }

        private void modificaValor(string nombre, float nvalor){
            foreach(Variable v in variables){
                if(nombre == v.getNombre())
                    v.setValor(nvalor);
            }
        }

        private float getValor(string nombre){
            foreach(Variable v in variables){
                if(v.getNombre().Equals(nombre))
                    return v.getValor();
            }
            return 0;
        }

        private Variable.tipoDato getTipo(string nombre){
            foreach(Variable v in variables){
                if(v.getNombre().Equals(nombre))
                    return v.getTipo();
            }
            return 0;
        }

    // Programar un metodo de conversion de un valor a un tipo de dato.
    // Requerimiento 3
    private float convertir(float valor, Variable.tipoDato tipoDato){
        if(tipoDato == Variable.tipoDato.Char)
            return (char)(valor % 256);
        else if(tipoDato == Variable.tipoDato.Int)
            return (int)(valor % 65535);
        else if(tipoDato == Variable.tipoDato.Float)
            return valor;
        return 0;
    }

        //Programa	-> 	Librerias? Variables? Main
        public void Programa(){
            Librerias();
            Variables();
            Main();
            displayVariables();
        }

        //Librerias	->	#include<identificador(.h)?> Librerias?
        private void Librerias(){
            if (getContenido() == "#"){
                match("#");
                match("include");
                match("<");
                match(tipos.identificador);
                if (getContenido() == "."){
                    match(".");
                    match("h");
                }
                match(">");
                Librerias();
            }
        }

        // Variables -> tipo_dato Lista_identificadores ; Variables?
        private void Variables(){
            if(getClasificacion() == tipos.tipo_datos){
                Variable.tipoDato tipo = Variable.tipoDato.Char;
                switch(getContenido()){
                    case "int":
                        tipo = Variable.tipoDato.Int;
                        break;
                    case "float":
                        tipo = Variable.tipoDato.Float;
                        break;
                }
                match(tipos.tipo_datos);
                listaIdentificadores(tipo);
                match(tipos.fin_sentencia);
                Variables();
            }
        }

        // Lista_identificadores -> identificador (,Lista_identificadores)? 
        private void listaIdentificadores(Variable.tipoDato tipo){
            if(getClasificacion() == tipos.identificador){                
                if(!existeVariable(getContenido()))
                    addVariable(getContenido(),tipo);
                else
                    throw new Error("Error de sintaxis, variable <" + getContenido() +"> duplicada en la linea: "+linea, log);
            }
            match(tipos.identificador);
            if(getContenido() == ","){
                match(",");
                listaIdentificadores(tipo);
            }
        }
        
        // Main -> void main() Bloque de instrucciones
        private void Main(){
            match("void");
            match("main");
            match("(");
            match(")");
            bloqueInstrucciones(true);
        }

        // Bloque de instrucciones -> {Lista de instrucciones?}
        private void bloqueInstrucciones(bool evaluacion){
            match("{");
            if(getContenido() != "}")
                listaInstrucciones(evaluacion);
            match("}");
        }

        //listainstrucciones -> instruccion listadeinstrucciones?
        private void listaInstrucciones(bool evaluacion)
        {
            instruccion(evaluacion);
            if (getContenido() != "}")
                listaInstrucciones(evaluacion);
        }

        //listainstruccionesCase -> instruccion listaInstruccionesCase?
        private void listaInstruccionesCase(bool evaluacion)
        {
            instruccion(evaluacion);
            if (getContenido() != "case" && getContenido() != "default" && getContenido() != "}" && getContenido() != "break")
                listaInstruccionesCase(evaluacion);
        }

        // Instruccion -> Printf | Scanf | If | While | do while | Asignacion
        private void instruccion(bool evaluacion){
            if(getContenido() == "printf")
                Printf(evaluacion);
            else if (getContenido() == "scanf")
                Scanf(evaluacion);
            else if (getContenido() == "if") 
                If(evaluacion);
            else if (getContenido() == "while")
                While(evaluacion);
            else if (getContenido() == "do")
                Do(evaluacion);
            else if (getContenido() == "for")
                For(evaluacion);
            else if (getContenido() == "switch")
                Switch(evaluacion);
            else
                Asignacion(evaluacion);
        }

        private Variable.tipoDato evaluaNumero(float resultado){
            if(resultado % 1 != 0)
                return Variable.tipoDato.Float;
            else if(resultado <= 255)
                return Variable.tipoDato.Char;
            else if (resultado <= 65535)
                return Variable.tipoDato.Int;
            return Variable.tipoDato.Float;
        }

        private bool evaluaSemantica( string variable, float resultado){
            Variable.tipoDato tipoDato = getTipo(variable);
            return false;
        }

        // Asignacion -> identificador = Expresion ;
        private void Asignacion(bool evaluacion){
            log.WriteLine();
            log.Write(getContenido() + " = " );
            string nombre = getContenido();
            if(!existeVariable(getContenido()))
                throw new Error("Error de sintaxis, variable <" + getContenido() +"> no existe en el contexto actual, linea: "+linea, log);
            match(tipos.identificador);
            match("=");
            dominante = Variable.tipoDato.Char;
            Expresion();
            match(";");
            float resultado = stack.Pop();
            log.Write("= " +resultado);
            log.WriteLine();
            if(dominante < evaluaNumero(resultado))
                dominante = evaluaNumero(resultado);
            if(dominante <= getTipo(nombre)){
                if(evaluacion)
                    modificaValor(nombre, resultado);
            }
            else
                throw new Error("Error de semantica, no podemos asignar un <" +dominante +"> a un <" +getTipo(nombre) +"> en linea: " +linea, log);
        }

        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool evaluacion){
            bool validaWhile;
            match("while");
            match("(");
            if(evaluacion)
                validaWhile = Condicion();
            else{
                Condicion();
                validaWhile = false;
            }
            // Requerimiento 4
            match(")");
            if(getContenido() == "{")
                bloqueInstrucciones(validaWhile);
            else
                instruccion(validaWhile);
        }

        //Do -> do bloqueInstrucciones | instruccion while(Condicion);
        private void Do(bool evaluacion){
            bool validaDo = !evaluacion;
            match("do");
            if(getContenido() == "{")
                bloqueInstrucciones(validaDo);
            else
                instruccion(validaDo);
            match("while");
            match("(");
            if(evaluacion)
                validaDo = Condicion();
            else{
                Condicion();
                validaDo = false;
            }
            // Reuerimiento 4
            match(")");
            match(";");
        }

        // For -> for(Asignacion Condicion; Incremento) bloqueInstrucciones | Instruccion  
        private void For(bool evaluacion){
            bool validaFor;
            match("for");
            match("(");
            Asignacion(evaluacion);
            string variable = getContenido();
            int pos = posicion;
            int lin = linea;
            do{
                validaFor = Condicion();
                if(!evaluacion)
                    validaFor = false;
                match(";");
                Incremento(validaFor);
                match(")");
                if(getContenido() == "{")
                    bloqueInstrucciones(validaFor);
                else
                    instruccion(validaFor);
                if(validaFor){
                    posicion = pos - variable.Length;
                    linea = lin;
                    setPosicion(posicion);
                    NextToken();
                }
            }while(validaFor);
        }

        // Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion){
            string variable = getContenido();
            if(!existeVariable(getContenido()))
                throw new Error("Error de sintaxis, variable <" + getContenido() +"> no existe en el contexto actual, linea: "+linea, log);
            match(tipos.identificador);
            if(getContenido() == "++"){
                match("++");
                if(evaluacion)
                    modificaValor(variable, getValor(variable) + 1);
            }
            else{
                match("--");
                if(evaluacion)
                    modificaValor(variable, getValor(variable) - 1);
            }
        }

        // Switch -> switch(Expresion){listaDeCasos}
        private void Switch(bool evaluacion){
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            match(")");
            match("{");
            listaDeCasos(evaluacion);
            if(getContenido() == "default"){
                match("default");
                match(":");
                listaInstruccionesCase(evaluacion);
                if(getContenido() == "break"){
                    match("break");
                    match(";");
                }
            }
            match("}");
        }

        // listaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (listaDeCasos)?
        private void listaDeCasos(bool evaluacion){
            match("case");
            Expresion();
            stack.Pop();
            match(":");
            listaInstruccionesCase(evaluacion);
            if(getContenido() == "break") {
                match("break");
                match(tipos.fin_sentencia);
            }      
            if(getContenido() == "case")
                listaDeCasos(evaluacion);
        }

        //Condicion -> Expresion operador_relacional Expresion
        private bool Condicion(){
            Expresion();
            string operador = getContenido();
            match(tipos.operador_relacional);
            Expresion();
            float e2 = stack.Pop();
            float e1 = stack.Pop();
            switch(operador){
                case "==":
                    return e1 == e2;
                case ">":
                    return e1 > e2;
                case ">=":
                    return e1 >= e2;
                case "<":
                    return e1 < e2;
                case "<=":
                    return e1 <= e2;
                default:
                    return e1 != e2;
            }
        }

        // If -> if(Condicion) Bloque de instrucciones (Else bloqueInstrucciones)?
        private void If(bool evaluacion){
            bool validaIf;
            match("if");
            match("(");
            if(evaluacion)
                validaIf = Condicion();
            else{
                Condicion();
                validaIf = false;
            }
            match(")");
            if(getContenido() == "{")
                bloqueInstrucciones(validaIf);
            else
                instruccion(validaIf);
            if(getContenido() == "else"){
                match("else");
                if(getContenido() == "{"){
                    if(evaluacion)
                        bloqueInstrucciones(!validaIf);
                    else
                        bloqueInstrucciones(evaluacion);
                }
                else{
                    if(evaluacion)
                        instruccion(!validaIf);
                    else
                        instruccion(evaluacion);
                }
            }
        }

        // Printf -> printf(cadena | expresion);
        private void Printf(bool evaluacion){
            match("printf");
            match("(");
            if(getClasificacion() == tipos.cadena){
                if(evaluacion)
                    Console.Write(getContenido().Replace("\"","").Replace("\\n","\n").Replace("\\t","\t"));
                match(tipos.cadena);
            }
            else{
                Expresion();
                float resultado = stack.Pop();
                if(evaluacion)
                    Console.Write(resultado);
            }
            match(")");
            match(tipos.fin_sentencia);
        }

        // Scanf -> scanf(cadena , & identificador);
        private void Scanf(bool evaluacion){
            match("scanf");
            match("(");
            match(tipos.cadena);
            match(",");
            match("&");
            // Requerimiento 5: modificar el valor de la variable.
            if(!existeVariable(getContenido()))
                throw new Error("Error de sintaxis, variable <" + getContenido() +"> no existe en el contexto actual, linea: "+linea, log);
            if(evaluacion){
                try{
                    string valor = "" +Console.ReadLine();
                    float valorFloat = float.Parse(valor);
                    modificaValor(getContenido(),valorFloat);
                }
                catch(Exception){
                    throw new Error("Error de sintaxis, el valor ingresado no es un numero, linea: "+linea, log);
                }
            }
            match(tipos.identificador);
            match(")");
            match(tipos.fin_sentencia);
        }
        
        // Expresion -> Termino MasTermino
        private void Expresion(){
            termino();
            masTermino();
        }
        
        // MasTermino -> (operador_termino Termino)?
        private void masTermino(){
            if(getClasificacion() == tipos.operador_termino){
                string operador = getContenido();
                match(tipos.operador_termino);
                termino();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch(operador){
                    case "+":
                        stack.Push(n2 + n1);
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        break;
                }
            }
        }
        
        // Termino -> Factor PorFactor
        private void termino(){
            factor();
            porFactor();
        }
        
        // PorFactor -> (operador_factor Factor)?
        private void porFactor(){
            if(getClasificacion() == tipos.operador_factor){
                string operador = getContenido();
                match(tipos.operador_factor);
                factor();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch(operador){
                    case "*":
                        stack.Push(n2 * n1);
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        break;
                }
            }
        }
        
        // Factor -> numero | identificador | (Expresion) 
        private void factor(){
            if(getClasificacion() == tipos.numero){
                log.Write(getContenido() + " ");
                if(dominante < evaluaNumero(float.Parse(getContenido())))
                    dominante = evaluaNumero(float.Parse(getContenido()));
                stack.Push(float.Parse(getContenido()));
                match(tipos.numero);
            }
            else if(getClasificacion() == tipos.identificador){
                if(!existeVariable(getContenido()))
                    throw new Error("Error de sintaxis, variable <" + getContenido() +"> no existe en el contexto actual, linea: "+linea, log);
                log.Write(getContenido() + " ");
                // Requerimiento 1:  Actualizar el dominante para variables en la expresión.
                if(dominante < getTipo(getContenido()))
                    dominante = getTipo(getContenido());
                stack.Push(getValor(getContenido()));
                match(tipos.identificador);
            }
            else{
                bool huboCasteo = false;
                Variable.tipoDato casteo = Variable.tipoDato.Char;
                match("(");
                if(getClasificacion() == tipos.tipo_datos){
                    huboCasteo = true;
                    switch(getContenido()){
                        case "char":
                            casteo = Variable.tipoDato.Char;
                            break;
                        case "int":
                            casteo = Variable.tipoDato.Int;
                            break;
                        case "float":
                            casteo = Variable.tipoDato.Float;
                            break;
                    }
                    match(tipos.tipo_datos);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                if(huboCasteo){
                    /*
                    Requerimiento 2: 
                    Saco un elemento del stack ya
                    Convierte ese valor al equivalente en casteo
                    Requerimiento 3:
                    Si el casteo es char y el pop regresa un 256, el valor equivalente en casteo es 0
                    */
                    float valor = stack.Pop();
                    stack.Push(convertir(valor, casteo));
                    dominante = casteo;
                }
            }
        }
    }
}