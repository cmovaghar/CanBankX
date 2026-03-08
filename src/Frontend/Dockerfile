FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

RUN apt update \
    && apt install -y python3 \
    && dotnet workload install wasm-tools

COPY . .
RUN dotnet publish Frontend.csproj --output /app

# Runtime stage
FROM nginx:latest
COPY --from=build /app/wwwroot/ /usr/share/nginx/html/
COPY nginx.conf /etc/nginx/nginx.conf