import axios from 'axios';
import { Platform } from 'react-native';

// Use 10.0.2.2 for Android emulator to access local host, or your backend's local IP (e.g. 192.168.1.100)
const BASE_URL = Platform.OS === 'android' ? 'http://10.0.2.2:5286/api' : 'http://localhost:5286/api';

const api = axios.create({
  baseURL: BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);

export default api;
