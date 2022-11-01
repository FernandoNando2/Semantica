// Fernando Hernández Domínguez
using System;
/*
Requerimiento 1: Actualizacion: 
    a) Agregar el residuo de la division en el porFactor() ya.
    b) Agregar en instruccion los incrementos de termino y factor
        a++, a--, a+=1, a-=1, a*=1, a/=1
        en donde el 1 puede ser una expresion ya
    c) Programar el destructor de la clase Lenguaje para ejecutar el metodo cerrar() en clase Lexico usar libreria y contenedor ya
Requerimiento 2: Actualizacion la venganza:
    a) Marcar errores semanticos cuando los incrementos de termino o factor superen el rango de la variable
    b) Considerar el inciso a) y b) para el for.
    c) Hacer funcionar el do y while ya
Requerimiento 3: 
    a) Considerar las variables y los casteos en las expresiones matematicas en ensamblador ya
    b) Considerar el residuo la division en ensamblador ya
    c) Programar el printf y scanf en ensamblador ya
Requerimiento 4: 
    a) Programar el else en ensamblador
    b) Programar el for en ensamblador
Requerimiento 5: 
    a) Programar el while en ensamblador
    b) Programar el do while en ensamblador
*/
namespace Semantica{
    class Lenguaje : Sintaxis,IDisposable{
        public void Dispose(){
            Console.WriteLine("Destructor");
        }
        List <Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.tipoDato dominante;
        int cIf;
        int cFor;
        public Lenguaje(){
            cIf = cFor = 0;
        }

        public Lenguaje(String ruta) : base(ruta) {
            cIf = cFor = 0;
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

        public void variablesAsm(){
            asm.WriteLine("\n;Variables: \n");
            foreach (Variable v in variables){
                if(v.getTipo() == Variable.tipoDato.Char){
                    asm.WriteLine("\t" +v.getNombre() +" db  ?");
                }
                if(v.getTipo() == Variable.tipoDato.Int){
                    asm.WriteLine("\t" +v.getNombre() +" dw  ?");
                }
                if(v.getTipo() == Variable.tipoDato.Float){
                    asm.WriteLine("\t" +v.getNombre() +" dd  ?");
                }
            }
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
            asm.WriteLine("#make_COM#");
            asm.WriteLine("include emu8086.inc");
            asm.WriteLine("ORG 100H");
            Librerias();
            Variables();
            variablesAsm();
            Main();
            displayVariables();
            asm.WriteLine("RET");
            asm.WriteLine("DEFINE_SCAN_NUM");
            asm.WriteLine("END");
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
            string nombre = getContenido();
            float resultado;
            string operador;
            if(!existeVariable(nombre))
                throw new Error("Error de sintaxis, variable <" + getContenido() +"> no existe en el contexto actual, linea: "+linea, log);
            else{
                match(tipos.identificador);
                dominante = Variable.tipoDato.Char;
                operador = getContenido();
                switch(operador[0]){
                    case '+':
                        if (getContenido() == "++"){
                            modificaValor(nombre, getValor(nombre) + 1);
                            match("++");
                            match(tipos.fin_sentencia);
                        }
                        else{
                            match("+=");
                            Expresion();
                            resultado = stack.Pop();
                            modificaValor(nombre, getValor(nombre) + resultado);
                            match(tipos.fin_sentencia);
                        }
                        break;
                    case '-':
                        if (getContenido() == "--"){
                            modificaValor(nombre, getValor(nombre) - 1);
                            match("--");
                            match(tipos.fin_sentencia);
                        }
                        else{
                            match("-=");
                            Expresion();
                            resultado = stack.Pop();
                            modificaValor(nombre, getValor(nombre) - resultado);
                            match(tipos.fin_sentencia);
                        }
                        break;
                    case '*':
                        match("*=");
                        Expresion();
                        resultado = stack.Pop();
                        modificaValor(nombre, getValor(nombre) * resultado);
                        match(tipos.fin_sentencia);
                        break;
                    case '/':
                        match("/=");
                        Expresion();
                        resultado = stack.Pop();
                        modificaValor(nombre, getValor(nombre) / resultado);
                        match(tipos.fin_sentencia);
                        break;
                    case '=':
                        match("=");
                        Expresion();
                        match(";");
                        resultado = stack.Pop();
                        asm.WriteLine("POP AX");
                        log.Write(nombre +" = " +resultado);
                        log.WriteLine();
                        if(dominante < evaluaNumero(resultado))
                            dominante = evaluaNumero(resultado);
                        if(dominante <= getTipo(nombre)){
                            if(evaluacion)
                                modificaValor(nombre, resultado);
                        }
                        else
                            throw new Error("Error de semantica, no podemos asignar un <" +dominante +"> a un <" +getTipo(nombre) +"> en linea: " +linea, log);
                        asm.WriteLine("MOV " + nombre + ", AX");
                        break;
                }
            }
        }

        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool evaluacion){
            bool validaWhile;
            match("while");
            match("(");
            int pos = posicion;
            int lin = linea;
            string variable = getContenido();
            do{
            if(evaluacion)
                validaWhile = Condicion("");
            else{
                Condicion("");
                validaWhile = false;
            }
            match(")");
            if(getContenido() == "{")
                bloqueInstrucciones(validaWhile);
            else
                instruccion(validaWhile);
            if(validaWhile){
                posicion = pos - variable.Length;
                linea = lin;
                setPosicion(posicion);
                NextToken();
            }
            }while(validaWhile);
        }

        //Do -> do bloqueInstrucciones | instruccion while(Condicion);
        private void Do(bool evaluacion){
            bool validaDo = evaluacion;
            match("do");
            int pos = posicion;
            int lin = linea;
            do{
                if(getContenido() == "{")
                    bloqueInstrucciones(validaDo);
                else
                    instruccion(validaDo);
                match("while");
                match("(");
                string variable = getContenido();
                validaDo = Condicion("");
                if(!evaluacion)
                    validaDo = evaluacion;
                else if(validaDo){
                    posicion = pos - 1;
                    linea = lin;
                    setPosicion(posicion);
                    NextToken();
                }
            }while(validaDo);
            match(")");
            match(";");
        }

        // For -> for(Asignacion Condicion; Incremento) bloqueInstrucciones | Instruccion  
        private void For(bool evaluacion){
            string etiquetaInFor = "inicioFor" +cFor;
            string etiquetaFinFor = "finFor" +cFor++;
            asm.WriteLine(etiquetaInFor + ":");
            bool validaFor;
            match("for");
            match("(");
            Asignacion(evaluacion);
            string variable = getContenido();
            float valor = 0;
            int pos = posicion;
            int lin = linea;
            do{
                validaFor = Condicion("");
                if(!evaluacion)
                    validaFor = false;
                match(";");
                valor = Incremento();
                // Requerimiento 1 d)
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
                    modificaValor(variable, valor);
                }
            }while(validaFor);
            asm.WriteLine(etiquetaFinFor +":");
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

        private float Incremento(){
            string variable = getContenido();
            float valor = getValor(variable);
            if(!existeVariable(getContenido()))
                throw new Error("Error de sintaxis, variable <" + getContenido() +"> no existe en el contexto actual, linea: "+linea, log);
            match(tipos.identificador);
            if(getContenido() == "++"){
                match("++");
                valor += 1;
            }
            else{
                match("--");
                valor -= 1;
            }
            return valor;
        }

        // Switch -> switch(Expresion){listaDeCasos}
        private void Switch(bool evaluacion){
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
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
            asm.WriteLine("POP AX");
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
        private bool Condicion(string etiqueta){
            Expresion();
            string operador = getContenido();
            match(tipos.operador_relacional);
            Expresion();
            float e2 = stack.Pop();
            asm.WriteLine("POP AX");
            float e1 = stack.Pop();
            asm.WriteLine("POP BX");
            asm.WriteLine("CMP AX, BX");
            switch(operador){
                case "==":
                    asm.WriteLine("JNE " + etiqueta);
                    return e1 == e2;
                case ">":
                    asm.WriteLine("JLE " + etiqueta);
                    return e1 > e2;
                case ">=":
                    asm.WriteLine("JL " + etiqueta);
                    return e1 >= e2;
                case "<":
                    asm.WriteLine("JGE " + etiqueta);
                    return e1 < e2;
                case "<=":
                    asm.WriteLine("JG " + etiqueta);
                    return e1 <= e2;
                default:
                    asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }
        }

        // If -> if(Condicion) Bloque de instrucciones (Else bloqueInstrucciones)?
        private void If(bool evaluacion){
            string etiquetaIf = "if" + ++cIf;
            bool validaIf;
            match("if");
            match("(");
            if(evaluacion)
                validaIf = Condicion(etiquetaIf);
            else{
                Condicion("");
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
            asm.WriteLine(etiquetaIf + ":");
        }

        // Printf -> printf(cadena | expresion);
        private void Printf(bool evaluacion){
            match("printf");
            match("(");
            if(getClasificacion() == tipos.cadena){
                if(evaluacion)
                    Console.Write(getContenido().Replace("\"","").Replace("\\n","\n").Replace("\\t","\t"));
                asm.WriteLine("PRINTN " +getContenido());
                match(tipos.cadena);
            }
            else{
                Expresion();
                float resultado = stack.Pop();
                asm.WriteLine("POP AX");
                if(evaluacion)
                    //Codigo ensamblador para imprimir una variable
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
                asm.WriteLine("CALL SCAN_NUM");
                asm.WriteLine("MOV " + getContenido() + ", CX");
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
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                switch(operador){
                    case "+":
                        stack.Push(n2 + n1);
                        asm.WriteLine("ADD AX,BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        asm.WriteLine("SUB AX,BX");
                        asm.WriteLine("PUSH AX");
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
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                switch(operador){
                    case "*":
                        stack.Push(n2 * n1);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH DX");
                        break;
                }
            }
        }
        
        // Factor -> numero | identificador | (Expresion) 
        private void factor(){
            if(getClasificacion() == tipos.numero){
                if(dominante < evaluaNumero(float.Parse(getContenido())))
                    dominante = evaluaNumero(float.Parse(getContenido()));
                stack.Push(float.Parse(getContenido()));
                asm.WriteLine("MOV AX," + getContenido());
                asm.WriteLine("PUSH AX");
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
                    float valor = stack.Pop();
                    asm.WriteLine("POP AX");
                    stack.Push(convertir(valor, casteo));
                    dominante = casteo;
                }
            }
        }
    }
}