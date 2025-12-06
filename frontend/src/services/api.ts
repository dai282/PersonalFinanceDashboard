import axios from 'axios';
import { Auth0Client } from '@auth0/auth0-spa-js';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7012/api';

// For Docker Compose
// const API_BASE_URL = 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Create Auth0 client instance
const auth0Client = new Auth0Client({
  domain: process.env.REACT_APP_AUTH0_DOMAIN!,
  clientId: process.env.REACT_APP_AUTH0_CLIENT_ID!,
  authorizationParams: {
    audience: process.env.REACT_APP_AUTH0_AUDIENCE,
  },
});

// Request interceptor to add token to requests
api.interceptors.request.use(
  async (config) => {
    //const token = localStorage.getItem('token');
    //instead of getting token from localStorage, get from auth0
    const token = await auth0Client.getTokenSilently();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    // Only redirect to login if we have a token AND get 401
    // Don't redirect if we're already on the login page trying to login
    //if (error.response?.status === 401 && localStorage.getItem('token')) {
    if (error.response?.status === 401) {
      // Token expired or invalid
      // localStorage.removeItem('token');
      // localStorage.removeItem('user');
      // window.location.href = '/login';

      //if unauthorized, redirect to auth0 login
      await auth0Client.loginWithRedirect();
    }
    return Promise.reject(error);
  }
);

export default api;