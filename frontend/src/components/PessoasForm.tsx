import { useEffect, useState, type FormEvent } from 'react';
import { api } from '../services/api';

// Estrutura usada no frontend para representar uma pessoa cadastrada.
// O campo id é gerado automaticamente pelo backend, assim como em um banco relacional clássico.
interface Pessoa {
    id: number;
    nome: string;
    idade: number;
}

export function PessoasForm() {
    const [nome, setNome] = useState('');
    const [idade, setIdade] = useState('');
    const [pessoas, setPessoas] = useState<Pessoa[]>([]);
    const [carregando, setCarregando] = useState(false);
    const [mensagem, setMensagem] = useState('');

    // Carrega as pessoas já cadastradas no backend.
    // Esse método é executado ao montar o componente e sempre que
    // uma operação de criação ou remoção for realizada.
    const carregarPessoas = async () => {
        setCarregando(true);
        try {
            const response = await api.get('/Pessoas');
            setPessoas(response.data);
        } catch (error) {
            console.error(error);
            setMensagem('Não foi possível carregar as pessoas.');
        } finally {
            setCarregando(false);
        }
    };

    useEffect(() => {
        carregarPessoas();
    }, []);

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        try {
            await api.post('/Pessoas', { nome, idade: Number(idade) });
            setMensagem('Pessoa cadastrada com sucesso!');
            setNome('');
            setIdade('');
            carregarPessoas();
        } catch (error) {
            console.error(error);
            setMensagem('Erro ao cadastrar pessoa.');
        }
    };

    const handleDelete = async (id: number) => {
        const confirmar = window.confirm('Deseja remover esta pessoa e todas as transações vinculadas?');
        if (!confirmar) {
            return;
        }

        try {
            await api.delete(`/Pessoas/${id}`);
            setMensagem('Pessoa removida com sucesso!');
            carregarPessoas();
        } catch (error) {
            console.error(error);
            setMensagem('Erro ao remover pessoa.');
        }
    };

    return (
        <div className="page-card">
            <h2>Cadastrar Nova Pessoa</h2>
            {mensagem && <p className={`status-message ${mensagem.includes('erro') || mensagem.includes('Erro') ? 'error' : 'success'}`}>{mensagem}</p>}
            <form onSubmit={handleSubmit} className="form-grid" style={{ marginBottom: '24px' }}>
                <input
                    type="text"
                    placeholder="Nome"
                    value={nome}
                    onChange={e => setNome(e.target.value)}
                    required
                    className="input"
                />
                <input
                    type="number"
                    placeholder="Idade"
                    value={idade}
                    onChange={e => setIdade(e.target.value)}
                    required
                    className="input"
                />
                <button type="submit" className="button button-primary">
                    Salvar Pessoa
                </button>
            </form>

            <h3>Pessoas cadastradas</h3>
            {carregando ? (
                <p className="muted">Carregando...</p>
            ) : pessoas.length === 0 ? (
                <p className="muted">Nenhuma pessoa cadastrada ainda.</p>
            ) : (
                <ul className="list">
                    {pessoas.map(pessoa => (
                        <li key={pessoa.id} className="list-item">
                            <span>
                                <strong>{pessoa.nome}</strong> • {pessoa.idade} anos
                            </span>
                            <button onClick={() => handleDelete(pessoa.id)} className="button button-danger">
                                Excluir
                            </button>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}