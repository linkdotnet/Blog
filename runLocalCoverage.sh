#!/bin/bash

rm -rf ./TestResults ./CoverageReport
dotnet test -c Release --coverage --coverage-output-format cobertura
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:"./TestResults/*.cobertura.xml" -targetdir:"./CoverageReport" -reporttypes:"htmlsummary"
