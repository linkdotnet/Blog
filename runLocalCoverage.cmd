rmdir /s /q .\TestResults 2>nul
rmdir /s /q .\CoverageReport 2>nul
dotnet test -c Release --coverage --coverage-output-format cobertura
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:".\TestResults\*.cobertura.xml" -targetdir:".\CoverageReport" -reporttypes:"html"