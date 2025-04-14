import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './components/App'
import './main.css'
import { UserProvider } from './contexts/userContext'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <UserProvider>
      <App />
    </UserProvider>
  </StrictMode>,
)
