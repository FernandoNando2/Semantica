;Archivo: prueba.asm
;Compilado: 09/11/2022 09:44:37 a. m.
#make_COM#
include emu8086.inc
ORG 100H
PRINT 'Introduce la altura de la piramide: '
CALL SCAN_NUM
MOV altura, CX
MOV AX,altura
PUSH AX
MOV AX,2
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE if1
MOV AX,altura
PUSH AX
POP AX
MOV i, AX
inicioFor1:
MOV AX,i
PUSH AX
MOV AX,0
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE finFor1
MOV AX,1
PUSH AX
MOV AL,0
PUSH AX
POP AX
MOV j, AX
iniciowhile1:
MOV AX,j
PUSH AX
MOV AX,altura
PUSH AX
MOV AX,i
PUSH AX
POP BX
POP AX
SUB AX,BX
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finwhile1
MOV AX,j
PUSH AX
MOV AX,2
PUSH AX
POP BX
POP AX
DIV BX
PUSH DX
MOV AX,0
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if2
PRINT '*'
JMP finIf2
if2:
PRINT '-'
finIf2:
MOV AX,1
PUSH AX
POP AX
ADD j, AX
JMP iniciowhile1
finwhile1:
PRINTN ''
PRINT ''
POP AX
SUB i, AX
JMP inicioFor1
finFor1:
MOV AL,0
PUSH AX
POP AX
MOV k, AX
iniciodo1:
PRINT '-'
MOV AL,2
PUSH AX
POP AX
ADD k, AX
MOV AX,k
PUSH AX
MOV AX,altura
PUSH AX
MOV AX,2
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE findo1
JMP iniciodo1
findo1:
PRINTN ''
PRINT ''
JMP finIf1
if1:
PRINTN ''
PRINTN 'Error: la altura debe de ser mayor que 2'
PRINT ''
finIf1:
MOV AX,1
PUSH AX
MOV AX,1
PUSH AX
POP BX
POP AX
CMP AX, BX
JE if3
PRINT 'Esto no se debe imprimir'
MOV AX,2
PUSH AX
MOV AX,2
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if4
PRINT 'Esto tampoco'
JMP finIf4
if4:
finIf4:
JMP finIf3
if3:
finIf3:
MOV AX,258
PUSH AX
POP AX
MOV a, AX
PRINT 'Valor de variable int 'a' antes del casteo: '
MOV AX,a
PUSH AX
POP AX
CALL PRINT_NUM
MOV AX,a
PUSH AX
MOV AH, 0
POP AX
MOV y, AL
MOV AH, 0
PRINTN ''
PRINT 'Valor de variable char 'y' despues del casteo de a: '
MOV AL,y
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN ''
PRINTN 'A continuacion se intenta asignar un int a un char sin usar casteo: '
PRINT ''

;Variables: 

	area dd  ?
	radio dd  ?
	pi dd  ?
	resultado dd  ?
	a dw  ?
	d dw  ?
	altura dw  ?
	cinco dw  ?
	x dd  ?
	y db  ?
	i dw  ?
	j dw  ?
	k dw  ?
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
DEFINE_SCAN_NUM
END
