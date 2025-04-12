import React from 'react'
import 'bootstrap/dist/css/bootstrap.min.css'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import Auth from './auth/Auth'
import Header from '../layouts/Header'

function App(){
    return (
        <Router>
            <Header />
            <Routes>
                <Route path='/' element={this} />
                <Route path='/auth/*' element={<Auth />} />
            </Routes>
        </Router>
    )
}

export default App