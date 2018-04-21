# Based on Asp.NETCore 2.0 Vue 2 Starter - by [DevHelp.Online](http://www.DevHelp.Online)
https://github.com/MarkPieszak/aspnetcore-Vue-starter

This VueJs sample client was contributed by [Paul Van Bladel](https://github.com/paulvanbladel)

# Installation

The Vuejs project should start without prior installation, it has webpack hot module middleware that will automatically install the dependencies and generate the output files in wwwroot/dist.

Changes can be made while the sample is running and the hot module middleware will automatically re-generate the dist files

# How to run the Vuejs project in visual studio

1. Start the OPServer project by right click the project and choose view in browser
2. Right click the Tenant1Api project and choose view in browser, since this one is just an api project there is no UI and you will see a blank page for this in the browser, that is ok.
3. Right click the Tenant1SpaVueJs project and choose view in browser. This one is the VueSp sample, it can authenticate against the OPserver project and it calls the Api in the Tenant1Api project which is protected by the OPServer as its authority.

# How to run from the command line

1. Open a command window (or pwoershell) on the OPServer project and enter the commands dotnet restore, then dotnet build, then dotnet run
2. You can optionally view the OPserver in the browser at http://localhost:50405/ but best to use a different web browser than the Vue Sample if you intend to login directly to the OPServer project so that no auth cookie is shared between projects (which would happen since both are on localhost).
3. Open a command window on the Tenant1Api folder then enter the commands dotnet restore, dotnet build, dotnet run - that will get the api running at http://localhost:5901/
4. Open a command window on the Tenant1SpaVueJs project folder and enter the commands dotnet restore, dotnet build, dotnet run
5. Open a web browser (ideally not the same browser at you have the OPServer open) at http://localhost:5900/

# Login Credentials

See the main README.md in the root of the solution for information about the existing Tenant1 user account credentials you can use to test the client.


