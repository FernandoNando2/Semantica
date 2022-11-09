// Fernando Hernández Domínguez
using System;
/*
Requerimiento 1: Actualizacion: ya
    a) Agregar el residuo de la division en el porFactor() ya.
    b) Agregar en instruccion los incrementos de termino y factor
        a++, a--, a+=1, a-=1, a*=1, a/=1
        en donde el 1 puede ser una expresion ya
    c) Programar el destructor de la clase Lenguaje para ejecutar el metodo cerrar() en clase Lexico usar libreria y contenedor ya
Requerimiento 2: Actualizacion la venganza: ya
    a) Marcar errores semanticos cuando los incrementos de termino o factor superen el rango de la variable ya
    b) Considerar el inciso a) y b) para el for. ya
    c) Hacer funcionar el do y while ya
Requerimiento 3: ya 
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
    class Lenguaje : Sintaxis, IDisposable{

        public void Dispose(){
            cerrar();
        }
        List <Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.tipoDato dominante;
        int cIf, cFor,cWhile, cDo;
        string operador;
        public Lenguaje(){
            cIf = cFor = cWhile = cDo = 0;
            operador = "";
        }

        public Lenguaje(String ruta) : base(ruta) {
            cIf = cFor = cWhile = cDo = 0;
            operador = "";
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
            Main();
            displayVariables();
            variablesAsm();
            //asm.WriteLine("RET");
            asm.WriteLine("DEFINE_PRINT_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM_UNS");
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
            bloqueInstrucciones(true,true);
        }

        // Bloque de instrucciones -> {Lista de instrucciones?}
        private void bloqueInstrucciones(bool evaluacion, bool ASM){
            match("{");
            if(getContenido() != "}")
                listaInstrucciones(evaluacion,ASM);
            match("}");
        }

        //listainstrucciones -> instruccion listadeinstrucciones?
        private void listaInstrucciones(bool evaluacion, bool ASM)
        {
            instruccion(evaluacion,ASM);
            if (getContenido() != "}")
                listaInstrucciones(evaluacion,ASM);
        }

        //listainstruccionesCase -> instruccion listaInstruccionesCase?
        private void listaInstruccionesCase(bool evaluacion, bool ASM)
        {
            instruccion(evaluacion, ASM);
            if (getContenido() != "case" && getContenido() != "default" && getContenido() != "}" && getContenido() != "break")
                listaInstruccionesCase(evaluacion, ASM);
        }

        // Instruccion -> Printf | Scanf | If | While | do while | Asignacion
        private void instruccion(bool evaluacion, bool ASM){
            if(getContenido() == "printf")
                Printf(evaluacion, ASM);
            else if (getContenido() == "scanf")
                Scanf(evaluacion,ASM);
            else if (getContenido() == "if") 
                If(evaluacion,ASM);
            else if (getContenido() == "while")
                While(evaluacion,ASM);
            else if (getContenido() == "do")
                Do(evaluacion,ASM);
            else if (getContenido() == "for")
                For(evaluacion,ASM);
            else if (getContenido() == "switch")
                Switch(evaluacion,ASM);
            else
                Asignacion(evaluacion,ASM);
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
        private void Asignacion(bool evaluacion, bool ASM){
            string nombre = getContenido();
            float resultado;
            if(!existeVariable(nombre))
                throw new Error("Error de sintaxis, variable <" + getContenido() +"> no existe en el contexto actual, linea: "+linea, log);
            else{
                match(tipos.identificador);
                if(getClasificacion() == tipos.incremento_factor || getClasificacion() == tipos.incremento_termino){
                    modificaValor(nombre, Incremento(nombre,evaluacion,ASM));
                    match(tipos.fin_sentencia);
                }
                else{
                    match("=");
                    dominante = Variable.tipoDato.Char;
                    Expresion(ASM);
                    resultado = stack.Pop();
                    if(ASM){
                        asm.WriteLine("POP AX");
                        if(getTipo(nombre) == Variable.tipoDato.Char)
                            asm.WriteLine("MOV "+nombre +", AL");
                        else
                            asm.WriteLine("MOV " +nombre +", AX");
                    }
                    match(tipos.fin_sentencia);
                    if (dominante < evaluaNumero(resultado))
                        dominante = evaluaNumero(resultado);
                    if (dominante <= getTipo(nombre)){
                        if (evaluacion)
                            modificaValor(nombre, resultado);
                    }
                    else
                        throw new Error("Error de semantica no podemos asignar un <" + dominante + "> a un <" + getTipo(nombre) + "> en linea: " + linea, log);
                    if(getTipo(nombre) == Variable.tipoDato.Char){
                        if(ASM)
                            asm.WriteLine("MOV AH, 0");
                    }
                }
            }
        }

        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool evaluacion, bool ASM){
            if(ASM)
                ++cWhile;
            bool validaWhile;
            string etiquetaInWhile = "iniciowhile" +cWhile;
            string etiquetaFinWhile = "finwhile" +cWhile;
            match("while");
            match("(");
            int pos = posicion;
            int lin = linea;
            string variable = getContenido();
            do{
                if(ASM)
                    asm.WriteLine(etiquetaInWhile + ":");
                validaWhile = Condicion(etiquetaFinWhile,ASM);
                if(!evaluacion)
                    validaWhile = false;
                match(")");
                if(getContenido() == "{")
                    bloqueInstrucciones(validaWhile, ASM);
                else
                    instruccion(validaWhile, ASM);
                if(validaWhile){
                    posicion = pos - variable.Length;
                    linea = lin;
                    setPosicion(posicion);
                    NextToken();
                }
                if(ASM){
                    asm.WriteLine("JMP " +etiquetaInWhile);
                    asm.WriteLine(etiquetaFinWhile + ":");
                }
                ASM = false;
            }while(validaWhile);
        }

        //Do -> do bloqueInstrucciones | instruccion while(Condicion);
        private void Do(bool evaluacion,bool ASM){
            if(ASM)
                ++cDo;
            string etiquetaInDo = "iniciodo" +cDo;
            string etiquetaFinDo = "findo" +cDo;
            bool validaDo = evaluacion;
            string variable;
            match("do");
            int pos = posicion;
            int lin = linea;
            do{
                if(ASM)
                    asm.WriteLine(etiquetaInDo + ":");
                if(getContenido() == "{")
                    bloqueInstrucciones(validaDo, ASM);
                else
                    instruccion(validaDo, ASM);
                match("while");
                match("(");
                variable = getContenido();
                validaDo = Condicion(etiquetaFinDo,ASM);
                if(!evaluacion)
                    validaDo = evaluacion;
                else if(validaDo){
                    posicion = pos - 1;
                    linea = lin;
                    setPosicion(posicion);
                    NextToken();
                }
                if(ASM){
                    asm.WriteLine("JMP " +etiquetaInDo);
                    asm.WriteLine(etiquetaFinDo + ":");
                }
                ASM = false;
            }while(validaDo);
            match(")");
            match(";");
        }

        // For -> for(Asignacion Condicion; Incremento) bloqueInstrucciones | Instruccion  
        private void For(bool evaluacion, bool ASM){
            if(ASM)
                ++cFor;
            string etiquetaInFor = "inicioFor" +cFor;
            string etiquetaFinFor = "finFor" +cFor;
            bool validaFor;
            match("for");
            match("(");
            Asignacion(evaluacion,ASM);
            string variable = getContenido();
            float valor = 0;
            int pos = posicion;
            int lin = linea;
            do{
                if(ASM)
                    asm.WriteLine(etiquetaInFor + ":");
                validaFor = Condicion(etiquetaFinFor,ASM);
                if(!evaluacion)
                    validaFor = false;
                match(";");
                match(tipos.identificador);
                valor = IncrementoFor(variable,evaluacion,ASM);
                string operador1 = operador;
                match(")");
                if(getContenido() == "{")
                    bloqueInstrucciones(validaFor,ASM);
                else
                    instruccion(validaFor,ASM);
                if(validaFor){
                    posicion = pos - variable.Length;
                    linea = lin;
                    modificaValor(variable, valor);
                    setPosicion(posicion);
                    switch(operador1){
                        case "++":
                            if(ASM)
                                asm.WriteLine("INC " +variable);
                            break;
                        case "--":
                            if(ASM)
                                asm.WriteLine("DEC " +variable);
                            break;
                        case "+=":
                            if(ASM){
                                asm.WriteLine("POP AX");
                                asm.WriteLine("ADD " +variable +", AX");
                            }
                            break;
                        case "-=":
                            if(ASM){
                                asm.WriteLine("POP AX");
                                asm.WriteLine("SUB " +variable +", AX");
                            }
                            break;
                        case "*=":
                            if(ASM){
                                asm.WriteLine("POP AX");
                                asm.WriteLine("MUL " +variable);
                            }
                            break;
                        case "/=":
                            if(ASM){
                                asm.WriteLine("POP AX");
                                asm.WriteLine("DIV " +variable);
                            }
                            break;
                    }
                    NextToken();
                }
                if(ASM){
                    asm.WriteLine("JMP " +etiquetaInFor);
                    asm.WriteLine(etiquetaFinFor + ":");
                }
                ASM = false;
            }while(validaFor);
        }

        // Incremento -> Identificador ++ | --

        private float IncrementoFor(string variable, bool evaluacion, bool ASM){
            float resultado;
            float valor = getValor(variable);
            if(!existeVariable(variable))
                throw new Error("Error de sintaxis, variable <" +variable +"> no existe en el contexto actual, linea: "+linea, log);
            operador = getContenido();
            switch(operador){
                case "++":
                    if(evaluacion)
                        valor++;
                    match("++");
                    if(getTipo(variable) == Variable.tipoDato.Char && valor > 255)
                        throw new Error("Error de semantica, variable <" +variable +"> excede el rango del char en linea: "+linea, log);
                    else if(getTipo(variable) == Variable.tipoDato.Int && valor > 65535)
                        throw new Error("Error de semantica, variable <" +variable +"> excede el rango del int en linea: "+linea, log);
                    break;
                case "--":
                    if(evaluacion)
                        valor--;
                    match("--");
                    break;
                case "+=":
                    match("+=");
                    Expresion(ASM);
                    resultado = stack.Pop();
                    if(evaluacion)
                        valor += resultado;
                    break;
                case "-=":
                    match("-=");
                    Expresion(ASM);
                    resultado = stack.Pop();
                    if (evaluacion)
                        valor -= resultado;
                    break;
                case "*=":
                    match("*=");
                    Expresion(ASM);
                    resultado = stack.Pop();
                    if (evaluacion)
                        valor *= resultado;
                    break;
                case "/=":
                    match("/=");
                    Expresion(ASM);
                    resultado = stack.Pop();
                    if(evaluacion)
                        valor /= resultado;
                    break;
            }
            if(getTipo(variable) < dominante)
                throw new Error("Error de semantica, no podemos asignar un <" + dominante + "> a un <" + getTipo(variable) + "> en linea: " + linea, log);
            return valor;
        }

        private float Incremento(string variable, bool evaluacion, bool ASM){
            float resultado;
            float valor = getValor(variable);
            if(!existeVariable(variable))
                throw new Error("Error de sintaxis, variable <" +variable +"> no existe en el contexto actual, linea: "+linea, log);
            operador = getContenido();
            switch(operador){
                case "++":
                    if(evaluacion){
                        valor++;
                        if(ASM)
                            asm.WriteLine("INC " +variable);
                    }
                    match("++");
                    if(getTipo(variable) == Variable.tipoDato.Char && valor > 255)
                        throw new Error("Error de semantica, variable <" +variable +"> excede el rango del char en linea: "+linea, log);
                    else if(getTipo(variable) == Variable.tipoDato.Int && valor > 65535)
                        throw new Error("Error de semantica, variable <" +variable +"> excede el rango del int en linea: "+linea, log);
                    break;
                case "--":
                    if(evaluacion){
                        valor--;
                        if(ASM)
                            asm.WriteLine("DEC " +variable);
                    }
                    match("--");
                    break;
                case "+=":
                    match("+=");
                    Expresion(ASM);
                    resultado = stack.Pop();
                    if(evaluacion)
                        valor += resultado;
                    if(ASM){
                        asm.WriteLine("POP AX");
                        asm.WriteLine("ADD " +variable +", AX");
                    }
                    break;
                case "-=":
                    match("-=");
                    Expresion(ASM);
                    resultado = stack.Pop();
                    if (evaluacion)
                        valor -= resultado;
                    if(ASM){
                        asm.WriteLine("POP AX");
                        asm.WriteLine("SUB " +variable +", AX");
                    }
                    break;
                case "*=":
                    match("*=");
                    Expresion(ASM);
                    resultado = stack.Pop();
                    if (evaluacion){
                        valor *= resultado;
                        if(ASM){
                            asm.WriteLine("POP AX");
                            asm.WriteLine("MUL " +variable);
                        }
                    }
                    break;
                case "/=":
                    match("/=");
                    Expresion(ASM);
                    resultado = stack.Pop();
                    if(evaluacion){
                        valor /= resultado;
                        if(ASM){
                            asm.WriteLine("POP AX");
                            asm.WriteLine("DIV " +variable);
                        }
                    }
                    break;
            }
            if(getTipo(variable) < dominante)
                throw new Error("Error de semantica, no podemos asignar un <" + dominante + "> a un <" + getTipo(variable) + "> en linea: " + linea, log);
            return valor;
        }

        // Switch -> switch(Expresion){listaDeCasos}
        private void Switch(bool evaluacion, bool ASM){
            match("switch");
            match("(");
            Expresion(ASM);
            stack.Pop();
            if(ASM)
                asm.WriteLine("POP AX");
            match(")");
            match("{");
            listaDeCasos(evaluacion,ASM);
            if(getContenido() == "default"){
                match("default");
                match(":");
                listaInstruccionesCase(evaluacion,ASM);
                if(getContenido() == "break"){
                    match("break");
                    match(";");
                }
            }
            match("}");
        }

        // listaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (listaDeCasos)?
        private void listaDeCasos(bool evaluacion, bool ASM){
            match("case");
            Expresion(ASM);
            stack.Pop();
            if(ASM)
                asm.WriteLine("POP AX");
            match(":");
            listaInstruccionesCase(evaluacion,ASM);
            if(getContenido() == "break") {
                match("break");
                match(tipos.fin_sentencia);
            }      
            if(getContenido() == "case")
                listaDeCasos(evaluacion,ASM);
        }

        //Condicion -> Expresion operador_relacional Expresion
        private bool Condicion(string etiqueta, bool ASM){
            Expresion(ASM);
            string operador = getContenido();
            match(tipos.operador_relacional);
            Expresion(ASM);
            float e2 = stack.Pop();
            if(ASM)
                asm.WriteLine("POP BX");
            float e1 = stack.Pop();
            if(ASM){
                asm.WriteLine("POP AX");
                asm.WriteLine("CMP AX, BX");
            }
            switch(operador){
                case "==":
                    if(ASM)
                        asm.WriteLine("JNE " + etiqueta);
                    return e1 == e2;
                case ">":
                    if(ASM)
                        asm.WriteLine("JLE " + etiqueta);
                    return e1 > e2;
                case ">=":
                    if(ASM)
                        asm.WriteLine("JL " + etiqueta);
                    return e1 >= e2;
                case "<":
                    if(ASM)
                        asm.WriteLine("JGE " + etiqueta);
                    return e1 < e2;
                case "<=":
                    if(ASM)
                        asm.WriteLine("JG " + etiqueta);
                    return e1 <= e2;
                default:
                    if(ASM)
                        asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }
        }

        // If -> if(Condicion) Bloque de instrucciones (Else bloqueInstrucciones)?
        private void If(bool evaluacion, bool ASM){
            if(ASM)
                ++cIf;
            string etiquetaIf = "if" +cIf;
            string etiquetaFinIf = "finIf" +cIf;
            bool validaIf;
            match("if");
            match("(");
            validaIf = Condicion(etiquetaIf,ASM);
            if(!evaluacion)
                validaIf = false;
            match(")");
            if(getContenido() == "{")
                bloqueInstrucciones(validaIf,ASM);
            else
                instruccion(validaIf,ASM);
            if(ASM){
                asm.WriteLine("JMP " + etiquetaFinIf);
            }
            if(ASM)
                asm.WriteLine(etiquetaIf + ":");
            if(getContenido() == "else"){
                match("else");
                if(getContenido() == "{"){
                    if(evaluacion)
                        bloqueInstrucciones(!validaIf,ASM);
                    else
                        bloqueInstrucciones(evaluacion,ASM);
                }
                else{
                    if(evaluacion)
                        instruccion(!validaIf,ASM);
                    else
                        instruccion(evaluacion,ASM);
                }
            }
            if(ASM)
                asm.WriteLine(etiquetaFinIf + ":");
        }

        // Printf -> printf(cadena | expresion);
        private void Printf(bool evaluacion, bool ASM){
            match("printf");
            match("(");
            if(getClasificacion() == tipos.cadena){
                string cadenaLimpia = getContenido().Replace("\"","").Replace("\\t","\t");
                if(evaluacion)
                    Console.Write(cadenaLimpia.Replace("\\n","\n"));
                if(ASM){
                    if(cadenaLimpia.Contains("\\n")){
                        string[] cadenas = cadenaLimpia.Split("\\n");
                        for(int i = 0; i < cadenas.Length; i++){
                            if(i == cadenas.Length - 1)
                                asm.WriteLine("PRINT \'"+ cadenas[i] +"\'");
                            else
                                asm.WriteLine("PRINTN \'"+ cadenas[i] +"\'");
                        }
                    }
                    else
                        asm.WriteLine("PRINT \'"+ cadenaLimpia +"\'");
                }
                match(tipos.cadena);
            }
            else{
                Expresion(ASM);
                float resultado = stack.Pop();
                if(ASM)
                    asm.WriteLine("POP AX");
                if(evaluacion){
                    if(ASM)
                        asm.WriteLine("CALL PRINT_NUM");
                    Console.Write(resultado);
                }
            }
            match(")");
            match(tipos.fin_sentencia);
        }

        // Scanf -> scanf(cadena , & identificador);
        private void Scanf(bool evaluacion, bool ASM){
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
                if(ASM){
                    asm.WriteLine("CALL SCAN_NUM");
                    asm.WriteLine("MOV " + getContenido() + ", CX");
                }
            }
            match(tipos.identificador);
            match(")");
            match(tipos.fin_sentencia);
        }
        
        // Expresion -> Termino MasTermino
        private void Expresion(bool ASM){
            termino(ASM);
            masTermino(ASM);
        }
        
        // MasTermino -> (operador_termino Termino)?
        private void masTermino(bool ASM){
            if(getClasificacion() == tipos.operador_termino){
                operador = getContenido();
                match(tipos.operador_termino);
                termino(ASM);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                if(ASM)
                    asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                if(ASM)
                    asm.WriteLine("POP AX");
                switch(operador){
                    case "+":
                        stack.Push(n2 + n1);
                        if(ASM){
                            asm.WriteLine("ADD AX,BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        if(ASM){
                            asm.WriteLine("SUB AX,BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                }
            }
        }
        
        // Termino -> Factor PorFactor
        private void termino(bool ASM){
            factor(ASM);
            porFactor(ASM);
        }
        
        // PorFactor -> (operador_factor Factor)?
        private void porFactor(bool ASM){
            if(getClasificacion() == tipos.operador_factor){
                operador = getContenido();
                match(tipos.operador_factor);
                factor(ASM);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                if(ASM)
                    asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                if(ASM)
                    asm.WriteLine("POP AX");
                switch(operador){
                    case "*":
                        stack.Push(n2 * n1);
                        if(ASM){
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        if(ASM){
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        if(ASM){
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH DX");
                        }
                        break;
                }
            }
        }
        
        // Factor -> numero | identificador | (Expresion) 
        private void factor(bool ASM){
            if(getClasificacion() == tipos.numero){
                if(dominante < evaluaNumero(float.Parse(getContenido())))
                    dominante = evaluaNumero(float.Parse(getContenido()));
                stack.Push(float.Parse(getContenido()));
                if(ASM){
                    if(dominante == Variable.tipoDato.Char)
                        asm.WriteLine("MOV AL," + getContenido());
                    else
                        asm.WriteLine("MOV AX," + getContenido());
                    asm.WriteLine("PUSH AX");
                }
                match(tipos.numero);
            }
            else if(getClasificacion() == tipos.identificador){
                if(!existeVariable(getContenido()))
                    throw new Error("Error de sintaxis, variable <" + getContenido() +"> no existe en el contexto actual, linea: "+linea, log);
                log.Write(getContenido() + " ");
                if(dominante < getTipo(getContenido()))
                    dominante = getTipo(getContenido());
                stack.Push(getValor(getContenido()));
                if(ASM){
                    if(dominante == Variable.tipoDato.Char)
                        asm.WriteLine("MOV AL," + getContenido());
                    else
                        asm.WriteLine("MOV AX," + getContenido());
                    asm.WriteLine("PUSH AX");
                }
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
                Expresion(ASM);
                match(")");
                if(huboCasteo){
                    float valor = stack.Pop();
                    if(ASM)
                        asm.WriteLine("MOV AH, 0");
                    stack.Push(convertir(valor, casteo));
                    dominante = casteo;
                }
            }
        }
    }
}