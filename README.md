# Descripción
Esta es la segunda práctica de la asignatura **Inteligencia Artificial para Videojuegos** de la **Universidad Complutense de Madrid**. 

Utilizamos pivotal tracker como herramienta de gestión del proyecto. Puedes ver nuestra organización [aquí](https://www.pivotaltracker.com/n/projects/2490634)

Consiste en la implementación de entidades con inteligencia artificial siguiendo el esquema y la estructura planteada en el enunciado **IAV-Práctica-2.pdf**.

Plantea un ejercicio básico basado en **Teseo,el minotauro y el hilo de Ariadna**. El usuario controlara a Tesea a traves del laberinto con las teclas WASD o con las flechas de dirección, hasta que se pulse la <u>Barra espaciadora</u> en ese momento,se activará el hilo de Ariadna y el jugador pasara a estar controlada por la maquina, siguiendo el camino generado por el algoritmo A* hasta la salida, continuará asi hasta que llegue a la salida o hasta que el jugador deje de pulsar la barra espaciadora.
Si teseo se encuentra con el minotauro, este empezara a seguirle por todo el laberinto hasta que le pierda de vista.

# Documentación técnica del proyecto

El proyecto está implementado con **Unity** y la documentación del código la generamos en español, así como la mayoría de variables, métodos y clases.

## 1 Mecánicas
- **Flechas de dirección**: encargadas del movimiento del jugador(tambien WASD)

        Flecha arriba -> movimiento en el eje Z positivo
        Flecha abajo -> movimiento en el eje Z negativo
        Flecha derecha -> movimiento positivo en el eje X
        Flecha izquierda -> movimiento negativo en el eje X

- **Barra espaciadora**: sirve para al hilo de Ariadna

        No pulsado -> el jugador es controlado por las fisicas, movido por el input del jugador
        Manteniendo pulsado -> HIlo activo, el jugador es guiado por la maquina

## 2 Entidades y escenario

### 2.1 Entidades
- **Teseo**: (<u>Personaje Azul</u>) es el personaje principal y aldedor del cual gira todo el ejercicio. 

- **Minotauro**:(<u>Personaje Amarillo</u>) es una **IA** básica de movimiento. De forma general realizará un movimiento de merodeo por todo el Laberinto. Si teseo entra dentro de su rango de visión, pasará a seguirle de forma automatica hasta que vuelva a perderlo de vista; 

- **Hilo de Ariadna**: es una **IA** de movimiento,encargada de calcular el camino hasta la salida desde la posicion del jugador y de guiarle de forma automática.Se representa en el escenario como un camino de baldosas rojas que iran desapareciendo segun el jugador las valla recorriendo


### 2.2 Escenario

El escenario trata de simular un **Laberinto** de manera que hay obstáculos(muros del laberinto). Las entidades no pueden atravesar los obstáculos, de manera que solo puedan circular por las zonas libres de estos. El minotauro,controlado mediante IA, no detectan dichos obstáculos,por lo que puede chocarse con ellos.
El hilo de Ariadna, por el contrario, genera un camino que sí tiene en cuenta los muros y los esquiva;

**El Laberinto** se genera automáticamente Siguiendo el diseño escrito en un documento .map.Que permite la facil manipulación de los obstaculos.

## 3. Implementación de **IA**

Para implementar el comportamiento descrito en el enunciado de la práctica hemos desarollado los siguientes comportamientos:

### 3.1 Script Graph
Se encarga de toda la representación interna del grafo, en la cual nosotros hemos incluido todo lo necesario para la implementacion del algoritmo A*

### 3.2 Script TestGraph
Encargado de llamar al algoritmo A*, de pintar el camino, de borrarlo y de avisar al jugador de cuando se ha activado el hilo


### 3.3 PlayerMovable
Se encarga de Manejar al jugador, tanto de procesar el input y añadirle las fisicas para el movimiento por control del jugador, como de la parte del seguimiento automatico del camino generado por A* cuando se pulsa la Barra espaciadora


### 3.4 Script GraphGrid
Encargado de pintar el Laberinto, en el que hicimos modificaciones para que tambien se crearan de forma aytomatica el player y el minotauro en la posición del mapa que espacificaramos en el documento .map. 

### 3.5 Script MinotaurVisión
Encargado de la detección del jugador y de activar el modo de seguimiento.

### 3.6 Script MinotaurMovable
En el que se describe la IA del minotauro, se encarga de hacer que el minotauro sigua un movimiento de merodeo aleatorio, cambiando de dirección cada vez que se choca con una pared. Además se encarga de que sigua al jugador cuando entra en su rango de visión;

### 3.7 Script PriorityQueue
Plantilla de una Cola de prioridad implementada en C# a la que le hicimos algunas modificaciones para adaptarla a las necesidades de nuestro código y que utilizamos para la facilitar la implementación del algoritmo A*


## 4 Referencias
- Pseudocódigo del libro: [**AI for Games, Third Edition**](https://ebookcentral.proquest.com/lib/universidadcomplutense-ebooks/detail.action?docID=5735527) de **Millington**

- Código de [**Federico Peinado**](https://github.com/federicopeinado) habilitado para la asignatura.
