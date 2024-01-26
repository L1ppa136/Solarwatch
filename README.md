If you want to have a look to the web application, please follow these instructions:
1. Clone repository
2. Acquire APIKey from https://openweathermap.org/
3. Acquire APIKey from https://platform.openai.com/docs/overview
4. Open the .sln file in your IDE
5. Create user-secrets
6. Add your Openweathermap APIKey to your user-secrets as ""WeatherAPIKey"
7. Add your OpenAI API key to your user-secrets as "OpenAI_APIKey"
8. Add a preferred issuer signing key to user-secrets of your chosing under the name of "IssuerSigningKey"
9. Install Docker Desktop
10. Run "docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest" in your command line, exchange "yourStrong(!)Password" to your chosing
11. Setup server using your preferred app, e.g. Azure Data Studio
12. Run Solarwatch in IDE
13. Open solarwatch_ui folder in your IDE
14. Open new terminal
15. Type npm install
16. Type npm run, ui will run on port 3000
