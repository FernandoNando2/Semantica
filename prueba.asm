;Archivo: prueba.asm
;Compilado: 26/10/2022 10:00:01 a. m.
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
inicioFor0:
MOV AX,0
PUSH AX
POP AX
MOV i, AX
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
inicioFor1:
MOV AX,0
PUSH AX
POP AX
MOV j, AX
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX,1
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV x, AX
inicioFor2:
MOV AX,0
PUSH AX
POP AX
MOV k, AX
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
inicioFor3:
MOV AX,0
PUSH AX
POP AX
MOV l, AX
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX,2
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV y, AX
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX,2
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV y, AX
finFor3:
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
inicioFor4:
MOV AX,0
PUSH AX
POP AX
MOV l, AX
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX,2
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV y, AX
finFor4:
finFor2:
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX,1
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV x, AX
inicioFor5:
MOV AX,0
PUSH AX
POP AX
MOV k, AX
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
inicioFor6:
MOV AX,0
PUSH AX
POP AX
MOV l, AX
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX,2
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV y, AX
finFor6:
finFor5:
finFor1:
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
inicioFor7:
MOV AX,0
PUSH AX
POP AX
MOV j, AX
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX,1
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV x, AX
inicioFor8:
MOV AX,0
PUSH AX
POP AX
MOV k, AX
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
inicioFor9:
MOV AX,0
PUSH AX
POP AX
MOV l, AX
MOV AX,1
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE 
MOV AX,2
PUSH AX
POP BX
POP AX
ADD AX,BX
PUSH AX
POP AX
MOV y, AX
finFor9:
finFor8:
finFor7:
finFor0:
RET
END
