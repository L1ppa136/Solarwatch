import React, { useState } from 'react';
import './LoginForm.css';

export default function LoginForm({ onSubmit, onCancel }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = (e) => {
    e.preventDefault();

    const credentials = {
      email,
      password,
    };
    onSubmit(credentials);
  };

  return (
    <div>
      <form className='LoginForm' onSubmit={handleLogin}>
        <label>
          Email:
          <input type='email' value={email} onChange={(e) => setEmail(e.target.value)}></input>
        </label>

        <label>
          Password:
          <input
            type='password'
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          ></input>
        </label>

        <div className='buttonContainer'>
          <button type='submit'>Login</button>
          <button type='button' onClick={onCancel}>
            Back
          </button>
        </div>
      </form>
    </div>
  );
}
