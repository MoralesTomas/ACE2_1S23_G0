import React, { useEffect, useState } from "react";
import axios from 'axios';

let hostAPI = ''

export default function Dashboard() {
    const [tempExt, setTempExt] = useState(0)
    const [tempInt, setTempInt] = useState(0)
    const [humidity, setHumidity] = useState(0)
    const [agua, setAgua] = useState(0)
    useEffect(() => {
        const api = async ()=>{
            let apidata = await axios.get(hostAPI)
            let apiDataData = apidata.data
            setTempExt(Number(apiDataData["valorTemperaturaExterna"]))
            setTempInt(Number(apiDataData["valorTemperaturaInterna"]))
            setHumidity(Number(apiDataData["valorHumedadExterna"]))
            setAgua(Number(apiDataData["porcentajeAguaDisponible"]))
        }
        const interval = setInterval(() => {
            api()
        }, 1000);
        return () => clearInterval(interval);
      }, []);
    return (
        <>
            <div className="container-fluid">
            <h1 className="text-white fs-4">Dashboard</h1>
                <div className="row g-3 my-2">
                    <div className="col-md-6 p-1">
                        <div className="p-3 bg-white shadow-sm d-flex justify-content-around align-items-center rounded">
                            <div>
                                <h3 className="fs-2">{`${tempExt}°`}</h3>
                                <p className="fs-5">Temperatura Externa</p>
                            </div>
                            <i className="bi bi-thermometer-sun p-3 fs-1"></i>
                        </div>
                    </div>
                    <div className="col-md-6 p-1">
                        <div className="p-3 bg-white shadow-sm d-flex justify-content-around align-items-center rounded">
                            <div>
                                <h3 className="fs-2">{`${tempInt}°`}</h3>
                                <p className="fs-5">Temperatura Interna</p>
                            </div>
                            <i className="bi bi-thermometer-half p-3 fs-1"></i>
                        </div>
                    </div>
                </div>
                <div className="row g-3 my-2">
                    <div className="col-md-6 p-1">
                        <div className="p-3 bg-white shadow-sm d-flex justify-content-around align-items-center rounded">
                            <div>
                                <h3 className="fs-2">{`${humidity}%`}</h3>
                                <p className="fs-5">Humedad</p>
                            </div>
                            <i className="bi bi-moisture p-3 fs-1"></i>
                        </div>
                    </div>
                    <div className="col-md-6 p-1">
                        <div className="p-3 bg-white shadow-sm d-flex justify-content-around align-items-center rounded">
                            <div>
                                <h3 className="fs-2">{`${agua}%`}</h3>
                                <p className="fs-5">Porcentaje de Agua</p>
                            </div>
                            <i className="bi bi-hourglass-bottom p-3 fs-1"></i>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}
