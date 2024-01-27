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
                    <Link to="/">
                        <img src="house-solid.svg" alt="Home icon" />
                    </Link>
                    <Link to='/Registration'>Register</Link>
                    <Link to='/Login'>Login</Link>
                    <Link to='/Solar-watch'>T&W</Link>
                </nav>
            </header>
            <Outlet>

            </Outlet>
        </div>
    );
  };

  export default Layout;