import { Outlet, Link } from "react-router-dom";


const Layout = () => {
    return (
        <div className='App'>
            <header className='App-header'>
                <div id="logo-container-navbar">
                   <img src="cloud_png.png" alt="Cloud Logo"/>
                   <h1>Twilight & Weather</h1>
                </div>
                <nav className='NavBar'>
                    <Link to='/Registration'>Register</Link>
                    <Link to='/Login'>Login</Link>
                    <Link to='/Solar-watch'>T&W Data</Link>
                </nav>
            </header>
            <Outlet>

            </Outlet>
        </div>
    );
  };

  export default Layout;