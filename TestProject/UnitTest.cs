using BookStore.Controllers;
using Business_Logic_Layer;
namespace TestProject
{
    [Collection("ControllerTests")]
    public class UnitTest
    {
        private readonly UsersController _usersController;
        public UnitTest(UsersController usersController)
        {
            _usersController = usersController;
        }
        [Fact]
        public async Task ShouldTestUsersController()
        {
            var user = new UserViewModel
            {
                Username = "Ali",
                Password = "acbd",
                Email = "ali@gmail.com",
                Name = "Ali",
                UserRoleId = 1
            };
            var signupUser = _usersController.SignUp(user);
            Assert.NotNull(signupUser);
        }
    }
}