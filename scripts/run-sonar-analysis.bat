@echo off

REM Install the SonarQube scanner if not already installed
dotnet tool install --global dotnet-sonarscanner

REM Run the analysis
dotnet sonarscanner begin /k:"eCommerceOnlineShop" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="admin" /d:sonar.password="admin"

REM Build the solution
dotnet build

REM End the analysis
dotnet sonarscanner end /d:sonar.login="admin" /d:sonar.password="admin" 