using System;
/*
Requerimiento 1: Actualizar el dominante para variables en la expresión.
    Ejemplo: 
Requerimiento 2: Actualizar el dominante para el casteo.
Requerimiento 3: Programar un metodo de conversion de un valor a un tipo de dato.
    private float convertir(float valor, string tipo_dato)
    Deberan usar el residuo de la division %255, *65535
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

        private void modificaValor(string nombre, float nvalor){// Requerimiento 3 ya
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

        // Bloque de instrucciones -> {Lista de instrucciones?}
        private void bloqueInstrucciones(){
            match("{");
            if(getContenido() != "}")
                listaInstrucciones();
            match("}");
        }

        //listainstrucciones -> instruccion listadeinstrucciones?
        private void listaInstrucciones()
        {
            instruccion();
            if (getContenido() != "}")
                listaInstrucciones();
        }

        //listainstruccionesCase -> instruccion listaInstruccionesCase?
        private void listaInstruccionesCase()
        {
            instruccion();
            if (getContenido() != "case" && getContenido() != "default" && getContenido() != "}" && getContenido() != "break")
                listaInstruccionesCase();
        }

        // Instruccion -> Printf | Scanf | If | While | do while | Asignacion
        private void instruccion(){
            if(getContenido() == "printf")
                Printf();
            else if (getContenido() == "scanf")
                Scanf();
            else if (getContenido() == "if") 
                If();
            else if (getContenido() == "while")
                While();
            else if (getContenido() == "do")
                Do();
            else if (getContenido() == "for")
                For();
            else if (getContenido() == "switch")
                Switch();
            else
                Asignacion();
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
        private void Asignacion(){
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
            if(dominante <= getTipo(nombre))
                modificaValor(nombre, resultado);
            else
                throw new Error("Error de semantica, no podemos asignar un <" +dominante +"> a un <" +getTipo(nombre) +"> en linea: " +linea, log);
        }

        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(){
            match("while");
            match("(");
            Condicion();
            match(")");
            if(getContenido() == "{")
                bloqueInstrucciones();
            else
                instruccion();
        }

        //Do -> do bloqueInstrucciones | instruccion while(Condicion);
        private void Do(){
            match("do");
            if(getContenido() == "{")
                bloqueInstrucciones();
            else
                instruccion();
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }

        // For -> for(Asignacion Condicion; Incremento) bloqueInstrucciones | Instruccion  
        private void For(){
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");
            if(getContenido() == "{")
                bloqueInstrucciones();
            else
                instruccion();
        }

        // Incremento -> Identificador ++ | --
        private void Incremento(){
            string variable = getContenido();
            if(!existeVariable(getContenido()))
                throw new Error("Error de sintaxis, variable <" + getContenido() +"> no existe en el contexto actual, linea: "+linea, log);
            match(tipos.identificador);
            if(getContenido() == "++"){
                match("++");
                modificaValor(variable, getValor((variable))+1);
            }
            else{
                match("--");
                modificaValor(variable, getValor((variable))-1);
            }
        }

        // Switch -> switch(Expresion){listaDeCasos}
        private void Switch(){
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            match(")");
            match("{");
            listaDeCasos();
            if(getContenido() == "default"){
                match("default");
                match(":");
                listaInstruccionesCase();
                if(getContenido() == "break"){
                    match("break");
                    match(";");
                }
            }
            match("}");
        }

        // listaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (listaDeCasos)?
        private void listaDeCasos(){
            match("case");
            Expresion();
            stack.Pop();
            match(":");
            listaInstruccionesCase();
            if(getContenido() == "break") {
                match("break");
                match(tipos.fin_sentencia);
            }      
            if(getContenido() == "case")
                listaDeCasos();
        }

        //Condicion -> Expresion operador_relacional Expresion
        private void Condicion(){
            Expresion();
            stack.Pop();
            match(tipos.operador_relacional);
            Expresion();
            stack.Pop();
        }

        // If -> if(Condicion) Bloque de instrucciones (Else bloqueInstrucciones)?
        private void If(){
            match("if");
            match("(");
            Condicion();
            match(")");
            if(getContenido() == "{")
                bloqueInstrucciones();
            else
                instruccion();
            if(getContenido() == "else"){
                match("else");
                if(getContenido() == "{")
                    bloqueInstrucciones();
                else
                    instruccion();
            }
        }

        // Printf -> printf(cadena | expresion);
        private void Printf(){
            match("printf");
            match("(");
            if(getClasificacion() == tipos.cadena){
                Console.Write(getContenido().Replace("\"","").Replace("\\n","\n").Replace("\\t","\t"));
                match(tipos.cadena);
            }
            else{
                foreach(Variable v in variables){
                    if(getContenido().Equals(v.getNombre()))
                        Console.Write(v.getValor());
                }
                Expresion();
                stack.Pop();
            }
            match(")");
            match(tipos.fin_sentencia);
        }

        // Scanf -> scanf(cadena , & identificador);
        private void Scanf(){
            match("scanf");
            match("(");
            match(tipos.cadena);
            match(",");
            match("&");
            // Requerimiento 2: si no existe el indentificador, lanzar excepcion.
            string valor = "" + Console.ReadLine();
            modificaValor(getContenido(),float.Parse(valor));
            // Requerimiento 5: modificar el valor de la variable.
            if(!existeVariable(getContenido())){
                throw new Error("Error de sintaxis, variable <" + getContenido() +"> no existe en el contexto actual, linea: "+linea, log);
            }
            match(tipos.identificador);
            match(")");
            match(tipos.fin_sentencia);
        }

        // Main -> void main() Bloque de instrucciones
        private void Main(){
            match("void");
            match("main");
            match("(");
            match(")");
            bloqueInstrucciones();
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
                    Saco un elemento del stack 
                    Convierte ese valor al equivalente en casteo 
                    Requerimiento 3:
                    Si el cateo es char y el pop regresa un 256, el valor equivalente en casteo es 0
                    */
                    dominante = casteo;
                }
            }
        }

    }

}