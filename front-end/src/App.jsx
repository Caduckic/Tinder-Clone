import { Navigate, Route, Routes } from 'react-router-dom'
import HomePage from './pages/HomePage'
import AuthPage from './pages/AuthPage'
import ProfilePage from './pages/ProfilePage'
import ChatPage from './pages/ChatPage'
import { useAuthStore } from './store/useAuthStore'
import { useEffect } from 'react'
import { Toaster } from 'react-hot-toast'

function App() {
    const { checkAuth, authUser, checkingAuth } = useAuthStore()
    useEffect(() => {
        checkAuth()
    }, [checkAuth])

    if (checkingAuth) return <p>loading...</p>

    return (
        <div className="-z-10 bg-[linear-gradient(to_right,#f0f0f0_1px,transparent_1px),linear-gradient(to_bottom,#f0f0f0_1px,transparent_1px)] absolute inset-0 h-full w-full bg-white bg-[size:6rem_4rem]">
            <Routes>
                <Route path="/" element={authUser ? <HomePage /> : <Navigate to={"/auth"} />} />
                <Route path="/auth" element={!authUser ? <AuthPage /> : <Navigate to={"/"} />} />
                <Route path="/profile" element={authUser ? <ProfilePage /> : <Navigate to={"/auth"} /> } />
                <Route path="/chat/:id" element={authUser ? <ChatPage /> : <Navigate to={"/auth"} />} />
            </Routes>

            <Toaster />
        </div>
    )
}

export default App
