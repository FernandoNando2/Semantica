;Archivo: prueba.asm
;Compilado: 24/10/2022 09:59:12 a. m.
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
MOV AX,0
PUSH AX
POP AX
MOV x, AX
MOV AX,5
PUSH AX
POP AX
POP BX
MOV AX,0
PUSH AX
POP AX
if2:
if1:
RET
END
