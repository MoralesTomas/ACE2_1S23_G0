/agregarRegistro

    Tipo:

        Post

    Parametros desde arduino:

    	ESTOS PARAMETROS DEBEN DE IR SIEMBRE DEFINIDOS.


        descansoLargo -> Variable booleana que indica si el registro actual es un descanso largo o si no lo es. Si no es un descansoLargo debera tener el valor false. DEBE DEFINIRSE

        descansoNormal -> Variable booleana que indica si el registro actual es un descanso normal o si no lo es. Si no es un descansoNormal debera tener el valor false. DEBE DEFINIRSE

        inicio -> Variable booleana, nos indica si el registro actual es el inicio o si no lo es, esto nos sirve para validar donde comeinza ya sea un pomodoro o un descanso. Si no es el inicio debera tener el valor false. DEBE DEFINIRSE

        	-->>Es decir que esta variable indica que el registro actual es el inicio del pomodoro o el inicio del descanso.

        fin -> Variable booleana, nos indica si el registro actual es el fin o si no lo es, esto nos sirve para validar donde termina ya sea un pomodoro o un descanso. Si no es el fin debera tener el valor false. DEBE DEFINIRSE

        	-->>Es decir que esta variable indica que el registro actual es el final del pomodoro o el final del descanso.

    	ACLARACION:::::::::::::::::::::::::::::::::::::::::::

    		Si es una interrupcion es decir si el usuario realiza una penalizacion entonces se enviara un registro pero los atributos inicio y fin deberan estar como falso.
    	FIN ACLARACION:::::::::::::::::::::::::::::::::::::::

        numeroPomodoro -> Variable entera que indica que numero de "pomodoro" es el que estamos trabajando, en caso de NO estar registrando un pomodoro y estar registrando un descanso este parametro debera contener un valor de menos uno ( -1 ). DEBE DEFINIRSE

        numeroDescanso -> Variable entera que indica que numero de "descanso" es el que estamos trabajando, en caso de no estar registrando un pomodoro y estar registrando un descanso este parametro debera contener un valor de menos uno ( -1 ). DEBE DEFINIRSE

        userName -> Variable de tipo string, nos indica el nombre del usuario que esta posteando este registro, si no esta definido se buscara en la configuracion de parametros de la BD. En caso de definirlo se tomara el valor que se este recibiendo.

        sentado -> Variable de tipo booleana que nos indica si el usuario esta sentado o si no lo esta. DEBE DEFINIRSE

    Ejemplo de consumo:

        http://localhost:5012/agregarRegistro

    Ejemplo de datos a enviar al endpoint :

        {
            "descansoLargo":false
            "descansoNormal":false,
            "inicio":true,
            "fin":false,
            "numeroPomodoro":1,
            "numeroDescanso": -1,
            "userName":"usuario1",
            "sentado":true
        }

ALGORITMO

INICIO: >Al iniciar la ejecucion del pomodoro primero arduino debera consumir los datos del endpoint 
"http://localhost:5012/datosUser" aca obtenendremos los datos para poder usar pomodoro, estos datos nos ayudan para poder obtener los valores de tiempo de la configuracion y el valor del nombre del usuario, este ultimo nos sirve pues al realizar un registro ( post ) necesitamos saber quien es el usuario que esta utilizando pomodoro.

    /datosUser

        Tipo:

            Get

        Parametros:

            Ninguno

        Descripcion:

            Este endpoint nos retorna los datos de la configuracion de la aplicacion, como el nombre de usuario y la configuracion de los pomodoros.

        Ejemplo de consumo:

            http://localhost:5012/datosUser

        Ejemplo de datos que podriamos recibir

            [
                {
                    "id": "a5408153-d0ca-4acc-838c-df985c4ad52f",
                    "userName": "Batman",
                    "valPomodoro": 25,
                    "valDescanso": 5
                }
            ]

> > UNA VEZ OBTENIDOS LOS DATOS PARA EL FUNCIONAMIENTO COMENZAMOS EL ALGORITMO<<<

POMODORO:

    1.Al comenzar un pomodoro o descanso se debera enviar un registro con el valor de true en el parametro "inicio".
    2.Si actualmente estamos en un pomodoro y el usuario se levanta ( detectado por el sensor ) se debera enviar un registro con los parametros ya mencionador pero con el valor false en el parametro "inicio" y "fin"
    3.Si actualmente estamos en un descanso se debera validar que el usuario NO este sentado, en caso de estar sentado se enviara un registro con los parametros ya mencionador pero con el valor false en el parametro "inicio" y "fin" y sus respecitivos atributos antes mencionados.
    4.Al terminar un pomodoro o un descanso se debera mandar un registro con el valor de true en el parametro "fin".
