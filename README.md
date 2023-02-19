# BACKEND DEVELOPER SCREENING TEST

c# coding test
1. Go to cars.com and log in using the credentials.
2. Select "used cars" then select " tesla" then "model s" then max price " 100K" then set distance to "all miles from" then set the zipcode to be 94596.
3. Click search for results.
4. Gather all data for all cars on the first 2 pages.
5. Choose a specific car and gather specific car data.
6. After this click " home delivery" and gather all data.
7. After this from the search results page click "model x" and do the same (4-5-6) with results.
8. Export the results as a JSON file.

Also as an additional note: Use CefSharp.OffScreen
Login credentials for cars.com:
Username: johngerson808@gmail.com Password: test8008

# FRONTEND DEVELOPER SCREENING TEST
 
Angular Front End Coding Test
Before start to work together, we have a little test project just to see how you do things, like code quality, best practices, efficiency and speed.
• Build a simple web site with Angular 11. You will provide a working “Vs Code” Buildable Project with build and run instructions.
• Use WebPack and Bootstrap 4
• Make frequent updates to GitHub with full description of commits
• WebSite will have a user login, logout feature.
• WebSite will connect to a WebSocket, when the websocket sends a “logout” message, the logged in user will be logged off the forwarded to the login page.
• WebSite will have a welcome screen, welcoming the user with his own message. (You will be getting that messsage from an API call)
• Authentication will be done through REST API calls.
 
Frontend projesi için gereken diğer bilgiler:

Note that to set: "[PATH_TO_CHROME]\chrome.exe" --disable-web-security

API and WebSocket info:
 
POST URL :
 
http://66.70.229.82:8181/Authorize   (Returns token as ApiResponse Data);
 
JSON REQUEST :
 
{
    "email": "sean@test.com",
    "password": "SeanPass"
}
 
SAMPLE RESPONSE :
 
{
    "data": {
        "userGroup": 0,
        "token": "8a2cfe059e8642b7adc367c6415428a6"
    },
    "status": 0,
    "error": null
}
 
API REQUEST MODEL
 
public class ApiRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
 
 
API RESPONSE MODEL
 
    public class ApiResult<string>
    {
        [JsonPropertyName("data")]
        public string Data { get; set; }
        [JsonPropertyName("status")]
        public ApiStatus Status { get; set; }
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
 
    public enum ApiStatus
    {
        Ok = 0,
        Error = 1
    }
 
 
GET URL :  http://66.70.229.82:8181/GetGreeting (Returns greeting as ApiResponse Data)
 
x-user-token (Send token with header as x-user-token with request)
 
API RESPONSE MODEL
 
    public class ApiResult<string>
    {
        [JsonPropertyName("data")]
        public string Data { get; set; }
        [JsonPropertyName("status")]
        public ApiStatus Status { get; set; }
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
 
    public enum ApiStatus
    {
        Ok = 0,
        Error = 1
    }
 
 
WEB SOCKET URL : ws://66.70.229.82:8181/?{token}
 
send token returned from Api after calling Authorize, to connect to web socket. Example usage : ws://66.70.229.82:8181/?28eb0c1405ed4df68ecc4a1a12ceb3cb
 
SOCKET MESSAGE STRUCTURE (JSON)
 
    public class SocketMessage
    {
        public SocketMessageType MessageType { get; set; }
        public DateTime TimeStamp { get; set; }
    }
 
    public enum SocketMessageType
    {
        Ping = 0,
        LogOff = 1
    }
 
EXAMPLE REQUEST (JSON):
 
{"messageType":0}
 
EXAMPLE RESPONSE (JSON):
 
{
  "MessageType": 0,
  "TimeStamp": "2021-08-05T22:52:31.1216874Z"
}
