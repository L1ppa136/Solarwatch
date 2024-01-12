import React, { useState } from 'react';

export default function RegistrationForm({ onSubmit, onCancel }) {
  const [email, setEmail] = useState('');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [passwordSecond, setPasswordSecond] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();

    if (
      password === passwordSecond &&
      email.includes('@') &&
      email.includes('.') &&
      email.lastIndexOf('.') > email.indexOf('@') + 1 &&
      username !== null &&
      username.trim() !== ''
    ) {
      const formData = {
        email,
        username,
        password,
      };
      onSubmit(formData);
    } else {
      if (password !== passwordSecond) {
        alert('Passwords must match!');
      } else if (
        !email.includes('@') ||
        !email.includes('.') ||
        email.lastIndexOf('.') <= email.indexOf('@') + 1
      ) {
        alert('Invalid e-mail format!');
      } else if (username === null || username.trim() === '') {
        alert('Username field cannot be empty!');
      }
    }
  };

  return (
    <div>
      <form className='RegistrationForm' onSubmit={handleSubmit}>
        <label>
          E-mail address:
          <input type='email' value={email} onChange={(e) => setEmail(e.target.value)}></input>
        </label>

        <label>
          Username:
          <input type='text' value={username} onChange={(e) => setUsername(e.target.value)}></input>
        </label>

        <label>
          Password:
          <input
            type='password'
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          ></input>
        </label>

        <label>
          Password again:
          <input
            type='password'
            value={passwordSecond}
            onChange={(e) => setPasswordSecond(e.target.value)}
          ></input>
        </label>
        <div className='buttonContainer'>
          <button type='submit'>Submit</button>
          <button type='button' onClick={onCancel}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
