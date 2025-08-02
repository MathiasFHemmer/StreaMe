# Stage 1: Build your app and produce StreaMe.dll
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /src

COPY *.sln ./
COPY Api/src/Api.csproj Api/src/

COPY App/src/App.csproj App/src/
COPY App/tests/App.Tests.csproj App/tests/

COPY Data/src/Data.csproj Data/src/
COPY Data/tests/Data.Tests.csproj Data/tests/

COPY Providers/src/Providers.csproj Providers/src/
COPY Providers/tests/Providers.Tests.csproj Providers/tests/

COPY Lib/src/Lib.csproj Lib/src/
COPY Lib/tests/Lib.Tests.csproj Lib/tests/

RUN dotnet restore 

COPY . .

RUN dotnet publish Api/src/Api.csproj -c Release -o /app/publish

WORKDIR /src/Ui
RUN npm install
RUN ENV_PATH=.env.docker npm run build

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
COPY --from=build /src/cert.pfx .

EXPOSE 5000
