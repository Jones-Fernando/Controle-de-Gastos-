import axios from 'axios';

// Cria um cliente Axios reutilizável para comunicação com o backend em .NET.
// A URL base do backend pode ser configurada via VITE_API_URL para facilitar
// a execução local ou a implantação em diferentes ambientes.
export const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5281/api'
});