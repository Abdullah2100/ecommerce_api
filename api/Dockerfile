#this to use the dotnet from coker 
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

#this to copy and resotre package 
COPY  . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app
#this to builde the final image 

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS runtime
WORKDIR /app


#create image directory 
RUN mkdir -p /app/images && chmod -R 777 /app/images
#this to copy the dll file and dotent package from build file to app directory
COPY --from=build /app ./

ENTRYPOINT ["DOTNET","api.dll"]