import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import RegistrationForm from '../Component/RegistrationForm.jsx';

const RegisterUser = (newUser) => {
  return fetch('https://localhost:7015/Auth/Register', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(newUser),
  })
    .then((res) => {
      return res.json();
    })
    .catch((err) => console.error('Registration error', err));
};

export default function RegistrationPage() {
  const navigate = useNavigate();
  const [showSuccessMessage, setShowSuccessMessage] = useState(false);

  const handleRegistration = (newUser) => {
    RegisterUser(newUser)
      .then(() => {
        setShowSuccessMessage(true);
        setTimeout(() => {
          setShowSuccessMessage(false);
          navigate('/Login');
        }, 3000);
      })
      .catch((error) => {
        console.error('Registration error', error);
        throw error;
      });
  };

  return (
    <div>
      {showSuccessMessage && <h2>Successful registration, welcome!</h2>}
      <RegistrationForm onSubmit={handleRegistration} onCancel={() => navigate('/Login')} />
    </div>
  );
}
