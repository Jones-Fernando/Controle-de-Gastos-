import { useEffect, useState, type FormEvent } from 'react';
import { api } from '../services/api';

interface Pessoa {
    id: number;
    nome: string;
    idade: number;
}

// O backend retorna o nome da pessoa vinculada à transação diretamente
// no campo PessoaNome, evitando a necessidade de serializar o objeto Pessoa.
interface Transacao {
    id: number;
    descricao: string;
    valor: number;
    tipo: string;
    pessoaId: number;
    pessoaNome?: string;
    PessoaNome?: string;
}

export function TransacoesForm() {
    const [pessoas, setPessoas] = useState<Pessoa[]>([]);
    const [transacoes, setTransacoes] = useState<Transacao[]>([]);
    const [descricao, setDescricao] = useState('');
    const [valor, setValor] = useState('');
    const [tipo, setTipo] = useState('Despesa');
    const [pessoaId, setPessoaId] = useState('');
    const [carregando, setCarregando] = useState(false);
    const [mensagem, setMensagem] = useState('');

    // Carrega os dados necessários para o formulário de transações:
    // lista de pessoas para seleção e histórico de transações já cadastradas.
    const carregarDados = async () => {
        setCarregando(true);
        try {
            const [pessoasResponse, transacoesResponse] = await Promise.all([
                api.get('/Pessoas'),
                api.get('/Transacoes')
            ]);
            setPessoas(pessoasResponse.data);
            setTransacoes(transacoesResponse.data);
        } catch (error) {
            console.error(error);
            setMensagem('Não foi possível carregar os dados.');
        } finally {
            setCarregando(false);
        }
    };

    useEffect(() => {
        carregarDados();
    }, []);

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        // Envia a transação ao backend e recarrega os dados em caso de sucesso.
        // A regra de menoridade e a validação da pessoa são aplicadas no servidor.
        try {
            await api.post('/Transacoes', {
                descricao,
                valor: Number(valor),
                tipo,
                pessoaId: Number(pessoaId)
            });
            setMensagem('Transação salva com sucesso!');
            setDescricao('');
            setValor('');
            setTipo('Despesa');
            setPessoaId('');
            carregarDados();
        } catch (error: any) {
            const responseData = error.response?.data;
            const mensagemErro = typeof responseData === 'string'
                ? responseData
                : responseData?.error ?? 'Erro inesperado ao salvar transação.';
            setMensagem(`Atenção: ${mensagemErro}`);
        }
    };

    return (
        <div className="page-card">
            <h2>Nova Transação</h2>
            {mensagem && <p className={`status-message ${mensagem.includes('Atenção') ? 'error' : 'success'}`}>{mensagem}</p>}
            <form onSubmit={handleSubmit} className="form-grid" style={{ marginBottom: '24px' }}>
                <select value={pessoaId} onChange={e => setPessoaId(e.target.value)} required className="select">
                    <option value="">Selecione quem está gastando/recebendo</option>
                    {pessoas.map(p => (
                        <option key={p.id} value={p.id}>{p.nome}</option>
                    ))}
                </select>

                <input type="text" placeholder="Descrição (ex: Padaria, Salário)" value={descricao} onChange={e => setDescricao(e.target.value)} required className="input" />

                <input type="number" step="0.01" placeholder="Valor (R$)" value={valor} onChange={e => setValor(e.target.value)} required className="input" />

                <select value={tipo} onChange={e => setTipo(e.target.value)} className="select">
                    <option value="Despesa">Despesa (Gasto)</option>
                    <option value="Receita">Receita (Ganho)</option>
                </select>

                <button type="submit" className="button button-success">
                    Salvar Transação
                </button>
            </form>

            <h3>Histórico de transações</h3>
            {carregando ? (
                <p className="muted">Carregando...</p>
            ) : transacoes.length === 0 ? (
                <p className="muted">Nenhuma transação cadastrada ainda.</p>
            ) : (
                <ul className="list">
                    {transacoes.map(transacao => (
                        <li key={transacao.id} className="list-item" style={{ flexDirection: 'column', alignItems: 'flex-start' }}>
                            <strong>{transacao.descricao}</strong>
                            <div>Valor: R$ {transacao.valor.toFixed(2)}</div>
                            <div>Tipo: {transacao.tipo}</div>
                            <div>Pessoa: {transacao.pessoaNome ?? transacao.PessoaNome ?? 'Pessoa removida'}</div>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}