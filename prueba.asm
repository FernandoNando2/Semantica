;Archivo: prueba.asm
;Compilado: 25/10/2022 09:55:35 a. m.
#make_COM#
include emu8086.inc
ORG 100H

;Variables: 

	area dw  ?
	radio dw  ?
	pi dw  ?
	resultado dw  ?
	a dw  ?
	d dw  ?
	altura dw  ?
	x dw  ?
	y dw  ?
	i dw  ?
	j dw  ?
MOV AX,61
PUSH AX
POP AX
MOV y, AX
MOV AX,61
PUSH AX
POP AX
POP BX
CMP AX, BX
JNE if1
MOV AX,10
PUSH AX
POP AX
MOV x, AX
if1:
RET
END
