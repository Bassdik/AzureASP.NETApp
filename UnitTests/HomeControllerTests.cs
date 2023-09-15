using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace AzureASPNETApp.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void CreateToken_ValidInput_RedirectsToSuccessView()
        {
            // Arrange
            var controller = new HomeController();
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockRequest = new Mock<HttpRequestBase>();
            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            controller.ControllerContext = new ControllerContext
            {
                Controller = controller,
                HttpContext = mockHttpContext.Object
            };

            var mail = "test@mail.com";
            var fakeFile = new Mock<HttpPostedFileBase>();
            fakeFile.Setup(f => f.InputStream).Returns(new MemoryStream());
            mockRequest.Setup(r => r.Files["CustomerFile"]).Returns(fakeFile.Object);

            // Act
            var result = controller.CreateToken(mail, fakeFile.Object) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Success", result.ViewName);
        }

        [TestMethod]
        public void CreateToken_InvalidInput_RedirectsToIndexView()
        {
            // Arrange
            var controller = new HomeController();
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockRequest = new Mock<HttpRequestBase>();
            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            controller.ControllerContext = new ControllerContext
            {
                Controller = controller,
                HttpContext = mockHttpContext.Object
            };

            // No mail and no file provided, which is invalid input
            string mail = null;
            HttpPostedFileBase fakeFile = null;

            // Act
            var result = controller.CreateToken(mail, fakeFile) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

    }
}