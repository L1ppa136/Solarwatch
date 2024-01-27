import './App.css';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import RegistrationPage from './Page/RegistrationPage.jsx';
import LoginPage from './Page/LoginPage.jsx';
import SolarWatchPage from './Page/SolarWatchPage.jsx';
import Layout from './Component/Layout.jsx';
import HomePage from "./Page/HomePage.jsx";
import { UserContextProvider } from './Contexts/UserContext.jsx';

function App() {

  return (
    <BrowserRouter>
        <Routes>
          <Route path="/" element={<UserContextProvider/>}>
            <Route path="/" element={<Layout/>}>
              <Route path='/' element={<HomePage />} />
              <Route path='/Registration' element={<RegistrationPage />} />
              <Route path='/Login' element={<LoginPage />} />
              <Route path='/Solar-watch' element={<SolarWatchPage />} />
            </Route>
          </Route>          
        </Routes>
    </BrowserRouter>
  );
}

export default App;
