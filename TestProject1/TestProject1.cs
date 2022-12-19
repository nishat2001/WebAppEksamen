using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using Moq;
using System;
using WebapplikasjonerDel2.DAL;
using WebapplikasjonerDel2.Models;
using WebapplikasjonerDel2.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using TestProject1;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;


//using ufoSightsController = WebapplikasjonerDel2.Controllers.ufoSightsController;


namespace TestProject1
{
    public class TestProject1
    {

        private const string _isLoggedIn = "isLoggedIn";
        private const string _notLoggetInn = "";

        private readonly Mock<IUfoSightsRepository> mockRep = new Mock<IUfoSightsRepository>();
        private readonly Mock<ILogger<ufoSightsController>> mockLog = new Mock<ILogger<ufoSightsController>>();

        private readonly Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
        private readonly MockHttpSession mockSession = new MockHttpSession();




        [Fact]
        public async Task GetAllLoggedInOK()
        {
            var ufo1 = new ufoSight
            {
                id = 1,
                title = "Ufo",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };
            var ufo2 = new ufoSight
            {
                id = 1,
                title = "Ufo",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };
            var ufo3 = new ufoSight
            {
                id = 1,
                title = "Ufo",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };

            var ufoListe = new List<ufoSight>();
            ufoListe.Add(ufo1);
            ufoListe.Add(ufo2);
            ufoListe.Add(ufo3);



            mockRep.Setup(ufo => ufo.getAll()).ReturnsAsync(ufoListe);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.getAll() as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<List<ufoSight>>((List<ufoSight>)resultat.Value, ufoListe);
        }


        // Tester for å sjekke at det blir hentet ut en observasjon

        [Fact]
        public async Task GetOneSightOK()
        {
            var returUfo = new ufoSight
            {
                id = 1,
                title = "Ufo",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };


            mockRep.Setup(ufo => ufo.GetOneSight(It.IsAny<int>())).ReturnsAsync(returUfo);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.GetOneSight(It.IsAny<int>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<ufoSight>(returUfo, (ufoSight)resultat.Value);

        }


        //Tester at en ikke kan hente en obersevasjon når man har error i innputvalidering

        [Fact]
        public async Task GetOneSightLoggedInNotOK1()
        {
            var returUfo = new ufoSight
            {
                id = 1,
                title = "Ufo",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };


            mockRep.Setup(ufo => ufo.GetOneSight(It.IsAny<int>())).ReturnsAsync(returUfo);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            ufoSightsController.ModelState.AddModelError("title", "Error in inputvalidation");

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;
            // Act
            // Act
            var resultat = await ufoSightsController.GetOneSight(It.IsAny<int>()) as BadRequestResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            // Assert.Equal("Error in inputvalidation", resultat.Value);
        }


        // Tester at man ikke kan hente en sight som ikke eksiterer
        [Fact]
        public async Task GetOneSightNotOK()
        {


            mockRep.Setup(ufo => ufo.GetOneSight(It.IsAny<int>())).ReturnsAsync(() => null);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.GetOneSight(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Could not find sight", resultat.Value);
        }

        //Tester at man ikke kan hente en sight når man ikke er logget inn

        [Fact]
        public async Task GetOneSightNotLoggedIn()
        {
            mockRep.Setup(ufo => ufo.GetOneSight(It.IsAny<int>())).ReturnsAsync(() => null);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _notLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.GetOneSight(It.IsAny<int>()) as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("not logged in", resultat.Value);
        }



        [Fact]
        public async Task DeleteLoggedInOk()
        {

            mockRep.Setup(ufo => ufo.Delete(It.IsAny<int>())).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.Delete(It.IsAny<int>()) as OkResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            //Assert.Equal("Sight deleted", resultat.Value);
        }


        [Fact]
        public async Task DelelteNotOK()
        {

            mockRep.Setup(ufo => ufo.Delete(It.IsAny<int>())).ReturnsAsync(false);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.Delete(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Sight could not be deleted", resultat.Value);
        }



        [Fact]
        public async Task editSightOK()
        {

            mockRep.Setup(ufo => ufo.EditSight(It.IsAny<ufoSight>())).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            var resultat = await ufoSightsController.EditSight(It.IsAny<ufoSight>()) as OkResult;

            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            //Assert.Equal("Changes saved", resultat.Value);
        }



        [Fact]
        public async Task editSighNotOK()
        {

            var ufoSight = new ufoSight
            {
                id = 1,
                title = "Ufo",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };

            mockRep.Setup(ufo => ufo.EditSight(It.IsAny<ufoSight>())).ReturnsAsync(false);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.EditSight(It.IsAny<ufoSight>()) as NotFoundObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Changes could not be saved", resultat.Value);
        }

        [Fact]
        public async Task EditLoggedInnNotOK()
        {

            var ufoSight = new ufoSight
            {
                id = 1,
                title = "",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };

            mockRep.Setup(ufo => ufo.EditSight(ufoSight)).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            ufoSightsController.ModelState.AddModelError("title", "Error in inputvalidation");

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.EditSight(ufoSight) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Error in inputvalidation", resultat.Value);
        }

        [Fact]
        public async Task EditSightNotLoggedIn()
        {
            var ufoSight = new ufoSight
            {
                id = 1,
                title = "",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };

            mockRep.Setup(ufo => ufo.EditSight(ufoSight)).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _notLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.EditSight(ufoSight) as UnauthorizedResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            //Assert.Equal("not logged in", resultat.Value);
        }


        [Fact]
        public async Task AdminLogin()
        {
            mockRep.Setup(au => au.AdminLogin(It.IsAny<adminUser>())).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.AdminLogin(It.IsAny<adminUser>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task AdminNotOK()
        {
            mockRep.Setup(au => au.AdminLogin(It.IsAny<adminUser>())).ReturnsAsync(false);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _notLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.AdminLogin(It.IsAny<adminUser>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.False((bool)resultat.Value);
        }

        [Fact]
        public async Task AdminLogInNotCorrectInput()
        {
            mockRep.Setup(ufo => ufo.AdminLogin(It.IsAny<adminUser>())).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            ufoSightsController.ModelState.AddModelError("username", "Incorrect input in server");

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;
            // Act
            // Act
            var resultat = await ufoSightsController.AdminLogin(It.IsAny<adminUser>()) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Incorrect input in server", resultat.Value);
        }




        //Sjekker om brukeren får logget inn

        [Fact]
        public async Task TestUserLoginOK()
        {

            var inUser = new User
            {
                id = 1,
                username = "oslomet",
                password = "oslomet",
            };

            mockRep.Setup(au => au.UserLogin(It.IsAny<User>())).ReturnsAsync(inUser);


            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.UserLogin(It.IsAny<User>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<User>(inUser, (User)resultat.Value);

        }

        //Tester at brukerern ikke har feil input
        [Fact]
        public async Task UserLogInNotCorrectInput()
        {
            var inUser = new User
            {
                id = 1,
                username = "oslomet",
                password = "oslomet",


            };

            mockRep.Setup(au => au.UserLogin(It.IsAny<User>())).ReturnsAsync(inUser);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            ufoSightsController.ModelState.AddModelError("username", "Incorrect input in server");

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;
            // Act
            // Act
            var resultat = await ufoSightsController.UserLogin(It.IsAny<User>()) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Incorrect input in server", resultat.Value);
        }


        //Tester om brukeren ikke fikk logget inn

        //Tester om brukeren ikke fikk logget inn

        [Fact]
        public async Task UserLoginhNotOK()
        {


            mockRep.Setup(au => au.UserLogin(It.IsAny<User>())).ReturnsAsync(() => null);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.UserLogin(It.IsAny<User>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Null(resultat.Value);
        }
        //Tester om admin ikke kan logge seg inn på grunn av innkorekt input


        [Fact]
        public async Task AdminUserLogInInputNotOK()
        {
            mockRep.Setup(au => au.AdminLogin(It.IsAny<adminUser>())).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            ufoSightsController.ModelState.AddModelError("username", "Incorrect input in server");
            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.AdminLogin(It.IsAny<adminUser>()) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Incorrect input in server", resultat.Value);
        }


        //Tester at man får til å legge til ny bruker

        [Fact]
        public async Task AddNewUserOK()
        {
            var inUser = new User
            {
                username = "oslomet",
                password = "oslomet"

            };


            mockRep.Setup(User => User.AddNewUser(inUser)).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.AddNewUser(inUser) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);

        }


        [Fact]

        public async Task AddNewUserNotOK()
        {

            mockRep.Setup(ufo => ufo.GetOneUser(It.IsAny<int>())).ReturnsAsync(() => null);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.GetOneUser(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Could not find sight", resultat.Value);
        }


        //Tester om man ikke er logget inn

        [Fact]
        public async Task GetOneUserNotLoggedIn()
        {
            mockRep.Setup(ufo => ufo.GetOneUser(It.IsAny<int>())).ReturnsAsync(() => null);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _notLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.GetOneUser(It.IsAny<int>()) as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("not logged in", resultat.Value);
        }



        //Tester om input ikke er korrekt

        [Fact]
        public async Task GetOneUserLoggedInNotOK1()
        {
            var inUser = new User
            {
                username = "oslomet",
                password = "oslomet"

            };


            mockRep.Setup(ufo => ufo.GetOneUser(It.IsAny<int>())).ReturnsAsync(inUser);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            ufoSightsController.ModelState.AddModelError("username", "Incorrect input in server");

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;
            // Act
            // Act
            var resultat = await ufoSightsController.GetOneUser(It.IsAny<int>()) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Incorrect input in server", resultat.Value);
        }


        // Tester om det er error i input
        [Fact]
        public async Task AddNewUserLoggedInnNotOK()
        {

            var user = new User
            {
                username = "oslomet",
                password = "oslomet"
            };

            mockRep.Setup(ufo => ufo.AddNewUser(user)).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            ufoSightsController.ModelState.AddModelError("username", "Error in inputvalidation");

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.AddNewUser(user) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Error in inputvalidation", resultat.Value);
        }

        //Tester når sight ikke blir lagt til 

        [Fact]
        public async Task AddNewUserNotLoggedInnNotOK()
        {

            var user = new User
            {
                username = "oslomet",
                password = "oslomet"
            };

            mockRep.Setup(ufo => ufo.AddNewUser(user)).ReturnsAsync(false);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.AddNewUser(user) as BadRequestResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            // Assert.Equal("Sight could not be added", resultat.Value);
        }


        //Henter alle brukere

        [Fact]
        public async Task GetAllUsersOK()
        {
            var user = new User
            {
                username = "Oslomet"
            };

            var UserList = new List<User>();
            UserList.Add(user);


            mockRep.Setup(user => user.GetAllUsers()).ReturnsAsync(UserList);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.GetAllUsers() as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<List<User>>((List<User>)resultat.Value, UserList);
        }


        //Tester når en ikke er logget inn

        [Fact]
        public async Task GetAllUsersNotLoggedIn()
        {

            var user = new User
            {
                username = "Oslomet"
            };

            var UserList = new List<User>();
            UserList.Add(user);


            mockRep.Setup(user => user.GetAllUsers()).ReturnsAsync(UserList);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _notLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act

            var resultat = await ufoSightsController.GetAllUsers() as UnauthorizedObjectResult;
            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("not logged in", resultat.Value);


        }




        [Fact]
        public async Task GetOneUserNotOK()
        {

            mockRep.Setup(ufo => ufo.GetOneUser(It.IsAny<int>())).ReturnsAsync(() => null);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.GetOneUser(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Could not find sight", resultat.Value);
        }


        // Tester når man ikke får slettet bruker
        [Fact]ri
        public async Task DelelteUserNotOK()
        {

            mockRep.Setup(user => user.DeleteUser(It.IsAny<int>())).ReturnsAsync(false);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.DeleteUser(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("User could not be deleted", resultat.Value);
        }


        //Tester når man får slettet bruker

        [Fact]
        public async Task DeleteUserOk()
        {

            mockRep.Setup(user => user.DeleteUser(It.IsAny<int>())).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.DeleteUser(It.IsAny<int>()) as OkResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            // Assert.Equal("User deleted", resultat.Value);
        }


        [Fact]
        public async Task TestGetOneUserOK()
        {
            var returUser = new User
            {
                id = 1,
                username = "oslomet",
                password = "oslomet"
            };

            mockRep.Setup(User => User.GetOneUser(It.IsAny<int>())).ReturnsAsync(returUser);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.GetOneUser(It.IsAny<int>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<User>(returUser, (User)resultat.Value);
        }




        [Fact]
        public async Task TestGetOneUserNotOK()
        {
            mockRep.Setup(au => au.AdminLogin(It.IsAny<adminUser>())).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);



            ufoSightsController.ModelState.AddModelError("username", "Incorrect input in server");

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.AdminLogin(It.IsAny<adminUser>()) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Incorrect input in server", resultat.Value);
        }



        [Fact]
        public async Task AddUserPostOK() //Add new ufo bruk denne metoden
        {
            var inUfoSights = new ufoSight
            {
                id = 1,
                title = "Ufo",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful",
                imgpath = ""
            };


            mockRep.Setup(nameOfuser => nameOfuser.AddUserPost(inUfoSights, 2)).ReturnsAsync(true);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.AddUserPost(inUfoSights, 2) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);

        }

        //Tester når man legger til userpost

        [Fact]
        public async Task AddUserPostNotOk()
        {

            var inUfoSights = new ufoSight
            {
                id = 1,
                title = "Ufo",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful",
                imgpath = ""
            };


            mockRep.Setup(nameOfuser => nameOfuser.AddUserPost(inUfoSights, 2)).ReturnsAsync(false);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.AddUserPost(inUfoSights, 2) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);

        }

        //Tester når en skal legge til en ny observasjon

        [Fact]
        public async Task AddUserPostNotOK()
        {
            var ufoSight = new ufoSight
            {
                id = 1,
                title = "",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };

            mockRep.Setup(nameOfuser => nameOfuser.AddUserPost(ufoSight, 2)).ReturnsAsync(false);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            ufoSightsController.ModelState.AddModelError("title", "Incorrect input in server");

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.AddUserPost(ufoSight, 2) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Incorrect input in server", resultat.Value);

        }

        [Fact]
        public async Task AddUsersNotLoggedIn()
        {

            var ufoSight = new ufoSight
            {
                id = 1,
                title = "",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };

            mockRep.Setup(nameOfuser => nameOfuser.AddUserPost(ufoSight, 2)).ReturnsAsync(false);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _notLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.AddUserPost(ufoSight, 2) as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("not logged in", resultat.Value);
        }

        [Fact]
        public async Task GetAllUserPost()
        {


            var ufo1 = new ufoSight
            {
                id = 1,
                title = "Ufo",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };
            var ufo2 = new ufoSight
            {
                id = 1,
                title = "Ufo",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };
            var ufo3 = new ufoSight
            {
                id = 1,
                title = "Ufo",
                date = "02-02-2020",
                city = "Oslo",
                country = "Norway",
                comment = "Beautiuful"
            };

            var ufoListe = new List<ufoSight>();
            ufoListe.Add(ufo1);
            ufoListe.Add(ufo2);
            ufoListe.Add(ufo3);



            mockRep.Setup(p => p.GetAllUserPost(1)).ReturnsAsync(ufoListe);

            var ufoSightsController = new ufoSightsController(mockRep.Object, mockLog.Object);

            mockSession[_isLoggedIn] = _isLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            ufoSightsController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await ufoSightsController.GetAllUserPost(1) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
        }

    }
}