import React from 'react'
import 'bootstrap/dist/css/bootstrap.min.css'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import Auth from './auth/Auth'
import Header from '../layouts/Header'
import Home from '../pages/Home/Home'
import Profile from '../pages/Profile/Profile'
import Process from '../pages/Process/Process'
import PdfConversionLayout from './pdf/PdfConversionLayout'

function App(){
    return (
        <Router>
            <Header />
            <Routes>
                <Route path='/' element={<Home />} />
                <Route path='/process/*' element={<Process/>}/>
                <Route path='/profile/*' element={<Profile />} />
                <Route path='/auth/*' element={<Auth />} />

                {/* Document Conversions */}
                <Route path="/pdf-to-word" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-excel" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-powerpoint" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-html" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-text" element={<PdfConversionLayout />} />

                {/* Image Conversions */}
                <Route path="/pdf-to-png" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-jpg" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-tiff" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-webp" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-svg" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-gif" element={<PdfConversionLayout />} />

                {/* Ebook Conversions */}
                <Route path="/pdf-to-epub" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-mobi" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-azw3" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-fb2" element={<PdfConversionLayout />} />

                {/* Other Formats */}
                <Route path="/pdf-to-markdown" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-rtf" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-tex" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-ps" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-dxf" element={<PdfConversionLayout />} />
                <Route path="/pdf-to-eps" element={<PdfConversionLayout />} />
            </Routes>
        </Router>
    )
}

export default App