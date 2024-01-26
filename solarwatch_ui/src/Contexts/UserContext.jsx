import { createContext, useState, useEffect } from 'react';
import { Outlet } from 'react-router-dom';

export const userContext = createContext(null);

const CheckTokenIfValid = (tokenAsString) => {
    return fetch('https://localhost:7015/Auth/CheckTokenIfValid', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(tokenAsString),
    })
      .then((authenticationResponse) => {
        if (!authenticationResponse.ok) {
          throw new Error(`Invalid token: ${authenticationResponse.status}`);
        }
        return authenticationResponse.json();
      })
      .catch((err) => {
        console.error('Token validation error', err);
      });
  };
export function UserContextProvider(){
    const [isLoggedIn, setIsLoggedIn] = useState(false);

    useEffect(() => {
        const myToken = localStorage.getItem("#36Tkn");
        const checkToken = async () => {
            if (myToken) {
                try {
                    const response = await CheckTokenIfValid(myToken.split(" ")[1]);
                    if (response.ok) {
                        setIsLoggedIn(true);
                    }else{
                        localStorage.removeItem("#36Tkn");
                        setIsLoggedIn(false);
                    }
                } catch (error) {
                    console.error('Token validation error', error);
                }
            }
        };
        checkToken();    
      },[]);

    return (
        <userContext.Provider value={{isLoggedIn, setIsLoggedIn}}>
            <Outlet>

            </Outlet>
        </userContext.Provider>
    );
}