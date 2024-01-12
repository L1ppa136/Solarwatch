import React, { useEffect, useState } from 'react';

const getCitiesForAutoComplete = async () => {
  const response = await fetch(`https://localhost:7015/Twilight/Cities`);
  const data = await response.json();
  return data;
};

export default function SolarWatchPage({ isLoggedIn }) {
  const [cityName, setCityName] = useState('');
  const todaysDate = new Date();
  const formattedDate = todaysDate.toISOString().split('T')[0]; // Format as YYYY-MM-DD
  const [date, setDate] = useState(formattedDate);
  const [twilightData, setTwilightData] = useState({});
  const [loading, setLoading] = useState(false);
  const [cityList, setCityList] = useState([]);

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
          'Authorization': localStorage.getItem('jwtToken'),
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
    <div>
      {isLoggedIn ? (
        <>
          <h1>Welcome to Solarwatch, type a city name and date(optional) to get twilight data!</h1>
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
            <input type='date' value={date} onChange={(e) => setDate(e.target.value)}></input>
          </label>
          <button onClick={() => getSolarWatchData(cityName)}>Get Twilight</button>
          {twilightData && (
            <div className='TwilightUI'>
              {loading && <p>Loading...</p>}
              <p>Sunrise: {twilightData.sunrise}</p>
              <p>Sunset: {twilightData.sunset}</p>
              <p>Solarnoon: {twilightData.solarNoon}</p>
            </div>
          )}
        </>
      ) : (
        <h1>You must be logged in to access this page!</h1>
      )}
    </div>
  );
}
