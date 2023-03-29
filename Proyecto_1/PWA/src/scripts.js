$(document).ready(function(){
    let btnSettings = document.querySelector('#btnSettings');
    let btnReport = document.querySelector('#btnReportes');
    let modalSettings = document.querySelector('#modalSettings');
    let modalReport = document.querySelector('#modalReportes');
    $('#login').hide();
    
    //Esconde el modal
    $('.closeModal').click(() => {
        $('.closeModal').closest(".modal").addClass("hide");
    });

    //Muestra el modal de ajustes
    $(btnSettings).click(() => {
        modalSettings.classList.toggle("hide");
    });

    //Muestra el modal de ajustes
    $(btnReport).click(() => {
        modalReport.classList.remove("hide");
    });

    //Muestra o esconde el login
    $('#btnLogin').click(()=>{
        $('#timer-box').slideToggle(500);
        $('#login').slideToggle(500);
    });

    /*------------------------- Establece el tiempo de pomodoro -------------------------*/
    $('#pomodoroValue').change(() => {
        if ($('#pomo-timer').hasClass('active-timer')){
            $('#timerValue').text(addZeroifNecessary($('#pomodoroValue').val()) + ":00");
        }
    });

    $('#breakValue').change(() => {
        if ($('#short-timer').hasClass('active-timer')){
            $('#timerValue').text(addZeroifNecessary($('#breakValue').val()) + ":00");
        }
    });

    $('#longBreakValue').change(() => {
        if ($('#long-timer').hasClass('active-timer')){
            $('#timerValue').text(addZeroifNecessary($('#longBreakValue').val()) + ":00");
        }
    });

    /* ---------------------------- CRONOMETRO ---------------------------- */
    let idInterval;
    let tiempoActual = 0;

    //Añade un cero a la izquierda si el numero ingresado es menor o igual a 9
    const addZeroifNecessary = value => {
		if (value < 10) {
			return "0" + value;
		} else {
			return "" + value;
		}
	}

    //Convierte milisegundos a minutos y segundos, y lo retorna en un formato especifico
    const convertTimeToMinNSec = (milisegundos) => {
		const minutos = parseInt(milisegundos / 1000 / 60);
		milisegundos -= minutos * 60 * 1000;
		segundos = (milisegundos / 1000);
		return `${addZeroifNecessary(minutos)}:${addZeroifNecessary(parseInt(segundos))}`;
	};

    //Actualizar el tiempo (los milisegundos)
    //Cuando llega a cero se pasa al siguiente
    //!Corroboar que no inicie de nuevo el pomodoro
    const updateTime = () => {
        tiempoActual -= 100;
		$('#timerValue').text(convertTimeToMinNSec(tiempoActual));
        if (tiempoActual <= 0){
            init();
            toggleClass();
            init();
            reset();
            setTimeout(start, 1000);
        }
	};

    //Inicia el cronometro
    const start = () => {
		clearInterval(idInterval);
		idInterval = setInterval(updateTime, 100);
		$('#btn-start').removeClass('start');
        $('#btn-start').addClass('pause');
        $('#btn-start').text("PAUSE");
        $('#btn-stop').removeClass('hide');
	};

    //Pausa el cronometro
    const pause = () => {
		clearInterval(idInterval);
		$('#btn-start').removeClass('pause');
        $('#btn-start').addClass('start');
        $('#btn-start').text("START");
	};

    //Detiene el cronometro
    const stop = () => {
		if (!confirm("¿Seguro que desea detener el pomodoro?")) {
			return;
		}
		init();
	}

    //Inicializa el cronometro a default
    const init = () => {
        clearInterval(idInterval);
        if ($('#pomo-timer').hasClass('active-timer')){
            $('#timerValue').text(addZeroifNecessary($('#pomodoroValue').val()) + ":00");
        }else if ($('#short-timer').hasClass('active-timer')){
            $('#timerValue').text(addZeroifNecessary($('#breakValue').val()) + ":00");
        }else if ($('#long-timer').hasClass('active-timer')){
            $('#timerValue').text(addZeroifNecessary($('#longBreakValue').val()) + ":00");
        }
        $('#btn-start').addClass('start');
        $('#btn-start').text("START");
		tiempoActual = 0;
        $('#btn-stop').addClass('hide');
    }

    //Alterna la clase active-timer entre pomodoro y descanso
    const toggleClass = () => {
        if ($('#pomo-timer').hasClass('active-timer')){
            $('#short-timer').addClass('active-timer');
            $('#pomo-timer').removeClass('active-timer');
        }else if ($('#short-timer').hasClass('active-timer')){
            $('#pomo-timer').addClass('active-timer');
            $('#short-timer').removeClass('active-timer');
        }else if ($('#long-timer').hasClass('active-timer')){

        }
    }

    //Resetea el cronometro
    const reset = () => {
        if ($('#pomo-timer').hasClass('active-timer')){
            if (tiempoActual == 0) tiempoActual = parseInt($('#pomodoroValue').val()) * 60000;
        }else if ($('#short-timer').hasClass('active-timer')){
            if (tiempoActual == 0) tiempoActual = parseInt($('#breakValue').val()) * 60000;
        }else if ($('#long-timer').hasClass('active-timer')){
            if (tiempoActual == 0) tiempoActual = parseInt($('#longBreakValue').val()) * 60000;
        }
    }

    //Evento de un boton para iniciar o pausar el cronometro 
    $('#btn-start').click(() => {
        if ($('#btn-start').hasClass('start')){
            reset();
            start();
        }else if ($('#btn-start').hasClass('pause')){
            pause();
        }
    });

    //Evento de un boton para detener el cronometro
    $('#btn-stop').click(() => {
        stop();
        reset();
        toggleClass();
        init();
    });

    /* --------------------------------------------------------------------------- */
    /* PESTAÑAS DEL TIMER */
    $('#pomo-timer').click(() => {
        $('#pomo-timer').addClass('active-timer');
        $('#short-timer').removeClass('active-timer');
        $('#long-timer').removeClass('active-timer');
        stop();
    });

    $('#short-timer').click(() => {
        $('#short-timer').addClass('active-timer');
        $('#pomo-timer').removeClass('active-timer');
        $('#long-timer').removeClass('active-timer');
        stop();
    });

    $('#long-timer').click(() => {
        $('#long-timer').addClass('active-timer');
        $('#pomo-timer').removeClass('active-timer');
        $('#short-timer').removeClass('active-timer');
        stop();
    });

    /* ----------------------------------------------------------------------------- */
    /* SELECT GRAPH */
    $('#graficas').change(() => {
        if ($('#graficas').val() == "graph1"){
            $('#graph-live').siblings().slideUp(500);
            $('#graph-live').slideDown(500);
        }else if($('#graficas').val() == "graph2"){
            $('#graph-pen-parado').siblings().slideUp(500);
            $('#graph-pen-parado').slideDown(500);
        }else if ($('#graficas').val() == "graph3"){
            $('#graph-pen-sentado').siblings().slideUp(500);
            $('#graph-pen-sentado').slideDown(500);
        }else if ($('#graficas').val() == "graph4"){
            $('#graph-pen-parado-inv').siblings().slideUp(500);
            $('#graph-pen-parado-inv').slideDown(500);
        }else if ($('#graficas').val() == "graph5"){
            $('#graph-pen-sentado-inv').siblings().slideUp(500);
            $('#graph-pen-sentado-inv').slideDown(500);
        }else if ($('#graficas').val() == "graph6"){
            $('#graph-historical').siblings().slideUp(500);
            $('#graph-historical').slideDown(500);
        }else if ($('#graficas').val() == "graph7"){
            $('#graph-penalizaciones').siblings().slideUp(500);
            $('#graph-penalizaciones').slideDown(500);
        }
        
    });
    $('.container-table').hide();
    $('#graph-live').slideDown();

    /* ACTUALIZACION DE DATOS Y NOMBRE USUARIO */
    async function updateUserData(){
        userData = {
            "nameUser": sessionStorage.getItem('user-name'),
            "nuevoValPomodoro": $('#pomodoroValue').val(),
            "nuevoValDescanso": $('#breakValue').val(),
            "nuevoValDescansoLargo": $('#longBreakValue').val()
        };
        await fetch('http://localhost:5000/actualizarParametrosApp', {
            method: 'PUT',
            body: JSON.stringify(userData),
            headers: {
                'Content-type': 'application/json'
            }
        }).then(res => res.json)
        .catch(err => console.error('ERROR: ', err))
        .then(response => console.log('Success: ', response));
    }

    /* SESSION STORAGE */
    $('#btn-login').on('click', (e) => {
        e.preventDefault();
        if ($('#user-name').val() != ""){
            sessionStorage.removeItem('user-name');
            sessionStorage.setItem('user-name', $('#user-name').val());
            updateUserData();
            alert('Nueva sesion iniciada');
        }else{
            alert('Llene el campo usuario');
        }
        // console.log(sessionStorage.getItem('user-name'));
    });
    $('#user-name').val(sessionStorage.getItem('user-name'));

});