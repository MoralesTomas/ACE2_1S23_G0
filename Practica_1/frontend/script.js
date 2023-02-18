$(document).ready(function(){
    let days = ['Domingo', 'Lunes', 'Martes', 'Miercoles', 'Jueves', 'Viernes', 'Sabado'];
    let months = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];

    function getDate(){
        const d = new Date();
        printDate = days[d.getDay()] + ", " + d.getDate() + " de " + months[d.getMonth()];
        $('#fecha').text(printDate);
    }

    function Reloj(){
        const now = new Date();
        let printTime = now.getHours() + ":" + now.getMinutes();
        $("#hora").text(printTime);
        setTimeout(Reloj, 1000);
    }
    Reloj();
    getDate();
    //Evento click en boton de menu
    $('#btn').click(function(){
        /* Compruebo si el menu lateral está contraido */
        if ($('#sidebar').hasClass('contraer-sidebar')){
            //Realizo la animación de abrir menú
            $('#sidebar').animate({ 
                width: '250px' 
            }, 500, ()=>{
                //$('.links-name').slideDown(100); //Muestro las opciones
            });
            $('#sidebar').removeClass('contraer-sidebar'); //Remuevo la clase contraer
            $('.links-name').slideDown(500); //Muestro las opciones
            /* Muevo el contenido de la seccion main a la derecha */
            $('#main').animate({ 
                left: '250px'
            }, 500);
            $('#main').css({width: 'calc(100% - 250px)'});
        }else{
            $('#sidebar').addClass('contraer-sidebar'); //Añado la clase contraer sidebar
            //Reduzco el tamaño del menu lateral
            $('#sidebar').animate({
                width: '65px',
            });
            $('.links-name').slideUp(); //Escondo las opciones
            /* Muevo el contenido de la seccion main a la izquierda */
            $('#main').animate({
                width: 'calc(100% - 65px)', 
                left: '65px'
            }, 500, function(){
                $('#main').css({width: 'calc(100% - 65px)'});
            });
        }
    });

    $('#dashb').click((e)=>{
        e.preventDefault();
        $('.dashboard').slideDown(800);
        $('.dashboard').siblings().slideUp(800);
    });

    $('#temperatura').click((e)=>{
        e.preventDefault();
        $('.expt1').slideDown(800);
        $('.expt1').siblings().slideUp(800);
    });

    $('#humedad').click((e)=>{
        e.preventDefault();
        $('.expt2').slideDown(800);
        $('.expt2').siblings().slideUp(800);
    });

    $('#velocidad').click((e)=>{
        e.preventDefault();
        $('.expt3').slideDown(800);
        $('.expt3').siblings().slideUp(800);
    });

    $('#direccion').click((e)=>{
        e.preventDefault();
        $('.expt4').slideDown(800);
        $('.expt4').siblings().slideUp(800);
    });

    $('#presion').click((e)=>{
        e.preventDefault();
        $('.expt5').slideDown(800);
        $('.expt5').siblings().slideUp(800);
    });

    $('#graficaR').click((e)=>{
        e.preventDefault();
        $('.expt6').slideDown(800);
        $('.expt6').siblings().slideUp(800);
    });

    /* $('#dash1').click((e) => {
        e.preventDefault();
        $('.expt1').slideDown(800);
        $('.expt1').siblings().slideUp(800);
    });

    $('#dash2').click((e) => {
        e.preventDefault();
        $('.expt2').slideDown(800);
        $('.expt2').siblings().slideUp(800);
    });

    $('#dash3').click((e) => {
        e.preventDefault();
        $('.expt3').slideDown(800);
        $('.expt3').siblings().slideUp(800);
    });

    $('#dash4').click((e) => {
        e.preventDefault();
        $('.expt4').slideDown(800);
        $('.expt4').siblings().slideUp(800);
    });

    $('#dash5').click((e) => {
        e.preventDefault();
        $('.expt6').slideDown(800);
        $('.expt6').siblings().slideUp(800);
    });

    $('#dash6').click((e) => {
        e.preventDefault();
        $('.expt5').slideDown(800);
        $('.expt5').siblings().slideUp(800);
    }); */

    /* DASHBOARD */
    $('.temperatura').click((e) => {
        e.preventDefault();
        $('.expt1').slideDown(800);
        $('.expt1').siblings().slideUp(800);
    });

    $('.magnitud-vel').click((e) => {
        e.preventDefault();
        $('.expt4').slideDown(800);
        $('.expt4').siblings().slideUp(800);
    });

    $('.dir').click((e) => {
        e.preventDefault();
        $('.expt6').slideDown(800);
        $('.expt6').siblings().slideUp(800);
    });

    $('.magnitud-presion').click((e) => {
        e.preventDefault();
        $('.expt5').slideDown(800);
        $('.expt5').siblings().slideUp(800);
    });

    $('.magnitud-hAbs').click((e) => {
        e.preventDefault();
        $('.expt2').slideDown(800);
        $('.expt2').siblings().slideUp(800);
    });

    $('.magnitud-hRel').click((e) => {
        e.preventDefault();
        $('.expt3').slideDown(800);
        $('.expt3').siblings().slideUp(800);
    });

    /* TRAER DATOS */
    function getTemperatura(){
        let data = Object.values(datos);
        return data.temperatura;
    }

    function getHumedadRel(){
        let data = Object.values(datos);
        return data.humedadRelativa;
    }

    function getHumedadAbs(){
        let data = Object.values(datos);
        return data.humedadAbsoluta;
    }
    
    function getVelViento(){
        let data = Object.values(datos);
        return data.velocidad;
    }

    function getDireccion(){
        let data = Object.values(datos);
        return data.direccion;
    }

    function getPresion(){
        let data = Object.values(datos);
        return data.presion;
    }

    var marksCanvas = document.getElementById("marksChart");

    var marksData = {
    labels: ["English", "Maths", "Physics", "Chemistry", "Biology", "History"],
    datasets: [{
        label: "Student A",
        backgroundColor: "rgba(200,0,0,0.2)",
        data: [65, 75, 70, 80, 60, 80]
    }, {
        label: "Student B",
        backgroundColor: "rgba(0,0,200,0.2)",
        data: [54, 65, 60, 70, 70, 75]
    }]
    };

    var radarChart = new Chart(marksCanvas, {
    type: 'radar',
    data: marksData
    });
      

});