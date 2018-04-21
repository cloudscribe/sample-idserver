## This Sample Has Working IdentityServer4 Clients

If you are having any trouble gettings your own clients working with cloudscribe Core and IdentityServer4 integration, this solution provides a good reference.

If you are new to cloudscribe please see the [Introduction](https://www.cloudscribe.com/docs/introduction)

[![Build Status](https://travis-ci.org/cloudscribe/sample-idserver.svg?branch=master)](https://travis-ci.org/cloudscribe/sample-idserver) 

[![Join the chat at https://gitter.im/joeaudette/cloudscribe](https://badges.gitter.im/joeaudette/cloudscribe.svg)](https://gitter.im/joeaudette/cloudscribe?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)


# Using cloudscribe Core with IdentityServer4

cloudscribe Core and IdentityServer4 integration provides a compelling solution that makes it easy to provision new OpenIdConnect Provider server endpoints.

IdentityServer4 implements all the needed protocols for OpenIdConnect and it provides issuing of JWT tokens for authentication.

cloudscribe Core libraries provides the actual management and storage of user, roles, and claims data, and optionally provides [multi-tenancy](https://www.cloudscribe.com/docs/multi-tenant-support) so that you can host multiple sites in a single installation each with their own Users, Roles, and Claims.

cloudscribe Core IdentityServer4 integrartion libraries provide management and storage for all the operational data needed for IdentityServer4 to mange Apis and Api clients.


#### What is OpenIdConnect (in brief)

You probably have seen sites that use social authentication, ie you can login using your Facebook, Twitter, Microsoft, or Google accounts. Those are all OpenIdConnect providers.

IdentityServer4 enables you to stand up your own OpenIdConnect provider similar to those services by large companies, and it also provides federation so that you can combine those social authentication providers all in one service, ie you can optionally enable Facebook, Twitter, Microsoft, Google, or any other 3rd party OpenIdConnect provider into your IdentityServer4 OpenIdConnect Provider server.

OpenIdConnect allows you to separate the authentication and authorization back end from the API's that will be protected and from the Client applications that will access the apis so that each can run as separaste web app installations. Client applications can authenticate against the OpenIdConnect Provider which issues JWT authentication tokens. The client applications pass the JWT token as headers when making requests to the apis, and the apis can validate the tokens agsainst the OpenIdConnect Provider service.

### About the Projects in the Solution

Each project in the solution can be run by either right clicking the project in Visual Studio and choosing "view in browser", or by opening a command window on each project folder and entering the commands dotnet restore, dotnet build, and dotnet run.

#### OPServer

This project is the sample OpenIdConnect Provider server. It is just an ASP.NET Core web application with the cloudscribe and IdentityServer4 libraries wired up and configured. The rest of the projects in the solution are sample clients and sample apis that can authenticate against the OPServer app. So this one must be running to try any of the sample clients.

The OPServer project is configured with 2 "tenants", ie 2 sites in one installation, each with different users, roles, claims, clients, and apis.

* Tenant1 runs at http://localhost:50405/
* Tenant2 runs at http://localhost:50405/two

You can login directly to the OPServer tenants using the corresponding administrator credentials below, then a new Administration menu item will appear. Look around the administration area to see how to manage user,s roles, claims, clients, apis, and more or even create additional tenants if you like.

Note also that the root tenant is the master tenant, administrators in the root tenant can create new tenants and manage other tenants, but administrators in Tenant2 can only manage Tenant2 and cannot create new tenants. You will only see the Site List and buttons to create new sites in the master tenant admin area.

When you login directly to the OPServer it is using cookie authentication, whereas the clients use JWT authentication to talk to apis. Since the OPServer and the web clients all run on localhost it is best to use different browsers for each such as Chrome for the client, Firefox for the OPServer to keep the cookies isolated. Ther browser isolates cookies by host name but since localhost is always the host name for these samples there isn't isolation like you would have in a deployment to different domains.

##### Tenant1 Test Users

| Email  | Password | Roles |
| ------------- | ------------- | ------------- | 
| admin@admin.com  | $Secret12345  | Administrator|
| bob@bob.com  | $Secret12345  | none |
| jill@jill.com  | $Secret12345  | Administrator |

##### Tenant2 Test Users

| Email  | Password | Roles |
| ------------- | ------------- | ------------- | 
| admin@admin.com  | $Secret12345  | Administrator|
| jim@jim.com  | $Secret12345  | none |
| sally@sally.com  | $Secret12345  | Administrator |

Note that while both tenants have an admin@admin.com user in administrator role, they are not the same user, users are isolated per tenant, it just so happens that we created users with the same admin@admin.com credentials in both tenants, don't let that confuse you into thinking they are the same user.

#### Client and API Sample Projects

All of the sample clients can authenticate using the test user credentials shown above.

##### Tenant1SpaVueJs http://localhost:5900/

This client uses [VueJs](https://vuejs.org/) for the UI and authenticates against the root tenant of the OPServer, so you can use the Tenant1 test users. It calls an api in the Tenant1Api project, so to try this project you need to run OPServer, Tenant1Api, and Tenant1SpaVueJs projects.

##### Tenant1Api

This project just has a simple api controller, the api is consumed by the Tenant1SpaVueJs client and it validates the JWT authentication token against the OPserver project.

##### Tenant1SpaPolymer http://localhost:5010/

This client uses [Google Polymer](https://www.polymer-project.org/) for the UI and authenticates against the root tenant of the OPServer, so you can use the Tenant1 test users.

In this sample the client html and js are hosted in the same project with the asp.net core api that it consumes. It has an api that validates the JWT auth token against the OPServer so the OPServer project is the only other one that needs to be running to try this sample.

##### Tenant1SpaLocalApi http://localhost:5010/

This client uses [Google Polymer](https://www.polymer-project.org/) for the UI and authenticates against the root tenant of the OPServer, so you can use the Tenant1 test users.

This sample is almost identitcal to Tenant1SpaPolymer, except that it consumes an API that lives in the OPServer project. Thus client project is an ASP.NET Core web application but it only serves static files, it doesn't have any controllers of its own. The main reason for this project is to show the flexibility that the apis and clients and the OpServer can be separate web apps or they can also be combined, ie the apis can be in the same app as the OpServer, in the same app as the client side code, or completely separate applications at different urls.

##### Tenant2SpaPolymer http://localhost:5011/

This client uses [Google Polymer](https://www.polymer-project.org/) for the UI and authenticates against the second tenant of the OPServer, so you can use the Tenant2 test users. This sample is pretty much identical to the Tenant1SpaPolymer sample except that it authenticates against a different tenant in the OPserver project. This is mainly to demonstrate the support for multi-tenancy.

In this sample the client html and js are hosted in the same project with the asp.net core api that it consumes. It has an api that validates the JWT auth token against the OPServer so the OPServer project is the only other one that needs to be running to try this sample.

##### Xamarin Android Client

This sample is in a separate solution by itself in the [xclient folder](/xclient) because it requires additional Xamarin tooling and Android emulator to get it working. There is a [YouTube Video](https://www.youtube.com/watch?v=T7WxxYG71zE&feature=youtu.be) that shows this sample running in the emulator. It authenticates against the root tenant of the OPServer project and consumes an api that lives withing the OPServer project, so you only need the OPserver project running to test it.

##### TIPs:

We used "folder" based [multi-tenancy](https://www.cloudscribe.com/docs/multi-tenant-support) instead of "host name" based multi-tenancy for this sample just because it is easier to demo and doesn't require DNS settings to add new tenants.

We used [NoDb](https://github.com/cloudscribe/NoDb) file system based storage for this demo because that way we could commit the data files to this repo and have the clients and server all setup and working for you to try. For a real deployment you should use one of the other supported storage layers such as [Microsoft SqlServer, PostgreSql, or MySql](https://www.cloudscribe.com/docs/complete-list-of-cloudscribe-libraries)

Some of the file paths used for NoDb storage can exceed the file path limitations on Windows. Therefore it is best to clone or download this project into a very short folder path to keep the file paths as short as possible. ie I am using c:\c as my root folder for this repository and I don't run into any path too long errors, but if you put it in a deeper folder it is possible you will encounter such errors with this sample.

## Client Configuration

The client must use the correct url for the identityserver tenant that is governing it. ie in the sample the Tenant2SpaPolymer client is specifiying the url for the second tenant in the attributes in the [index.html](https://github.com/joeaudette/cloudscribe.Samples/blob/master/cloudscribe-idserver-nodb/Tenant2SpaPolymer/wwwroot/index.html) like this:

    <my-app authority="http://localhost:50405/two"
            client-id="polymerclient2"
            scopes="openid profile tenant2Spa" 
            redirect-uri="http://localhost:5011/index.html?action=popupcallback"
            silent-redirect-uri="http://localhost:5011/silent-renew.html"
            post-logout-redirect-uri="http://localhost:5011/"
            sample-api-uri="http://localhost:5011/api/identity"></my-app>

Notice the folder segment "two" in the authority url because the sample is using folder based mutli-tenancy, if it were host name based then it would just use the host name.

The Tenant1SpaPolymer client is configured in it's [index.html](https://github.com/joeaudette/cloudscribe.Samples/blob/master/cloudscribe-idserver-nodb/Tenant1SpaPolymer/wwwroot/index.html) like this, without any url segment because it uses the root tenant as its authority:

    <my-app authority="http://localhost:50405"
            client-id="polymerclient"
            scopes="openid profile tenant1Spa" 
            redirect-uri="http://localhost:5010/index.html?action=popupcallback"
            silent-redirect-uri="http://localhost:5010/silent-renew.html"
            post-logout-redirect-uri="http://localhost:5010/"
            sample-api-uri="http://localhost:5010/api/identity"></my-app>

Note that I wrote a polymer wrapper around the [oidc-client.js](https://github.com/IdentityModel/oidc-client-js)


##### Screenshots

![ploymer html client screen shot](https://github.com/joeaudette/cloudscribe/raw/master/screenshots/polymer-html-client.png)

![ploymer html client login screen shot](https://github.com/joeaudette/cloudscribe/raw/master/screenshots/polymer-html-client-login.png)

![ploymer html client logged in screen shot](https://github.com/joeaudette/cloudscribe/raw/master/screenshots/polymer-html-client-logged-in.png)

![ploymer html client api call screen shot](https://github.com/joeaudette/cloudscribe/raw/master/screenshots/polymer-html-client-api.png)


## Meta

This sample uses [cloudscribe Core](https://github.com/joeaudette/cloudscribe) for user authentication and [NoDb](https://github.com/joeaudette/NoDb) file system storage for content and data. 

[cloudscribe Core](https://github.com/joeaudette/cloudscribe) is a multi-tenant web application foundation. It provides multi-tenant identity management for sites, users, and roles.

[NoDb](https://github.com/joeaudette/NoDb) is a "No Database" file system storage, it is also a "NoSql" storage system.


### Publishing

Note that the .csproj settings are configured to exclude the nodb_storage folder from publishing. That folder is where the data is stored, so we generally don't want to overwrite production data when we redeploy. However for the first deployment you should add this folder manually. However, I don't really recommend using NoDb storage for an identity server endpoint, you would be better off using a database for that kind of usage. This starter kit is mainly to provide a working example with sample data, and NoDb made it easy to do that since the data is just files on disk.






