import { useEffect, useState } from 'react';
import { api } from '../services/api';

// o componente DashboardTotais consome o endpoint /Pessoas/totais
// para exibir os totais de receitas, despesas e saldo por pessoa,
// além dos valores consolidados de todas as pessoas cadastradas.
interface PessoaResumo {
    id: number;
    nome: string;
    idade: number;
    totalReceitas: number;
    totalDespesas: number;
    saldo: number;
}

interface TotaisResponse {
    pessoas: PessoaResumo[];
    totalGeral: {
        totalReceitas: number;
        totalDespesas: number;
        saldo: number;
    };
}

// Formata os valores financeiros no padrão brasileiro para exibição clara no dashboard.
const formatarMoeda = (valor: number) =>
    new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(valor);

export function DashboardTotais() {
    const [dados, setDados] = useState<TotaisResponse | null>(null);
    const [erro, setErro] = useState('');
    const [carregando, setCarregando] = useState(true);

    useEffect(() => {
        // Carrega os totais uma única vez quando o componente é montado.
        // A API retorna o cálculo de receitas, despesas e saldo para cada pessoa
        // e também o total geral de todas as pessoas.
        const carregarTotais = async () => {
            try {
                const response = await api.get('/Pessoas/totais');
                setDados(response.data);
            } catch (error) {
                console.error(error);
                setErro('Não foi possível carregar os totais no momento.');
            } finally {
                setCarregando(false);
            }
        };

        carregarTotais();
    }, []);

    if (carregando) {
        return <div className="page-card"><p className="muted">Carregando totais...</p></div>;
    }

    if (erro) {
        return <div className="page-card"><p className="status-message error">{erro}</p></div>;
    }

    if (!dados) {
        return <div className="page-card"><p className="muted">Nenhum dado disponível.</p></div>;
    }

    return (
        <div className="page-card">
            <h2>Consulta de Totais</h2>
            <div className="metric-grid">
                <div className="metric-card receita">
                    <h3>Receitas</h3>
                    <h2>{formatarMoeda(dados.totalGeral.totalReceitas)}</h2>
                </div>

                <div className="metric-card despesa">
                    <h3>Despesas</h3>
                    <h2>{formatarMoeda(dados.totalGeral.totalDespesas)}</h2>
                </div>

                <div className={`metric-card saldo ${dados.totalGeral.saldo >= 0 ? '' : 'despesa'}`}>
                    <h3>Saldo Total</h3>
                    <h2>{formatarMoeda(dados.totalGeral.saldo)}</h2>
                </div>
            </div>

            <div style={{ marginTop: '24px', overflowX: 'auto' }}>
                <table className="table">
                    <thead>
                        <tr>
                            <th>Pessoa</th>
                            <th>Receitas</th>
                            <th>Despesas</th>
                            <th>Saldo</th>
                        </tr>
                    </thead>
                    <tbody>
                        {dados.pessoas.map(pessoa => (
                            <tr key={pessoa.id}>
                                <td>{pessoa.nome}</td>
                                <td>{formatarMoeda(pessoa.totalReceitas)}</td>
                                <td>{formatarMoeda(pessoa.totalDespesas)}</td>
                                <td>{formatarMoeda(pessoa.saldo)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}