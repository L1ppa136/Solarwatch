import React, { useState } from 'react';
import LoginForm from '../Component/LoginForm';
import { useNavigate } from 'react-router-dom';

const LoginUser = (credentials) => {
  return fetch('https://localhost:7015/Auth/Login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(credentials),
  })
    .then((authenticationResponse) => {
      if (!authenticationResponse.ok) {
        throw new Error(`Login failed with status: ${authenticationResponse.status}`);
      }
      return authenticationResponse.json();
    })
    .catch((err) => {
      console.error('Login error', err);
      alert('Login failed. Please check your credentials and try again.');
    });
};

export default function LoginPage({ isLoggedIn, setIsLoggedIn }) {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [username, setUsername] = useState('');
  const [showLoginMessage, setShowLoginMessage] = useState(false);

  const handleLogin = (credentials) => {
    setLoading(true);

    LoginUser(credentials)
      .then((authenticationResponse) => {
        setUsername(authenticationResponse.username);
        localStorage.setItem('jwtToken', `Bearer ${authenticationResponse.token}`);
        setShowLoginMessage(true);
        setIsLoggedIn(true);
        setTimeout(() => {
          setShowLoginMessage(false);
          navigate('/Solar-watch');
        }, 3000);
      })
      .catch((err) => console.error(err))
      .finally(() => setLoading(false));
  };

  const handleLogout = () => {
    setIsLoggedIn(false);
    localStorage.removeItem('jwtToken');
  };

  return (
    <div>
      {loading && <p>Loading...</p>}
      {showLoginMessage && <h3>Welcome, {username}</h3>}
      {isLoggedIn ? (
        <>
          <h1>You are already logged in!</h1>
          <button onClick={handleLogout}>Logout</button>
        </>
      ) : (
        <LoginForm onSubmit={handleLogin} onCancel={() => navigate('/')} />
      )}
    </div>
  );
}
