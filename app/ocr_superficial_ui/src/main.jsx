import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './components/App'
import './main.css'
import { UserProvider } from './contexts/userContext'
import { ImageProvider } from './contexts/ImageContext'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <ImageProvider>
      <UserProvider>
        <App />
      </UserProvider>
    </ImageProvider>
  </StrictMode>,
)
