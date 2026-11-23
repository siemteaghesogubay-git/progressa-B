using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using progressa.Controllers;
using progressa.Dtos;
using progressa.Services.IServices;
using System.Security.Claims;


namespace Progressa.Tests.Controllers
{
    public class UserControllerTests
    {
        [Fact]
        public async Task DeleteUser_Success_ShouldReturn204AndUseIdFromToken()
        {
            //Arrange - mock som svarar att raderingen lyckades, och en inloggad användare med id "user-123"
            var mock = new Mock<IUserService>();

            mock.Setup(service => service.DeleteAccountAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(DeleteUserResult.Ok());

            // Controller med en inloggad användare (id "user-123" i token)
            var controller = new UserController(mock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "user-123")], "Test"))
                    }
                }
            };

            //Act - anropa DeleteUser med ett lösenord
            var result = await controller.DeleteUser(new DeleteAccountDto { Password = "Test123!" });

            //Assert - svaret är 204, och servicen fick id:t från token
            Assert.IsType<NoContentResult>(result);
            mock.Verify(s => s.DeleteAccountAsync("user-123", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ShouldReturn404()
        {
            //Arrange - mock som svarar att användaren inte finns
            var mock = new Mock<IUserService>();

            mock.Setup(service => service.DeleteAccountAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(DeleteUserResult.NotFound());

            // Controller med en inloggad användare (id "user-123" i token)
            var controller = new UserController(mock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "user-123")], "Test"))
                    }
                }
            };

            //Act - anropa DeleteUser med ett lösenord
            var result = await controller.DeleteUser(new DeleteAccountDto { Password = "Test123!" });

            //Assert - svaret är 404, och klienten får meddelandet att användaren inte hittades
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var problem = Assert.IsType<ProblemDetails>(notFound.Value);
            Assert.Equal("Användaren hittades inte.", problem.Title);
        }

        [Fact]
        public async Task DeleteUser_InvalidPassword_ShouldReturn400WithPasswordError()
        {
            //Arrange - mock som svarar att lösenordet är fel
            var mock = new Mock<IUserService>();

            mock.Setup(service => service.DeleteAccountAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(DeleteUserResult.InvalidPassword());

            // Controller med en inloggad användare (id "user-123" i token)
            var controller = new UserController(mock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "user-123")], "Test"))
                    }
                }
            };

            //Act - anropa DeleteUser med ett lösenord
            var result = await controller.DeleteUser(new DeleteAccountDto { Password = "Test123!" });

            //Assert - svaret är 400 (inte 401), och felet ligger på fältet Password
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);
            Assert.Contains("Fel lösenord.", problem.Errors[nameof(DeleteAccountDto.Password)]);
        }


        [Fact]
        public async Task DeleteUser_Failed_ShouldReturn500()
        {
            //Arrange - mock som svarar att raderingen misslyckades
            var mock = new Mock<IUserService>();

            mock.Setup(service => service.DeleteAccountAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(DeleteUserResult.Failed(["Databasfel."]));

            // Controller med en inloggad användare (id "user-123" i token)
            var controller = new UserController(mock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "user-123")], "Test"))
                    }
                }
            };

            //Act - anropa DeleteUser med ett lösenord
            var result = await controller.DeleteUser(new DeleteAccountDto { Password = "Test123!" });

            //Assert - svaret är 500, och klienten får bara det allmänna meddelandet
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var problem = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Account couldn't be deleted", problem.Title);

        }

        [Fact]
        public async Task DeleteUser_NoUserIdInToken_ShouldReturn401AndNotCallService()
        {
            //Arrange - mock utan svar: servicen ska aldrig anropas
            var mock = new Mock<IUserService>();

            // Controller med en användare som saknar id i token
            var controller = new UserController(mock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity([], "Test"))
                    }
                }
            };

            //Act - anropa DeleteUser med ett lösenord
            var result = await controller.DeleteUser(new DeleteAccountDto { Password = "Test123!" });

            //Assert - svaret är 401, och servicen anropades aldrig
            Assert.IsType<UnauthorizedResult>(result);
            mock.Verify(s => s.DeleteAccountAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
