import { useState } from 'react'
import { useAuthStore } from '../store/useAuthStore'

const SignUpForm = () => {
    const [name, setName] = useState("")
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")
    const [gender, setGender] = useState("")
    const [age, setAge] = useState("")
    const [genderPreferences, setGenderPreferences] = useState([])

    const { signup, loading } = useAuthStore()

    const changePrefs = (e) => {
        let newPrefs = genderPreferences
        if (newPrefs.includes(e.target.value)) {
            newPrefs = newPrefs.filter(val => val != e.target.value)
        }
        else {
            newPrefs.push(e.target.value)
        }
        //console.log(newPrefs)
        setGenderPreferences(newPrefs)
    }

    return (
        <form className='space-y-6' onSubmit={(e) => {
            e.preventDefault()
            signup({ name, email, password, age, gender, genderPreferences })
        }}>
            <div>
                <label htmlFor='name' className='block text-sm font-medium text-gray-700'>
                    Name
                </label>
                <div className='mt-1'>
                    <input
                        id='name'
                        name='name'
                        type='text'
                        required
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        className='appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-pink-500 focus:border-pink-500 sm:text-sm'
                    />
                </div>
            </div>

            <div>
                <label htmlFor='email' className='block text-sm font-medium text-gray-700'>
                    Email address
                </label>
                <div className='mt-1'>
                    <input
                        id='email'
                        name='email'
                        type='email'
                        autoComplete='email'
                        required
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        className='appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-pink-500 focus:border-pink-500 sm:text-sm'
                    />
                </div>
            </div>

            <div>
                <label htmlFor='password' className='block text-sm font-medium text-gray-700'>
                    Password
                </label>
                <div className='mt-1'>
                    <input
                        id='password'
                        name='password'
                        type='password'
                        autoComplete='current-password'
                        required
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        className='appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-pink-500 focus:border-pink-500 sm:text-sm'
                    />
                </div>
            </div>

            <div>
                <label htmlFor='age' className='block text-sm font-medium text-gray-700'>
                    Age
                </label>
                <div className='mt-1'>
                    <input
                        id='age'
                        name='age'
                        type='number'
                        required
                        value={age}
                        onChange={(e) => setAge(e.target.value)}
                        min='18'
                        max='500'
                        className='appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-pink-500 focus:border-pink-500 sm:text-sm'
                    />
                </div>
            </div>

            <div>
                <label className='block text-sm font-medium text-gray-700'>
                    Your Gender
                </label>
                <div className='mt-2 flex gap-2'>
                    <div className='flex items-center'>
                        <input
                            id='man'
                            name='gender'
                            type='radio'
                            checked={gender === "man"}
                            onChange={() => setGender("man")}
                            className='h-4 w-4 text-pink-600 focus:ring-pink-500 border-gray-300'
                        />
                        <label htmlFor='man' className='ml-2 block text-sm text-gray-900'>
                            Man
                        </label>
                    </div>
                    <div className='flex items-center'>
                        <input
                            id='woman'
                            name='gender'
                            type='radio'
                            checked={gender === "woman"}
                            onChange={() => setGender("woman")}
                            className='h-4 w-4 text-pink-600 focus:ring-pink-500 border-gray-300'
                        />
                        <label htmlFor='woman' className='ml-2 block text-sm text-gray-900'>
                            Woman
                        </label>
                    </div>
                    <div className='flex items-center'>
                        <input
                            id='other'
                            name='gender'
                            type='radio'
                            checked={gender === "other"}
                            onChange={() => setGender("other")}
                            className='h-4 w-4 text-pink-600 focus:ring-pink-500 border-gray-300'
                        />
                        <label htmlFor='other' className='ml-2 block text-sm text-gray-900'>
                            Other
                        </label>
                    </div>
                </div>
            </div>
            <div>
                <label className='block text-sm font-medium text-gray-700'>Show Me</label>
                <div className='mt-2 space-y-2'>
                    <div className='flex items-center'>
                        <input
                            id='prefer-men'
                            name='gender-preference'
                            type='checkbox'
                            value='men'
                            
                            onChange={changePrefs}
                            className='h-4 w-4 rounded border-gray-300 text-pink-600 focus:ring-pink-500'
                        />
                        <label htmlFor='prefer-men' className='ml-2 block text-sm text-gray-900'>
                            Men
                        </label>
                    </div>
                    <div className='flex items-center'>
                        <input
                            id='prefer-women'
                            name='gender-preference'
                            type='checkbox'
                            value='women'
                            
                            onChange={changePrefs}
                            className='h-4 w-4 rounded border-gray-300 text-pink-600 focus:ring-pink-500'
                        />
                        <label htmlFor='prefer-women' className='ml-2 block text-sm text-gray-900'>
                            Women
                        </label>
                    </div>
                    <div className='flex items-center'>
                        <input
                            id='prefer-other'
                            name='gender-preference'
                            type='checkbox'
                            value='other'
                            
                            onChange={changePrefs}
                            className='h-4 w-4 rounded border-gray-300 text-pink-600 focus:ring-pink-500'
                        />
                        <label htmlFor='prefer-other' className='ml-2 block text-sm text-gray-900'>
                            Other
                        </label>
                    </div>
                </div>
            </div>

            <div>
                <button
                    type='submit'
                    className={`w-full flex justify-center py-2 px-4 border-transparent
                        rounded-md shadow-sm text-sm font-medium text-white ${
                        loading
                            ? "bg-pink-400 cursor-not-allowed"
                            : "bg-pink-600 hover:bg-pink-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-pink-500"
                        }`}
                    disabled={loading}
                >
                    {loading ? "Signing up..." : "Sign up"}
                </button>
            </div>
        </form>
    )
}

export default SignUpForm