;Archivo: prueba.asm
;Compilado: 21/10/2022 09:56:43 a. m.
#make_COM#
include emu8086.inc
ORG 100H

;Variables: 

	area db  ?
	radio db  ?
	pi db  ?
	resultado db  ?
	a db  ?
	d db  ?
	altura db  ?
	x db  ?
	y db  ?
	i db  ?
	j db  ?
MOV AX,3
PUSH AX
MOV AX,5
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
MOV AX,8
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
MOV AX,10
PUSH AX
MOV AX,4
PUSH AX
POP BX
POP AX
SUB AX,BX
PUSH AX
MOV AX,2
PUSH AX
POP BX
POP AX
DIV BX
PUSH AX
POP BX
POP AX
SUB AX,BX
PUSH AX
POP AX
MOV y, AX
RET
END
