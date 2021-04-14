![Captura de pantalla 2021-04-14 183856](https://i.imgur.com/o0GPxX8.jpg)
# Descripción
Esta es la segunda práctica de la asignatura **Inteligencia Artificial para Videojuegos** de la **Universidad Complutense de Madrid**. 

Utilizamos pivotal tracker como herramienta de gestión del proyecto. Puedes ver nuestra organización [aquí](https://www.pivotaltracker.com/n/projects/2490634)

Consiste en la implementación de entidades con inteligencia artificial siguiendo el esquema y la estructura planteada en el enunciado **IAV-Práctica-2.pdf**.

Plantea un ejercicio básico basado en **Teseo,el minotauro y el hilo de Ariadna**. El usuario controlara a Teseo a traves del laberinto con las teclas WASD o con las flechas de dirección, hasta que se pulse la <u>Barra espaciadora</u> en ese momento,se activará el hilo de Ariadna y el jugador pasara a estar controlado por la máquina, siguiendo el camino generado por el algoritmo A* hasta la salida mientras que el jugador mantenga pulsada la barra espaciadora.
Si Teseo se encuentra con el minotauro, este empezará a seguirle por todo el laberinto hasta que le pierda de vista.

# Documentación técnica del proyecto

El proyecto está implementado con **Unity** y la documentación del código la generamos en español, así como la mayoría de variables, métodos y clases.

## 1 Mecánicas
- **Flechas de dirección o WADS**: encargadas del movimiento del jugador

        Flecha arriba (W) -> movimiento en el eje Z positivo
        Flecha abajo (S)-> movimiento en el eje Z negativo
        Flecha derecha (D)-> movimiento positivo en el eje X
        Flecha izquierda (A)-> movimiento negativo en el eje X

- **Barra espaciadora**: sirve para activar el hilo de Ariadna

        No pulsado -> el jugador es controlado por las fisicas, movido por el input del jugador
        Manteniendo pulsado -> Hilo activo, el jugador es guiado por la máquina

## 2 Entidades y escenario

### 2.1 Entidades
- **Teseo**: (<u>Personaje Azul</u>) es el personaje principal y alrededor del cual gira todo el ejercicio. 

- **Minotauro**:(<u>Personaje Amarillo</u>) es una **IA** básica de movimiento. De forma general realizará un movimiento de merodeo por todo el Laberinto. Si Teseo entra dentro de su rango de visión, pasará a seguirle de forma automática hasta que vuelva a perderlo de vista; 

- **Hilo de Ariadna**: es una **IA** de movimiento,encargada de calcular el camino hasta la salida desde la posición del jugador y de guiarle de forma automática.Se representa en el escenario como un camino de baldosas rojas que iran desapareciendo segun el jugador las vaya recorriendo.


### 2.2 Escenario

El escenario trata de simular un **Laberinto** de manera que hay obstáculos(muros del laberinto). Las entidades no pueden atravesar los obstáculos, de manera que solo puedan circular por las zonas libres de estos. El minotauro,controlado mediante IA, detecta dichos obstáculos, de forma que cuando tiene un muro de frente, gira para evitarlo.
El hilo de Ariadna,genera un camino que sí tiene en cuenta los muros y los esquiva;

**El Laberinto** se genera automáticamente, siguiendo el diseño escrito en un documento .map, el cual permite la fácil manipulación de los obstáculos.

## 3. Implementación de **IA**

Para implementar el comportamiento descrito en el enunciado de la práctica hemos desarollado los siguientes comportamientos:

### 3.1 Script Graph
Clase base que venía incluida en la plantilla del proyecto. Se encarga de toda la representación interna del grafo, en la cual nosotros hemos incluido todo lo necesario para la implementacion del algoritmo A*. También es la clase que calcula el número de nodos explorados y el tiempo invertido en dicha exploración.

### 3.2 Script Ariadna
Clase base que venía incluida en la plantilla del proyecto a la cual se le han realizado algunos cambios. Es la encargada de llamar al algoritmo A*, de pintar el camino, de borrarlo y de avisar al jugador de cuando se ha activado el hilo para que así avance de forma cinemática siguiendo dicho camino.


### 3.3 Script GraphGrid
Clase base que venía incluida en la plantilla del proyecto. Se encarga de pintar el laberinto. Hicimos modificaciones para que tambien se crearan mediante fichero el player y el minotauro en la posición del mapa que especifiquemos en el documento .txt. Otra de las modificaciones que implementamos fue una nueva lectura de ficheros de mapas puesto que al hacer la build nos daban problemas en la localización de los assets de los mapas. Para ello usamos un objeto de la clase TextAsset configurable a través del editor de Unity que pasando un txt normal con el mapa a pintar genera el mapa.

### 3.4 PlayerMovable
Se encarga de manejar el movimiento del jugador. Procesa el input del usuario y, si el hilo de Ariadna no está activo, añadirle las fuerzas correspondientes para moverlo. También define métodos para activar el movimiento controlado por la máquina, recibir el camino más corto generado por el algoritmo A* y ir avanzando siguiendo dicho camino que recibe.

### 3.5 Script MinotaurVisión
Encargado de la detección del jugador y de activar el modo de seguimiento. Consiste en dos métodos, un OnCollisionEnter y un OnCollisionExit que se comunican con el comportamiento del movimiento del minotauro (Script MinotaurMovable) para activar el modo de seguimiento en dicho script cuando el objeto que ha entrado dentro del rango de visión del minotauro ha sido Teseo.

### 3.6 Script MinotaurMovable
En el que se describe la IA del movimento del minotauro. Implementa un comportamiento de merodeo aleatorio, el cual hace que el minotauro cambie de dirección cada vez que detecta una pared mediante Raycast. Dicha implementación del movimiento consiste en cambiar el transform.forward del minotauro de forma aleatoria a una de las cuatro posibles direcciones básicas: back, left, forward y right. Además se encarga de que siga al jugador cuando este entra en su rango de visión.

### 3.7 Script PriorityQueue
Plantilla de una cola de prioridad implementada en C# a la que le hicimos algunas modificaciones para adaptarla a las necesidades de nuestro código y que utilizamos para la facilitar la implementación del algoritmo A*

## 4 Pruebas realizadas

Para comprobar el funcionamiento de A*, hemos realizado diversas pruebas en entornos controlados por nosotros mismos para evaluar la corrección de el algoritmo. Para ello, hemos desactivado el comportamiento de movimiento y la visión del Minotauro (para que cuando nos acercásemos mucho a él no nos detectase).

### 4.1 Prueba en Entorno Vacío

El objetivo de esta prueba es evaluar si, una vez encontrándonos en línea recta con el minotauro activásemos el hilo de Ariadna en un entorno carente de obstáculos esquivaríamos los bloques que cuentan con un coste mayor puesto que son los adyacentes al minotauro (y que en el vídeo aparecen representados de color verde). Es una de las primeras comprobaciones que hicimos para evaluar que los costes de las baldosas funcionaban correctamente. Haz **click en la imagen** que aparece a continuación para ver el **vídeo de la prueba**:

[![alt text](http://img.youtube.com/vi/QxFtuxsBPC8/0.jpg)](https://www.youtube.com/watch?v=QxFtuxsBPC8&ab_channel=IA_UCM_Tester "Click para ir al video: Prueba: Entorno Vacío")

### 4.1 Prueba en Pasillo abierto

Similar en esencia a la prueba del apartado anterior, puesto que se distingue en que ahora el jugador se encuentra en un pasillo recto junto con el minotauro. Debido a que el coste de pasar por al lado del minotauro es elevado (o imposible si el pasillo es de una casilla de ancho, puesto que la casilla del minotauro no se puede atravesar), lo más lógico es que Teseo de media vuelta y llegue a la casilla de salida por el exterior de dicho pasillo puesto que este está abierto por dos extremos. Haz **click en la imagen** que aparece a continuación para ver el **vídeo de la prueba**:

[![alt text](http://img.youtube.com/vi/XORXvt9_bjg/0.jpg)](https://youtu.be/XORXvt9_bjg "Click para ir al video: Prueba: Pasillo abierto")

## 5 Recursos de terceros empleados
- Pseudocódigo del libro: [**AI for Games, Third Edition**](https://ebookcentral.proquest.com/lib/universidadcomplutense-ebooks/detail.action?docID=5735527) de **Millington**

- Código de [**Federico Peinado**](https://github.com/federicopeinado) habilitado para la asignatura.

- [**Mixamo**](https://www.mixamo.com/) para las mayas y las animaciones empleadas como assets. 
