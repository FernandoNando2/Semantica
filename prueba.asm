;Archivo: prueba.asm
;Compilado: 28/10/2022 09:39:04 a. m.
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
	l dw  ?
	k dw  ?
PRINTN "HOLA"
CALL SCAN_NUM
MOV radio, CX
RET
DEFINE_SCAN_NUM
END
