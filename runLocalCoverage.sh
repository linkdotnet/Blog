#!/bin/bash

dotnet test --collect:"XPlat Code Coverage" --no-build --verbosity normal -p:CollectCoverage=true -p:CoverletOutputFormat="opencover"
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator.exe -reports:".\tests\*\coverage.opencover.xml" -targetdir:".\CoverageReport" -reporttypes:"htmlsummary"