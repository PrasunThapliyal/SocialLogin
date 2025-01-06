04 Jan 2025
===========

Implement Login with Google
---------------------------

---------------------------
Summary:
	We want to create a Web App where user can login using Google instead of having to create their username/password on this app.
	We are essentially doing 2 things:
	(1) Use OAuth 2.0 / Open Id to use Google to authenticate the user, assuming Google implements OAuth/Open Id.
		If all we want to do us Authentication, then OpenId is what we use
		If we also want to call Google APIs, for eg, access end user's Google Drive or Photos, then we need OAuth 2.0
		In this example, we only want authentication.
	(2) Once Google successfully authenticates the user, we most likely need some user management on the app as well (unless our use case has a 1 time operation)

	To begin with, we need to create an app on Google Cloud Console.
	Among other things, we need to whitelist the URL from where authentication request will come to this app, and the redirect URL.
	The App provides us with a Client Id and Client Secret.

	[Note !!]
	We will not be able to safeguard the Client Id no matter what, despite some of the Google documentation that suggests we must
	keep the Client Id protected. However, we must protect the Client Secret.
	For Open Id alone, we don't need the Client Secret.
	My guess is that its okay to expose Client Id on the browser. A malicious user might not be able to do much by gaining this.
	For eg, if we visit Udemy.com, we can bring up inspect window and in the sources tab their client id is visible.

	Note that Google has some usage limits. We can request the limit increase - this will be necessary for a production app
---------------------------
Extended Summary:
	For Authentication, i.e part (1) of the solution, we have the choice to use Google's Javascript library, 
	or implement the OAuth flow using backend.
	In this example, we have used Google's JS library. All we have to do is call their method to render the Sign In button

	If we do want to implement OAuth using backend, we can refer this:
		ref: Google external login setup in ASP.NET Core
		https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-6.0
		and Facebook, Google, and external provider authentication in ASP.NET Core
		https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/?view=aspnetcore-6.0&tabs=visual-studio
		
		The example uses Microsoft's nuget that handles the OAuth Flow
		<PackageReference Include="Microsoft.AspNetCore.Authentication.Google" Version="6.0.36" />

		The Good:
			With this example, you can see the OAuth flow in inspect window
			Note that you need to add this to the redirect uris list in Google Console: https://localhost:7062/signin-google
			Also, with the integrated UI (Razor pages), you get sign in, sign out, forget etc out of the box
		The Bad:
			If you were expecting to be able to hide the Client Id since you are now using the backend to drive the OAuth flow
			you are in for a disappointment .. the Client Id can be seen in the request URLs in the inspect window
		The Ugly:
			Very tight integration with Razor Pages. Difficult to roll your own UI whilst still using their APIs

	The approach we've used in this example uses Google's JS library to do OAuth from the UI
	Once we have authenticated, Google returns a 'credential' .. this is a JWT token that has user info
	We can pass this token to the backend to create a user and sign him/her in
		The first commit shows how we can decode the JWT, and create a user in our app
		We rolled out our own Table with a Custom User class. Used HttpContext to sign in/sign out
		In the second commit, we used the AspNetCore.Identity framework, using the DB provided by them and their tables to create user etc
		Instead of using HttpContext.SignIn, we have now used UserManager and SignInManager.
		For now, there's only sign in and sign out, but other use cases can be implemented in similar fashion
---------------------------
---------------------------
Setup on Google Developer Cloud Console
	Create a new Google Account: prasun01.test@gmail.com
	Go to developers.google.com => Cloud => Console => Select Project -> New Project - TheCrosswordsApp (No organiation)
	=> APIs and Services => OAuth Consent Screen
	---
	OAuth consent screen management is changing. This page has been replaced with a new, easier-to-use experience. The current pages will only be available for a few more days.
	Go to New Experience
	=> Google Auth Platform => Audience => Get Started
	App name: The Crosswords App
	User support email: prasun01.test@gmail.com
	Audience: External
	Contact Information: prasun01.test@gmail.com
	I Agree
	Create

	Continue further on branding ...
	Logo: skip
	Application home page: https://localhost:7062
	Application privacy policy link: https://localhost:7062/auth/privacy
	Application terms of service link: https://localhost:7062/auth/termsOfService

	Audience:
	Test users: prasun01.test@gmail.com; prasun.thapliyal@gmail.com

	Data Access:
	Add or Remove Scopes: email, profile, openid

	Credentials:
	Create Client => 
	Troubleshooting: Unable to create a client ? Its because of ZScaler blocking your request
		[Fix]: Switch to personal laptop instead of office laptop
	Application type: Web application
	Name: Web client 1
	Authorized Javascript origins: https://localhost:7062
	Authorized redirect URIs: https://localhost:7062

	At this point, your Client ID and Client Secret are presented .. save them somewhere safe

---------------------------
TLDR for this section:
Absolutely confusing and we're not using this for now
	ref: Use social sign-in provider authentication without ASP.NET Core Identity
		Has Razor pages
		https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/social-without-identity?view=aspnetcore-7.0


				builder.Services.AddAuthentication(
					options =>
					{
						options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
						options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
					})
					.AddCookie()
					.AddGoogle(options =>
					{
						options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
						options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
					});
	..

				app.UseStaticFiles();

				app.UseAuthentication();
				app.UseAuthorization();

---------------------------
---------------------------
Few more things:
	The project is created using Dotnet Core 8
	I'm using Static Files feature to host index.html
	I created a user prasun01.test@gmail.com to create an app in the console.
	Also added prasun.thapliyal@gmail.com as one of the test users
	ChatGPT was the saviour
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------
Ver 1 implemented using ChatGPT
Not using Dotnet Core's Identity Framework - The Sample application on microsoft's website uses a WebApp project with Individual Accounts turned on
That implementation works, but is too tightly coupled with Razor UI

This version does not use any of Dotnet's Identity Framework
---------------------------
Added .gitignore and .gitattributes from here: https://github.com/dotnet/core/blob/main/.gitignore
---------------------------
Ver 2: Instead of creating our own database tables, lets try to use AspNetCore.Identity
Removed own table, and the User class we created.
No Razor
No change in index.html
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------
---------------------------



