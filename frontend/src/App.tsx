import { BrowserRouter, NavLink, Route, Routes } from 'react-router-dom';
import { DashboardTotais } from './components/DashboardTotais';
import { PessoasForm } from './components/PessoasForm';
import { TransacoesForm } from './components/TransacoesForm';

// App é o componente raiz do frontend.
// Ele configura o roteamento entre as principais telas da aplicação,
// garantindo que a navegação seja feita sem recarregar a página.
function App() {
  return (
    <BrowserRouter>
      <div className="app-shell">
        <nav className="app-nav">
          <NavLink to="/" end className={({ isActive }) => `app-nav-link ${isActive ? 'active' : ''}`}>
            Dashboard
          </NavLink>
          <NavLink to="/pessoas" className={({ isActive }) => `app-nav-link ${isActive ? 'active' : ''}`}>
            Cadastrar Pessoa
          </NavLink>
          <NavLink to="/transacoes" className={({ isActive }) => `app-nav-link ${isActive ? 'active' : ''}`}>
            Lançar Transação
          </NavLink>
        </nav>

        <Routes>
          <Route path="/" element={<DashboardTotais />} />
          <Route path="/pessoas" element={<PessoasForm />} />
          <Route path="/transacoes" element={<TransacoesForm />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;