import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'

// Ponto de entrada do frontend React.
// Cria a raiz da aplicação e renderiza o componente App dentro de StrictMode,
// garantindo avisos adicionais durante o desenvolvimento.
createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
