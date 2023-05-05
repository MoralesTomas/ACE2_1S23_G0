import React from 'react'
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Nav from './Navbar'
import '../index.css'
import Dashboard from './Dashboard'
import Grafica1 from './Grafica1'
import Grafica2 from './Grafica2'
import Grafica3 from './Grafica3'
import Grafica4 from './Grafica4'
import Grafica5 from './Grafica5'


function Home({ Toggle }) {
    return (
        <div className='px-3'>
            <Nav Toggle={Toggle} />
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<Dashboard/>} />
                    <Route path="/grafica1" element={<Grafica1 />} />
                    <Route path="/grafica2" element={<Grafica2 />} />
                    <Route path="/grafica3" element={<Grafica3 />} />
                    <Route path="/grafica4" element={<Grafica4 />} />
                    <Route path="/grafica5" element={<Grafica5 />} />
                </Routes>
            </BrowserRouter>
        </div>)
}
export default Home