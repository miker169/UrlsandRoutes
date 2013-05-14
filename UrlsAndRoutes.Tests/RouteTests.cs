using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UrlsAndRoutes.Tests
{
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Routing;

    using Moq;

    [TestClass]
    public class RouteTests
    {
        private HttpContextBase CreateHttpContext(string targetUrl = null, string httpMethod = "GET")
        {
            // create the mock request
            var mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Setup(m => m.AppRelativeCurrentExecutionFilePath).Returns(targetUrl);
            mockRequest.Setup(m => m.HttpMethod).Returns(httpMethod);

            // create the mock response
            var mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Setup(m => m.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

            // create the mock context, using the request and response
            var mockContext = new Mock<HttpContextBase>();
            mockContext.Setup(m => m.Request).Returns(mockRequest.Object);
            mockContext.Setup(m => m.Response).Returns(mockResponse.Object);

            //return the mocked context
            return mockContext.Object;

        }

        private void TestRouteMatch(
            string url, string controller, string action, object routeproperties = null, string httpMethod = "GET")
        {
            //Arrange
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            //Act
            var result = routes.GetRouteData(this.CreateHttpContext(url, httpMethod));
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(TestIncomingRouteResult(result, controller, action, routeproperties));

        }

        private bool TestIncomingRouteResult(
            RouteData routeResult, string controller, string action, object propertySet = null)
        {
            Func<object, object, bool> valCompare =
                (v1, v2) => StringComparer.InvariantCultureIgnoreCase.Compare(v1, v2) == 0;

            var result = valCompare(routeResult.Values["controller"], controller)
                         && valCompare(routeResult.Values["action"], action);

            if (propertySet != null)
            {
                var propInfo = propertySet.GetType().GetProperties();
                if (
                    propInfo.Any(
                        pi =>
                        !(routeResult.Values.ContainsKey(pi.Name)
                          && valCompare(routeResult.Values[pi.Name], pi.GetValue(propertySet, null)))))
                {
                    result = false;
                }
            }
            return result;
        }

        private void TestRouteFail(string url)
        {
            // Arrange
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            // Act -process the route
            var result = routes.GetRouteData(this.CreateHttpContext(url));

            //Assert
            Assert.IsTrue(result == null || result.Route == null);
        }

        [TestMethod]
        public void TestIncomingRoutes()
        {
            // check for the URL that we hope to receive
            this.TestRouteMatch("~/", "Home", "Index");

            this.TestRouteMatch("~/Home", "Home", "Index");
            this.TestRouteMatch("~/Home/Index", "Home", "Index");

            this.TestRouteMatch("~/Home/About", "Home", "About");
            this.TestRouteMatch("~/Home/About/MyId", "Home", "About", new { id = "MyId" });
            this.TestRouteMatch(
                "~/Home/About/MyId/More/Segments", "Home", "About", new { id = "MyId", catchall = "More/Segments" });

            this.TestRouteFail("~/Home/OtherAction");
            this.TestRouteFail("~/Account/Index");
            this.TestRouteFail("~/Account/About");


        }
    }
}