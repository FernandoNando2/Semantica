;Archivo: prueba.asm
;Compilado: 08/11/2022 10:53:52 a. m.
#make_COM#
include emu8086.inc
ORG 100H
MOV AX,0
PUSH AX
POP AX
MOV i, AX
inicioFor0:
MOV AX,i
PUSH AX
MOV AX,10
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finFor0
MOV AX,i
PUSH AX
POP AX
CALL PRINT_NUM
INC i
JMP inicioFor0
finFor0:

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
RET
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
DEFINE_SCAN_NUM
END
