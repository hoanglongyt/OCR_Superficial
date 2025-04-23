import React from 'react'
import 'bootstrap/dist/css/bootstrap.min.css'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import Auth from './auth/Auth'
import Header from '../layouts/Header'
import Home from '../pages/Home/Home'
import Profile from '../pages/Profile/Profile'
import Process from '../pages/Process/Process'

function App(){
    return (
        <Router>
            <Header />
            <Routes>
                <Route path='/' element={<Home />} />
                <Route path='/process/*' element={<Process/>}/>
                <Route path='/profile/*' element={<Profile />} />
                <Route path='/auth/*' element={<Auth />} />
            </Routes>
        </Router>
    )
}

export default App