import { create } from 'zustand'
import { axiosInstance } from '../lib/axios'
import toast from 'react-hot-toast'
import axios from 'axios'

export const useAuthStore = create((set) => ({
    authUser: null,
    checkingAuth: true,
    loading: false,

    signup: async (data) => {
        try {
            set({ loading: true })
            const genderToInt = { man: 0, men: 0, woman: 1, women: 1, other: 2 }
            data.gender = genderToInt[data.gender]
            data.genderPreferences = data.genderPreferences.map(g => genderToInt[g])
            const res = await axiosInstance.post('/SignUp', data)
            axiosInstance.defaults.headers.common['Authorization'] = `Bearer ${res.data.accessToken}` // doesnt work TODO fix
            console.log(res.data)
            set({ authUser: res.data.user })
            toast.success('Account created succssfully')
        } catch (error) {
            toast.error(error.response.data.message || 'Something went wrong')
        } finally {
            set({ loading: false })
        }
    },

    checkAuth: async () => {
        try {
            const res = await axiosInstance.get('/Auth')
            console.log(res.data.user)
            set({authUser: res.data})
        } catch (error) {
            console.error(error)
        } finally {
            set({checkingAuth: false})
        }
    }
}))