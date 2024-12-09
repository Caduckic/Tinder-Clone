import axios from 'axios'

// TODO baseUrl will have to change during deployment
export const axiosInstance = axios.create({
    baseURL: 'https://localhost:7178/api',
    withCredentials: true
})