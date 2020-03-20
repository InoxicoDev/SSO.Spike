# SSO.Spike

This project is a proof of concept in order to verify that a Third Party user coming from a Third Party application can log in to an Inoxico application without having to log in to Inoxico having a Single Signon experience.

The Inoxico Target App is a javascript based application using the OIDC client for authenticating with Identity Server using the Implicit flow.

How to test it out
===

Starting up the project presents you with 4 browser windows if you have Visual Studio setup normally. The only windows of interest are the ones containing the addresses https://localhost:44302 and https://localhost:44304.
The first one simulates a normal Inoxican login experience. The second one, the Third Party experience. The 3rd party user will have to login first to the 3rd party application and then be transferred to the Target application (which is located on the Inoxico domain).
You will have to press the Login button on both scenarios in order to login. I didn't play too much with the Javascript but you'll notice that pressing Login as the Inoxico user will prompt you to login, while pressing Login as the 3rd Party user will just immediately log you in (since you're already logged in).
You will know you're logged in when the token contents gets displayed.

How it works
===

The Third Party application needs to connect to a Service exposed by the Inoxico domain and with that pass through a given `Client Id` and `User Identity Token` (the Identity and **NOT** Access Token from the 3rd Party application). The service will return with a URL that the 3rd Party user will redirect to. The user will be directed to the Inoxico STS's Authorize endpoint and will be propagating that information along. The Inoxico Identity Server has a 3rd Party Identity Provider built and configured to deal with this incoming `User Identity Token` and `Client Id`. It will attempt to contact the 3rd Party STS in order to verify that the `User Identity Token` is valid by using the `Client Id` as a lookup. Once that is verified the claims are populated with the 3rd party user's `User Id` which is matched against the Users in the Inoxico STS using the `Provider Id` field. I've updated the UserService so that it fails if it can't match the users by the `Provider Id`. Once the Authorization is complete, it will redirect the user to the Intended application page where the user will need to click on Login for the cookie to be sent to the Inoxico STS in order to return with the access token and no login screens are required.
