import { useState, useContext } from 'react';
import "./LoginPage.css";
import LoginForm from '../Component/LoginForm';
import { useNavigate } from 'react-router-dom';
import { userContext } from '../Contexts/UserContext';
import Loading from '../Component/Loading';

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

export default function LoginPage() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [username, setUsername] = useState('');
  const [showLoginMessage, setShowLoginMessage] = useState(false);
  const {isLoggedIn, setIsLoggedIn} = useContext(userContext);

  const handleLogin = (credentials) => {
    setLoading(true);

    LoginUser(credentials)
      .then((authenticationResponse) => {
        setUsername(authenticationResponse.username);
        localStorage.setItem('#36Tkn', `Bearer ${authenticationResponse.token}`);
        setShowLoginMessage(true);
        setIsLoggedIn(true);
        setTimeout(() => {
          setShowLoginMessage(false);
          navigate('/Solar-watch');
        }, 2000);
      })
      .catch((err) => console.error(err))
      .finally(() => setLoading(false));
  };

  const handleLogout = () => {
    setIsLoggedIn(false);
    localStorage.removeItem('#36Tkn');
  };

  return (
    <div>
      {loading && <Loading />}
      {showLoginMessage && <h3 id="welcome-login-text">Welcome, {username}</h3>}
      {isLoggedIn ? (
        <>
          <h1 id="logged-in-text">You are logged in!</h1>
          <button id="logout-button" onClick={handleLogout}>Logout</button>
        </>
      ) : (
        <LoginForm onSubmit={handleLogin} onCancel={() => navigate('/')} />
      )}
    </div>
  );
}
