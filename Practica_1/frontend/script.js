$(document).ready(function(){
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
});