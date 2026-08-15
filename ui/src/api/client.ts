import axios from 'axios';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5169';

const client = axios.create({ baseURL: BASE_URL });

export default client;
