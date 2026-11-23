# Flappy Bird - Activitat Unity

## Autor
Hug Ribera

## Versió de Unity
Unity 6.3 LTS (6000.3.25f1)

## Controls

### PC
- Barra espaiadora: impuls de l'ocell.

### Android
- Tocar la pantalla: impuls de l'ocell.
El projecte ha estat compilat i provat en un dispositiu Android mitjançant un APK funcional.

## Funcionalitats implementades

### Fons amb efecte de paral·laxi
S'ha implementat un desplaçament continu del fons a una velocitat inferior a la de les canonades per generar un efecte de profunditat. El moviment s'atura quan finalitza la partida.

### Puntuació i rècord
El joc mostra la puntuació actual i el millor rècord aconseguit. El rècord es desa mitjançant PlayerPrefs, de manera que es conserva entre execucions del joc.

### Nou rècord
Quan el jugador supera el rècord anterior es mostra un missatge de "NEW RECORD" i es reprodueix el so corresponent.

### Efectes de so
S'han implementat sons per als següents esdeveniments:
- Impuls de l'ocell.
- Col·lisió.
- Inici de partida.
- Nou rècord.
- Punt aconseguit.

### Animacions
L'ocell disposa d'una animació de vol i una animació de mort que s'activen segons l'estat de la partida.

### Inclinació
L'ocell s'inclina cap amunt quan puja i cap avall quan cau, amb una transició suau basada en la seva velocitat vertical.

## Ajustos de jugabilitat

S'han ajustat diversos paràmetres per aproximar el comportament al Flappy Bird original:

- Gravity Scale: 2
- Force: 700
- Velocitat de les canonades: 0.05
- Temps d'aparició de canonades: 1.5 segons

Aquests valors proporcionen un equilibri entre dificultat i control, mantenint una resposta immediata als impulsos i una caiguda progressiva de l'ocell.