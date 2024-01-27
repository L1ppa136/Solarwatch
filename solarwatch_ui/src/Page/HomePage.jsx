import "./HomePage.css";

export default function HomePage() {
  return (
  <div id="welcome-container">
    <h1>Welcome to Twilight & Weather!</h1>
    <p>Twilight & Weather, true to its name, stands as a straightforward web application crafted with React.js and C#, backed by a dockerized MSSQL server for storing data on queried cities and twilight information. An exciting addition is the ability to obtain a conversational description of the current weather conditions through 
        <a href="https://platform.openai.com/docs/overview">OpenAI.</a>
    <br/><br/>Feel free to explore by clicking on T&W in the navigation bar! Thank you for visiting!
    </p>    
   </div>    
  )
}
