;Archivo: prueba.asm
;Compilado: 01/11/2022 09:13:23 a. m.
#make_COM#
include emu8086.inc
ORG 100H

;Variables: 

	area dd  ?
	radio dd  ?
	pi dd  ?
	resultado dd  ?
	a dw  ?
	d dw  ?
	altura dw  ?
	x dd  ?
	y dw  ?
	i dw  ?
	j dw  ?
	l dw  ?
	k dw  ?
	c db  ?
MOV AX,0
PUSH AX
POP AX
MOV y, AX
PRINTN "\n hola"
MOV AX,10
PUSH AX
POP AX
POP BX
CMP AX, BX
JG 
PRINTN "\n hola"
MOV AX,10
PUSH AX
POP AX
POP BX
CMP AX, BX
JG 
PRINTN "\n hola"
MOV AX,10
PUSH AX
POP AX
POP BX
CMP AX, BX
JG 
PRINTN "\n hola"
MOV AX,10
PUSH AX
POP AX
POP BX
CMP AX, BX
JG 
PRINTN "\n hola"
MOV AX,10
PUSH AX
POP AX
POP BX
CMP AX, BX
JG 
PRINTN "\n hola"
MOV AX,10
PUSH AX
POP AX
POP BX
CMP AX, BX
JG 
PRINTN "\n hola"
MOV AX,10
PUSH AX
POP AX
POP BX
CMP AX, BX
JG 
PRINTN "\n hola"
MOV AX,10
PUSH AX
POP AX
POP BX
CMP AX, BX
JG 
PRINTN "\n hola"
MOV AX,10
PUSH AX
POP AX
POP BX
CMP AX, BX
JG 
PRINTN "\n hola"
MOV AX,10
PUSH AX
POP AX
POP BX
CMP AX, BX
JG 
PRINTN "\n hola"
MOV AX,10
PUSH AX
POP AX
POP BX
CMP AX, BX
JG 
RET
DEFINE_SCAN_NUM
END
