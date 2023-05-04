import React from 'react'
import '../index.css'

function Sidebar() {  
    return (    
        <div className='bg-white sidebar p-2'>
            <div className='m-2'>
                <i className='bi bi-bootstrap-fill me-3 fs-4'></i>
                <span className='brand-name fs-4'>Proyecto 2</span>
            </div>        
            <hr className='text-dark' />
            <div className='list-group list-group-flush'>            
            <a className='list-group-item py-2' href='/'>                
                <i className='bi bi-speedometer2 fs-5 me-3'></i>                
                <span >Dashboard</span>            
            </a>
            <a className='list-group-item py-2' href='/grafica1'>                
                <i className='bi bi-thermometer-sun fs-5 me-3'></i>                
                <span >Temperatura Externa</span>            
            </a>            
            <a className='list-group-item py-2' href='/grafica2'>                
                <i className='bi bi-thermometer-half fs-5 me-3'></i>                
                <span >Temperatura Interna</span>
            </a>            
            <a className='list-group-item py-2' href='/grafica3'>                
                <i className='bi bi-moisture fs-5 me-3'></i>                
                <span >Humedad de la Tierra</span>            
            </a>            
            <a className='list-group-item py-2' href='/grafica4'>                
                <i className='bi bi-droplet-half fs-5 me-3'></i>                
                <span >Porcentaje de Agua</span>            
            </a>            
            <a className='list-group-item py-2' href='/grafica5'>                
                <i className='bi bi-hourglass-bottom fs-5 me-3'></i>                
                <span >Periodo de Activacion</span>            
            </a>        
        </div>    
    </div>)
}
export default Sidebar