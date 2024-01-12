import React, { useState } from 'react';
import './App.css';
import { BrowserRouter, Route, Link, Routes } from 'react-router-dom';
import RegistrationPage from './Page/RegistrationPage.jsx';
import LoginPage from './Page/LoginPage.jsx';
import SolarWatchPage from './Page/SolarWatchPage.jsx';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  return (
    <BrowserRouter>
      <div className='App'>
        <header className='App-header'>
          <nav className='NavBar'>
            <Link to='/Registration'>Register</Link>
            <Link to='/Login'>Login</Link>
            <Link to='/Solar-watch'>Solar-watch</Link>
          </nav>
        </header>
        <Routes>
          <Route path='/Registration' element={<RegistrationPage />} />
          <Route
            path='/Login'
            element={<LoginPage isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />}
          />
          <Route path='/Solar-watch' element={<SolarWatchPage isLoggedIn={isLoggedIn} />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
