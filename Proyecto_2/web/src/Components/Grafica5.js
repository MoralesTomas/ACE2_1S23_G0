import React, { useEffect, useState } from 'react'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js';
import zoomPlugin from 'chartjs-plugin-zoom';
import { Line } from 'react-chartjs-2';
import Dropdown from 'react-bootstrap/Dropdown';
import axios from 'axios';

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  zoomPlugin
);

let options = {
  responsive: true,
  plugins: {
    legend: {
      position: 'top',
    },
    title: {
      display: false,
      text: '',
    },
    zoom: {
      pan: {
          enabled: true,
          mode: 'x'
      },
      zoom: {
          pinch: {
              enabled: true       // Enable pinch zooming
          },
          wheel: {
              enabled: true       // Enable wheel zooming
          },
          mode: 'x',
      }
  }
  },
};

let hostFechas = 'http://localhost:5000/fechasDisponibles'
let hostHorarios = 'http://localhost:5000/filtradoOficial'

export default function Grafica2() {
  let dataDefault = {
    labels: [],
    datasets: [
      {
        label: '',
        data: [],
        borderColor: 'rgb(255, 99, 132)',
        backgroundColor: 'rgba(255, 99, 132, 0.5)',
        fill: true,
        tension: 0.5,
      },
    ],
  };
  const [element, setElement] = useState([])
  const [fecha, setFecha] = useState("")
  const [data, setData] = useState(dataDefault)
  const [desde,setdesde]=useState("")
  const [hasta,sethasta]=useState("")
  useEffect(()=>{
    let fetchfechas = async()=>{
      let api = await axios.get(hostFechas);
      let apiFechas = api.data.map(item=>item)
      setElement(apiFechas)
    }
    fetchfechas()
  },[])
  let asignardatos = async () => {
    let dataproof2 = {
      labels: [],
      datasets: [
        {
          label: '',
          data: [],
          borderColor: 'rgb(255, 99, 132)',
          backgroundColor: 'rgba(255, 99, 132, 0.5)',
          fill: true,
          tension: 0,
        },
      ],
    };
    let api = await axios.post(hostHorarios,prepareRequest());
    let horariosApi = api.data
    for(let o of horariosApi){
      for(let oe of o["horario"]){
        dataproof2.datasets[0].data.push(o["estadoRiego"])
        dataproof2.labels.push(oe)
      }
    }
    dataproof2.datasets[0].label=fecha
    setData(dataproof2)
  }
  let getgetdesde=(e)=>{
    setdesde(e.target.value)
  }
  let getgethasta=(e)=>{
    sethasta(e.target.value)
  }
  let prepareRequest = () =>{
    return{
      fecha:fecha,
      hora_1:desde.substring(0,2),
      minuto_1:desde.substring(3,5),
      segundo_1:desde.substring(6,8),
      hora_2:hasta.substring(0,2),
      minuto_2:hasta.substring(3,5),
      segundo_2:hasta.substring(6,8),
    }
  }
  let handlerSelect=(e)=>{
    setFecha(element[e])
  }
  return (
    <div>
      <div className="container-fluid">
        <div className="row">
          <div className="col-md-12 p-1">
            <h1 className="text-white">Estado de Riego lo Largo del Tiempo</h1>
          </div>
        </div>
        <div className="row">
          <div className="col-md-2 p-1">
            <Dropdown onSelect={handlerSelect}>
              <Dropdown.Toggle id="dropdown-basic">
                Fechas Disponibles
              </Dropdown.Toggle>
              <Dropdown.Menu > 
                {element.map((v,i)=> <Dropdown.Item eventKey={i}>{v}</Dropdown.Item>)}
              </Dropdown.Menu>
            </Dropdown>
          </div>
          <div className="col-md-2 p-1"><h4>{fecha}</h4>
          </div>
          <div className="col-md-1 p-1"></div>
          <div className="col-md-1 p-1">
            <h3>Desde: </h3>
          </div>
          <div className="col-md-2 p-1">
            <input className='form-control' type="time" step="1" id='desde' onChange={getgetdesde} />
          </div>
          <div className="col-md-1 p-1">
            <h3>Hasta: </h3>
          </div>
          <div className="col-md-2 p-1">
            <input className='form-control' type="time" step="1" id='hasta' onChange={getgethasta} />
          </div>
          <div className="col-md-1 p-1">
            <button className='btn btn-success' onClick={asignardatos}>Buscar</button>
          </div>
        </div>
        <div className="row backwhite">
          <div className="col-md-12 p-1" id='chartproof'>
            <Line options={options} data={data} redraw />
          </div>
        </div>
      </div>
    </div>
  )
}
