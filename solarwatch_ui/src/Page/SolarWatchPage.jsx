import "./SolarWatchPage.css";
import { useEffect, useState, useContext } from 'react';
import Loading from "../Component/Loading.jsx";
import { userContext } from '../Contexts/UserContext.jsx';

const getCitiesForAutoComplete = async () => {
  const response = await fetch(`https://localhost:7015/Twilight/Cities`);
  const data = await response.json();
  return data;
};

export default function SolarWatchPage() {
  const [cityName, setCityName] = useState('');
  const todaysDate = new Date();
  const formattedDate = todaysDate.toISOString().split('T')[0]; // Format as YYYY-MM-DD
  const [date, setDate] = useState(formattedDate);
  const [twilightData, setTwilightData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [cityList, setCityList] = useState([]);
  const {isLoggedIn} = useContext(userContext);

  useEffect(() => {
    const fetchCities = async () => {
      const cities = await getCitiesForAutoComplete();
      setCityList(cities);
    };
    
    
    fetchCities();      
  }, []);

  const getSolarWatchData = async (cityName) => {
    setLoading(true);
    try {
      const requestBody = { cityName: cityName, date: date.toString() };
      const response = await fetch('https://localhost:7015/Twilight/Solarwatch', {
        method: 'POST',
        headers: {
          'Authorization': localStorage.getItem('#36Tkn'),
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(requestBody),
      });
      const data = await response.json();
      setTwilightData(data);
    } catch (error) {
      console.error(error);
    }
    setLoading(false);
  };

  return (
    <div className='TW-container'>
      {isLoggedIn ? (
        <>
          <h3 className="T&W-header">Type a city name and date(optional) to get twilight data with the current weather description!</h3>
          <label>
            City:
            <input
              type='text'
              list='cityList'
              value={cityName}
              onChange={(e) => setCityName(e.target.value)}
            ></input>
            <datalist id='cityList'>
              {cityList.map((cityName, index) => (
                <option key={index} value={cityName}></option>
              ))}
            </datalist>
          </label>
          <label>
            Date:
            <input className="react-datepicker" type='date' value={date} onChange={(e) => setDate(e.target.value)}></input>
          </label>
          {/* Use isLoading state to conditionally render the Loading component */}
          <button onClick={() => getSolarWatchData(cityName)}>
            Fetch T&W
          </button>
          {loading && <Loading />}
          {twilightData ? (
            <div id='TwilightUI'>
              <h2>Sunrise:</h2><p>{twilightData.sunrise}</p>
              <h2>Sunset:</h2><p>{twilightData.sunset}</p>
              <h2>Solarnoon:</h2><p>{twilightData.solarNoon}</p>
              <h2>Weather description (current):</h2>
              <p id="description-text">{twilightData.weatherDescription}</p>
            </div>
          ) : (<div>Fetched Data will be rendered here...</div>)}
        </>
      ) : (
        <h1 className="T&W-header">You must be logged in to access this feature!</h1>
      )}
    </div>
  );
}
